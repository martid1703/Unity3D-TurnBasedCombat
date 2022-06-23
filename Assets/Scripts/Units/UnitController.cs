using System;
using System.Collections;
using Spine;
using Spine.Unity;
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

        [Space]
        public SkeletonAnimation _skeletonAnimation;

        [SpineEvent(dataField: "skeletonAnimation", fallbackToTextField: true)]
        public string footstepEvent;

        [SpineEvent(dataField: "skeletonAnimation", fallbackToTextField: true)]
        public string attackEvent;

        [SpineEvent(dataField: "skeletonAnimation", fallbackToTextField: true)]
        public string takeDamageEvent;

        [Space]
        [SerializeField]
        private AudioSource _stepAudio;

        [SerializeField]
        private AudioSource _attackAudio;

        [SerializeField]
        private AudioSource _damageAudio;

        private Spine.EventData _footstepEventData;
        private Spine.EventData _attackEventData;
        private Spine.EventData _takeDamageEventData;

        private void Awake()
        {
            _animatorController = GetComponent<UnitAnimatorController>();
            _healthBar = GetComponentInChildren<HealthBar>();
        }

        private void Start()
        {
            _footstepEventData = _skeletonAnimation.Skeleton.Data.FindEvent(footstepEvent);
            _attackEventData = _skeletonAnimation.Skeleton.Data.FindEvent(attackEvent);
            _takeDamageEventData = _skeletonAnimation.Skeleton.Data.FindEvent(takeDamageEvent);

            _skeletonAnimation.AnimationState.Event += HandleAnimationStateEvent;
        }

        private void HandleAnimationStateEvent(TrackEntry trackEntry, Spine.Event e)
        {
            if (_footstepEventData == e.Data)
            {
                _stepAudio.PlayOneShot(_stepAudio.clip);
            }
            if (_attackEventData == e.Data)
            {
                _attackAudio.PlayOneShot(_attackAudio.clip);
            }
            if (_takeDamageEventData == e.Data)
            {
                _damageAudio.PlayOneShot(_damageAudio.clip);
            }
        }

        public void SetBattleSpeed(float speed)
        {
            _animatorController.SetAnimationSpeed(speed);
            _battleSpeed = speed;
        }

        public void Idle()
        {
            _animatorController.SetCharacterState(PlayerAnimationState.Idle, true);
        }

        public IEnumerator Attack(Action onComplete = null)
        {
            float animationDuration = _animatorController.SetCharacterState(PlayerAnimationState.Attack, false);
            _attackAudio.PlayOneShot(_attackAudio.clip);
            yield return new WaitForSecondsRealtime(animationDuration);
        }

        public IEnumerator TakeDamage(float damage, UnitData unitData, Action onComplete = null)
        {
            float animationDuration = _animatorController.SetCharacterState(PlayerAnimationState.TakeDamage, false);
            _damageAudio.PlayOneShot(_damageAudio.clip);
            StartCoroutine(ShowDamagePopup(damage, unitData.Health, animationDuration));

            _healthBar.SetReduceHPSpeed(BattleSpeedConverter.GetHPReduceSpeed(_battleSpeed));
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

        public IEnumerator Run()
        {
            float duration = _animatorController.SetCharacterState(PlayerAnimationState.Run, true);
            yield return null;
        }

        public void Die()
        {
            float animationDuration = _animatorController.SetCharacterState(PlayerAnimationState.Die, false);
        }
    }
}
