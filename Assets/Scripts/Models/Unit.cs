using System;
using System.Collections;
using TMPro;
using UnfrozenTestWork;
using UnityEngine;

[RequireComponent(typeof(UnitController))]
public class Unit : MonoBehaviour
{
    private UnitController _unitController;
    private UnitSelectionDisplayer _unitSelectionDisplayer;
    private BoxCollider2D _boxCollider2d;
    private TextMeshPro _unitInfo;
    private HealthBar _healthBar;
    private InitiativeBar _initiativeBar;

    public UnitData UnitData { get; private set; }
    public bool IsAlive => UnitData.Health > 0;
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
    }

    private void UpdateUnitData(UnitData unitData)
    {
        UnitData = unitData;
        _unitInfo.text = ToString();
        if (UnitData.Type == UnitType.Player)
        {
            transform.localScale = new Vector3(1f, 1f);
        }

        if (UnitData.Type == UnitType.Enemy)
        {
            transform.localScale = new Vector3(-1f, 1f);
        }
    }

    public IEnumerator TakeTurn(Unit target)
    {
        yield return Attack(target);
        Deselect();
    }

    public IEnumerator SkipTurn()
    {
        Debug.Log($"{this} skipping the turn.");
        yield return null;
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

        var newUnitData = UnitData.TakeDamage(damage);
        UpdateUnitData(newUnitData);

        Deselect();
        _unitController.Idle();
    }

    private IEnumerator MoveToTarget(
        Vector3 targetPosition,
        float stopDistance = 0.1f,
        float speed = 10f
    )
    {
        float dotProduct = Vector2.Dot(new Vector2(transform.position.x, 0), new Vector2(targetPosition.x, 0));
        if (dotProduct == -1)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y);
        }

        _unitController.Run();

        while (Mathf.Abs(transform.position.x - targetPosition.x) > stopDistance)
        {
            var x = Mathf.Lerp(
                transform.position.x,
                targetPosition.x,
                speed * Time.deltaTime
            );
            transform.position = new Vector3(x, transform.position.y);
            yield return null;
        }
    }

    public override string ToString()
    {
        return $"Unit:{UnitData.Type}, Initiative:{UnitData.Initiative}, Health: {UnitData.Health}, Damage: {UnitData.Damage}.";
    }
}
