using System;
using System.Collections;
using UnityEngine;

namespace UnfrozenTestWork
{
    [RequireComponent(typeof(UnitAnimatorController))]
    public class UnitController : MonoBehaviour
    {
        private UnitAnimatorController _animatorController;

        private void Awake()
        {
            _animatorController = GetComponent<UnitAnimatorController>();
        }

        public void Idle()
        {
            _animatorController.SetCharacterState(PlayerAnimationState.Idle, true);
        }

        public IEnumerator Attack(Action onComplete = null)
        {
            float animationDuration = _animatorController.SetCharacterState(PlayerAnimationState.Attack, false, Idle);
            yield return new WaitForSecondsRealtime(animationDuration);
        }

        public IEnumerator TakeDamage(Action onComplete = null)
        {
            float animationDuration = _animatorController.SetCharacterState(PlayerAnimationState.TakeDamage, false, Idle);
            yield return new WaitForSecondsRealtime(animationDuration);
        }

        public IEnumerator Charge(Action onComplete = null)
        {
            float animationDuration = _animatorController.SetCharacterState(PlayerAnimationState.Charge, false, Idle);
            yield return new WaitForSecondsRealtime(animationDuration);
        }

        public IEnumerator DoubleShift(Action onComplete = null)
        {
            float animationDuration = _animatorController.SetCharacterState(PlayerAnimationState.DoubleShift, false, Idle);
            yield return new WaitForSecondsRealtime(animationDuration);
        }

        public void Run()
        {
            _animatorController.SetCharacterState(PlayerAnimationState.Run, true);
        }
    }
}
