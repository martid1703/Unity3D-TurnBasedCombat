using System;
using Spine.Unity;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class UnitAnimatorController : MonoBehaviour
    {
        [SerializeField]
        private SkeletonAnimation _skeletonAnimation;

        [SerializeField]
        private AnimationReferenceAsset _idle, _takeDamage, _doubleShift, _attack, _charge, _run;

        [SerializeField]
        private float _animationSpeed = 1f;


        private void Start()
        {
            SetCharacterState(PlayerAnimationState.Idle, true);
        }

        public float SetCharacterState(PlayerAnimationState playerState, bool loop, Action onAnimationEnd = null)
        {
            switch (playerState)
            {
                case PlayerAnimationState.Idle:
                    return SetAnimation(_idle, loop, _animationSpeed, onAnimationEnd);
                case PlayerAnimationState.Attack:
                    return SetAnimation(_attack, loop, _animationSpeed, onAnimationEnd);
                case PlayerAnimationState.DoubleShift:
                    return SetAnimation(_doubleShift, loop, _animationSpeed, onAnimationEnd);
                case PlayerAnimationState.TakeDamage:
                    return SetAnimation(_takeDamage, loop, _animationSpeed, onAnimationEnd);
                case PlayerAnimationState.Charge:
                    return SetAnimation(_charge, loop, _animationSpeed, onAnimationEnd);
                case PlayerAnimationState.Run:
                    return SetAnimation(_run, loop, _animationSpeed, onAnimationEnd);
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerState));
            }
        }

        private float SetAnimation(AnimationReferenceAsset animationReference, bool loop, float _animationSpeed, Action onAnimationEnd)
        {
            Spine.TrackEntry track = _skeletonAnimation.state.SetAnimation(0, animationReference, loop);
            track.timeScale = _animationSpeed;
            if (onAnimationEnd != null)
            {
                track.End += delegate { onAnimationEnd(); };
            }

            return track.AnimationEnd;
        }
    }
}
