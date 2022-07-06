using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace UnfrozenTestWork
{
    [RequireComponent(typeof(IUnitController))]
    public class UnitModel : MonoBehaviour
    {
        [SerializeField]
        public UnitData UnitData;

        public bool IsSelectedAsTarget;
        public bool IsSelectedAsAttacker;
        public bool IsAlive => UnitData.Health > 0;
        public bool IsEnemy => UnitData.Belonging == UnitBelonging.Enemy;

        private IUnitController _unitController;
        private UnitSelectionDisplayer _unitSelectionDisplayer;
        private BoxCollider2D _boxCollider2d;


        void Awake()
        {
            _unitController = GetComponent<IUnitController>();
            _boxCollider2d = GetComponentInChildren<BoxCollider2D>();
            _unitSelectionDisplayer = GetComponentInChildren<UnitSelectionDisplayer>();
        }

        void Start() { }

        public Func<UnitModel, bool> IsUnitSelectable;
        public Func<UnitModel, bool> IsUnitSelectableAsTarget;

        public EventHandler UnitSelected;

        public void SelectAsTarget()
        {
            IsSelectedAsTarget = true;
            _unitSelectionDisplayer.SelectAsTarget();
            UnitSelected?.Invoke(this, new EventArgs());
        }

        public void OnBattleSpeedChange(object sender, BattleSpeedEventArgs args)
        {
            _unitController.SetBattleSpeed(BattleSpeedConverter.GetAnimationSpeed(args.Speed));
            UnitData.ChangeMoveSpeed(BattleSpeedConverter.GetUnitMoveSpeed(args.Speed));
        }


        void OnMouseEnter()
        {
            if (!IsUnitSelectable(this) || IsSelectedAsTarget || IsSelectedAsAttacker)
            {
                return;
            }
            _unitSelectionDisplayer.Highlight();
            _unitController.ShowUnitInfo(ToString());
        }

        private void OnMouseUp()
        {
            if (!IsUnitSelectableAsTarget(this))
            {
                return;
            }
            _unitController.HideUnitInfo();
            SelectAsTarget();
        }

        public EventHandler UnitOnMouseExit;

        void OnMouseExit()
        {
            _unitController.HideUnitInfo();
            UnitOnMouseExit?.Invoke(this, new EventArgs());
            if (IsSelectedAsTarget || IsSelectedAsAttacker)
            {
                return;
            }
            _unitSelectionDisplayer.Deselect();
        }

        public void Kill()
        {
            UnitData.TakeDamage(UnitData.Health);
            Deselect();
            Destroy(transform.gameObject);
        }

        public void SelectAsAttacker()
        {
            _unitSelectionDisplayer.SelectAsAttacker();
            IsSelectedAsAttacker = true;
            IsSelectedAsTarget = false;
        }

        public void Deselect()
        {
            IsSelectedAsAttacker = false;
            IsSelectedAsTarget = false;
            _unitSelectionDisplayer.Deselect();
        }

        public void Initialize(UnitData unitData)
        {
            UnitData = unitData;
            ResetLookDirection();
            _unitController.Initialize(unitData);
        }

        private void ResetLookDirection()
        {
            if (UnitData.Belonging == UnitBelonging.Player)
            {
                _unitController.SetLookDirection(Vector3.right);
            }

            if (UnitData.Belonging == UnitBelonging.Enemy)
            {
                _unitController.SetLookDirection(Vector3.left);
            }
        }

        public IEnumerator TakeTurn(UnitModel target)
        {
            Debug.Log($"{this} taking his turn on the target: {target}.");
            yield return Attack(target);
            Deselect();
        }

        public IEnumerator SkipTurn()
        {
            Debug.Log($"{this} skipping the turn.");
            Deselect();
            yield return null;
        }

        private IEnumerator Attack(UnitModel attackedUnit)
        {
            Debug.Log($"{this} attacking the target: {attackedUnit}.");

            Vector3 startPosition = transform.position;
            Vector3 targetPosition = GetTargetPosition(attackedUnit);
            yield return MoveToTarget(targetPosition);

            Vector3 attackDir = (attackedUnit.transform.position - transform.position).normalized;
            yield return _unitController.Attack();

            yield return attackedUnit.TakeDamage(damage: UnitData.Damage);

            yield return MoveToTarget(startPosition);

            ResetLookDirection();

            _unitController.Idle();
        }

        private Vector3 GetTargetPosition(UnitModel attackedUnit)
        {
            float selfThickness = _boxCollider2d.bounds.extents.x;
            float targetThickness = attackedUnit._boxCollider2d.bounds.extents.x;
            Vector3 targetPosition;
            if (IsTargetToTheLeft(attackedUnit))
            {
                targetPosition = new Vector3(
                    attackedUnit.transform.position.x + selfThickness + targetThickness,
                    attackedUnit.transform.position.y
                );
            }
            else
            {
                targetPosition = new Vector3(
                    attackedUnit.transform.position.x - selfThickness - targetThickness,
                    attackedUnit.transform.position.y
                );
            }

            return targetPosition;
        }

        private bool IsTargetToTheLeft(UnitModel target)
        {
            return transform.position.x > target.transform.position.x;
        }

        public event EventHandler UnitIsDead;

        public IEnumerator TakeDamage(int damage)
        {
            yield return _unitController.TakeDamage(damage, UnitData);
            UnitData.TakeDamage(damage);
            Deselect();

            if (!IsAlive)
            {
                _unitController.Die();
                UnitIsDead?.Invoke(this, new EventArgs());
                yield break;
            }

            _unitController.Idle();
        }

        private IEnumerator MoveToTarget(
            Vector3 targetPosition,
            float stopDistance = 0.1f)
        {
            Vector3 heading = targetPosition - transform.position;
            _unitController.FlipUnitOrientationIfNeeded(heading);

            StartCoroutine(_unitController.Run());

            while (Math.Abs(heading.x) > stopDistance)
            {
                var x = Mathf.MoveTowards(
                    transform.position.x,
                    targetPosition.x,
                    UnitData.MoveSpeed * Time.deltaTime
                );
                transform.position = new Vector3(x, transform.position.y);
                heading = targetPosition - transform.position;
                yield return null;
            }
        }

        public override string ToString()
        {
            return $"Unit:{UnitData.Type}, Initiative:{UnitData.Initiative}, Health: {UnitData.Health}, Damage: {UnitData.Damage}.";
        }
    }
}
