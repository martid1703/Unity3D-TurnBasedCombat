using System;
using System.Collections;
using UnityEngine;

namespace UnfrozenTestWork
{
    public interface IUnitController
    {
        IEnumerator Attack(Action onComplete = null);
        void Die();
        void Idle();
        void Initialize(UnitData unitData);
        IEnumerator Run();
        void SetBattleSpeed(float speed);
        IEnumerator TakeDamage(float damage, UnitData unitData, Action onComplete = null);
        void ShowUnitInfo(string message);
        void HideUnitInfo();
        void SetLookDirection(Vector3 lookDirection);
        void FlipUnitOrientationIfNeeded(Vector3 targetPosition);
    }
}