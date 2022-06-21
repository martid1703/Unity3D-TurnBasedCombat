using System;
using System.Collections;
using UnityEngine;

namespace UnfrozenTestWork
{
    [RequireComponent(typeof(UnitAnimatorController))]
    public class UnitController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _popupPrefab;
        private UnitAnimatorController _animatorController;
        private HealthBar _healthBar;
        private float _battleSpeed;

        private void Awake()
        {
            _animatorController = GetComponent<UnitAnimatorController>();
            _healthBar = GetComponentInChildren<HealthBar>();
        }

        public void SetBattleSpeed(float speed)
        {
            _animatorController.SetAnimationSpeed(speed);
            _battleSpeed=speed;
        }

        public void Idle()
        {
            _animatorController.SetCharacterState(PlayerAnimationState.Idle, true);
        }

        public IEnumerator Attack(Action onComplete = null)
        {
            float animationDuration = _animatorController.SetCharacterState(PlayerAnimationState.Attack, false);
            yield return new WaitForSecondsRealtime(animationDuration);
        }

        public IEnumerator TakeDamage(float damage, UnitData unitData, Action onComplete = null)
        {
            float animationDuration = _animatorController.SetCharacterState(PlayerAnimationState.TakeDamage, false);
            StartCoroutine(ShowDamagePopup(damage, unitData.Health, animationDuration));

            _healthBar.SetReduceHPSpeed(BattleSpeedConverter.GetUnitMoveSpeed(_battleSpeed));
            StartCoroutine(_healthBar.TakeDamage(damage));
            yield return new WaitForSecondsRealtime(animationDuration);
        }

        private IEnumerator ShowDamagePopup(float damage, float health, float destroyTime)
        {
            var popup = Instantiate(_popupPrefab, transform.position, Quaternion.identity);
            if (IsCriticalDamage(damage, health))
            {
                Debug.Log($"Taking CRITICAL damage: {damage}. Current HP: {health}. Health remaining: {health - damage}.");
                yield return popup.GetComponent<DamagePopup>().CriticalPopup(damage, transform.position, destroyTime);
            }
            else
            {
                Debug.Log($"Taking damage: {damage}. Current HP: {health}. Health remaining: {health - damage}");
                yield return popup.GetComponent<DamagePopup>().RegularPopup(damage, transform.position, destroyTime);
            }
            Destroy(popup);
        }

        private bool IsCriticalDamage(float damage, float health)
        {
            if (damage > health / 2)
            {
                return true;
            }
            return false;
        }

        public IEnumerator Charge(Action onComplete = null)
        {
            float animationDuration = _animatorController.SetCharacterState(PlayerAnimationState.Charge, false);
            yield return new WaitForSecondsRealtime(animationDuration);
        }

        public IEnumerator DoubleShift(Action onComplete = null)
        {
            float animationDuration = _animatorController.SetCharacterState(PlayerAnimationState.DoubleShift, false);
            yield return new WaitForSecondsRealtime(animationDuration);
        }

        public void Run()
        {
            _animatorController.SetCharacterState(PlayerAnimationState.Run, true);
        }

        public void Die()
        {
            transform.Rotate(new Vector3(0, 0, 90), Space.Self);
        }
    }
}
