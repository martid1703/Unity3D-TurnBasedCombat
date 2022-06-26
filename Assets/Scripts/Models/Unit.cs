using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace UnfrozenTestWork
{
    [RequireComponent(typeof(UnitController))]
    public class Unit : MonoBehaviour
    {
        private UnitController _unitController;
        private UnitSelectionDisplayer _unitSelectionDisplayer;
        private BoxCollider2D _boxCollider2d;
        private TextMeshPro _unitInfo;
        private HealthBar _healthBar;
        private InitiativeBar _initiativeBar;

        [SerializeField]
        public UnitData UnitData;
        public bool IsAlive => UnitData.Health > 0;
        public bool IsEnemy => UnitData.Type == UnitType.Enemy;
        public bool IsSelected { get; private set; }

        void Awake()
        {
            _unitController = GetComponent<UnitController>();
            _boxCollider2d = GetComponentInChildren<BoxCollider2D>();
            _unitInfo = transform.GetComponentInChildren<TextMeshPro>();
            _healthBar = GetComponentInChildren<HealthBar>();
            _initiativeBar = GetComponentInChildren<InitiativeBar>();
            _unitSelectionDisplayer = GetComponentInChildren<UnitSelectionDisplayer>();
        }

        void Start() { }

        public Func<Unit, bool> IsUnitSelectable;

        void OnMouseEnter()
        {
            if (!IsUnitSelectable(this))
            {
                return;
            }
            BattleManager.Instance.SetAttackCursor();
            _unitSelectionDisplayer.Highlight();
        }

        private void OnMouseUp()
        {
            if (!IsUnitSelectable(this))
            {
                return;
            }
            SelectAsTarget();
        }

        void OnMouseExit()
        {
            BattleManager.Instance.SetRegularCursor();
            if (!IsUnitSelectable(this) || IsSelected)
            {
                return;
            }
            _unitSelectionDisplayer.Deselect();
        }

        public void DestroySelf()
        {
            Destroy(transform.gameObject);
        }

        public EventHandler UnitSelected;

        public void SelectAsTarget()
        {
            IsSelected = true;
            _unitSelectionDisplayer.SelectAsTarget();
            UnitSelected?.Invoke(this, new EventArgs());
        }

        public void OnBattleSpeedChange(object sender, BattleSpeedEventArgs args)
        {
            _unitController.SetBattleSpeed(BattleSpeedConverter.GetAnimationSpeed(args.Speed));
            UnitData.ChangeMoveSpeed(BattleSpeedConverter.GetUnitMoveSpeed(args.Speed));
        }

        public void SelectAsAttacker()
        {
            _unitSelectionDisplayer.SelectAsAttacker();
        }

        public void Deselect()
        {
            IsSelected = false;
            _unitSelectionDisplayer.Deselect();
        }

        public void Initialize(UnitData unitData)
        {
            _healthBar.Initialize(unitData.Health);
            _initiativeBar.Initialize(unitData.Initiative);
            UpdateUnitData(unitData);
            ResetLookDirection();
        }

        private void ResetLookDirection()
        {
            if (UnitData.Type == UnitType.Player)
            {
                transform.right = Vector3.right;
            }

            if (UnitData.Type == UnitType.Enemy)
            {
                transform.right = Vector3.left;
            }
        }

        private void UpdateUnitData(UnitData unitData)
        {
            UnitData = unitData;
            _unitInfo.text = ToString();
        }

        public IEnumerator TakeTurn(Unit target)
        {
            yield return Attack(target);
            Deselect();
        }

        public IEnumerator SkipTurn()
        {
            Debug.Log($"{this} skipping the turn.");
            Deselect();
            yield return new WaitForSeconds(1f);
        }

        private IEnumerator Attack(Unit attackedUnit)
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

        private Vector3 GetTargetPosition(Unit attackedUnit)
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

        private bool IsTargetToTheLeft(Unit target)
        {
            return transform.position.x > target.transform.position.x;
        }

        public IEnumerator TakeDamage(int damage)
        {
            yield return _unitController.TakeDamage(damage, UnitData);

            UnitData.TakeDamage(damage);
            UpdateUnitData(UnitData);
            Deselect();

            if (!IsAlive)
            {
                _unitController.Die();
                yield break;
            }

            _unitController.Idle();
        }

        private IEnumerator MoveToTarget(
            Vector3 targetPosition,
            float stopDistance = 0.1f)
        {
            Vector3 heading = targetPosition - transform.position;
            if (NeedFlipUnitOrientation(heading))
            {
                transform.right = heading;
            }

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

        private bool NeedFlipUnitOrientation(Vector3 targetPosition)
        {
            Vector3 heading = targetPosition - transform.position;

            return (heading.x > 0 && transform.right.x < 0) || (heading.x < 0 && transform.right.x > 0);
        }

        public override string ToString()
        {
            return $"Unit:{UnitData.Type}, Initiative:{UnitData.Initiative}, Health: {UnitData.Health}, Damage: {UnitData.Damage}.";
        }
    }
}
