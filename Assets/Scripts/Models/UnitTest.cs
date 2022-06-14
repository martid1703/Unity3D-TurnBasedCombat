// using System;
// using System.Collections;
// using TMPro;
// using UnfrozenTestWork;
// using UnityEngine;

// [RequireComponent(typeof(PlayerController))]
// public class UnitTest : MonoBehaviour
// {
//     private PlayerController _animationController;
//     public UnitData UnitData { get; private set; }
//     public SpriteRenderer SpriteRenderer { get; private set; }
//     private TextMeshPro _unitInfo;
//     private Color _startColor;
//     private Color _selectedColor;
//     private bool _isSelected;


//     public bool IsAlive => UnitData.Health > 0;

//     void Awake()
//     {
//         _animationController = GetComponent<PlayerController>();
//         SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
//         _unitInfo = transform.GetComponentInChildren<TextMeshPro>();
//     }

//     void Start()
//     {
//         _startColor = SpriteRenderer.color;
//         _selectedColor = Color.cyan;
//     }

//     void OnMouseEnter()
//     {
//         var currentColor = SpriteRenderer.color;
//         if (_isSelected)
//         {
//             SpriteRenderer.color = currentColor;
//             return;
//         }
//         SpriteRenderer.color = Color.red;
//     }

//     void OnMouseExit()
//     {
//         if (!_isSelected)
//         {
//             SpriteRenderer.color = _startColor;
//         }
//     }

//     public EventHandler UnitSelected;
//     public void Select()
//     {
//         _isSelected = true;
//         if (_isSelected)
//         {
//             SpriteRenderer.color = _selectedColor;
//             UnitSelected?.Invoke(this, new EventArgs());
//         }
//     }

//     private void OnMouseUp()
//     {
//         Select();
//     }

//     public void UpdateUnitData(UnitData unitData)
//     {
//         UnitData = unitData;
//         _unitInfo.text = ToString();
//     }

//     public IEnumerator TakeTurn(UnitTest target, Action onTurnComplete)
//     {
//         yield return Attack(target, onTurnComplete);
//     }

//     public IEnumerator SkipTurn(Action onTurnComplete)
//     {
//         Debug.Log($"{this} skipping the turn.");
//         yield return null;
//         onTurnComplete();
//     }

//     private IEnumerator Attack(UnitTest attackedUnit, Action onAttackComplete)
//     {
//         Debug.Log($"{this} attacking the target: {attackedUnit}.");

//         Vector3 startPosition = transform.position;
//         Vector3 targetPosition = GetTargetPosition(attackedUnit);
//         yield return MoveToTarget(targetPosition);

//         Vector3 attackDir = (attackedUnit.transform.position - transform.position).normalized;
//         yield return _animationController.PlayAnimAttack(attackDir, null, () => { _animationController.PlayAnimIdle(); });

//         yield return attackedUnit.TakeDamage(UnitData.Damage);

//         yield return MoveToTarget(startPosition);

//         onAttackComplete();
//     }

//     private Vector3 GetTargetPosition(UnitTest attackedUnit)
//     {
//         float selfThickness = SpriteRenderer.bounds.extents.x;
//         float targetThickness = attackedUnit.SpriteRenderer.bounds.extents.x;
//         Vector3 targetPosition;
//         if (IsTargetToTheLeft(attackedUnit))
//         {
//             targetPosition = new Vector3(attackedUnit.transform.position.x + selfThickness + targetThickness, attackedUnit.transform.position.y);
//         }
//         else
//         {
//             targetPosition = new Vector3(attackedUnit.transform.position.x - selfThickness - targetThickness, attackedUnit.transform.position.y);
//         }

//         return targetPosition;
//     }

//     private bool IsTargetToTheLeft(UnitTest target)
//     {
//         return transform.position.x > target.transform.position.x;
//     }

//     public void DestroySelf()
//     {
//         Destroy(transform.gameObject);
//     }

//     public IEnumerator TakeDamage(int damage)
//     {
//         Debug.Log($"Taking {damage} damage.");

//         SpriteRenderer.color = Color.red;

//         yield return new WaitForSecondsRealtime(0.5f);

//         var newUnitData = UnitData.TakeDamage(damage);
//         UpdateUnitData(newUnitData);

//         SpriteRenderer.color = _startColor;
//         _isSelected = false;
//     }

//     private IEnumerator MoveToTarget(Vector3 targetPosition, float stopDistance = 0.5f, float speed = 10f)
//     {
//         //TODO: play sliding animation

//         while (Vector3.Distance(transform.position, targetPosition) > stopDistance)
//         {

//             transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);

//             yield return null;
//         }
//     }

//     public override string ToString()
//     {
//         return $"UnitTest:{UnitData.Type}, \nInitiative:{UnitData.Initiative}, \nHealth: {UnitData.Health}, \nDamage: {UnitData.Damage}.";
//     }
// }
