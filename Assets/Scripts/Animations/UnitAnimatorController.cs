using System;
using Spine.Unity;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class UnitAnimatorController : MonoBehaviour, IUnitAnimatorController
    {
        [SerializeField]
        private SkeletonAnimation _skeletonAnimation;

        [SerializeField]
        private AnimationReferenceAsset _idle, _takeDamage, _attack, _run, _die;

        [SerializeField]
        private float _animationSpeed = 1f;

        private void Start()
        {
            SetCharacterState(PlayerAnimationState.Idle, true);
        }

        public void SetAnimationSpeed(float speed)
        {
            _animationSpeed = speed;
            var currentTrack = GetCurrentTrackEntry(0);
            if (currentTrack != null)
            {
                currentTrack.timeScale = speed;
            }
        }

        private Spine.TrackEntry GetCurrentTrackEntry(int layerIndex)
        {
            var currentTrackEntry = _skeletonAnimation.AnimationState.GetCurrent(layerIndex);
            return currentTrackEntry;
        }

        public float SetCharacterState(PlayerAnimationState playerState, bool loop, Action onAnimationEnd = null)
        {
            switch (playerState)
            {
                case PlayerAnimationState.Idle:
                    return SetAnimation(_idle, loop, onAnimationEnd);
                case PlayerAnimationState.Attack:
                    return SetAnimation(_attack, loop, onAnimationEnd);
                case PlayerAnimationState.TakeDamage:
                    return SetAnimation(_takeDamage, loop, onAnimationEnd);
                case PlayerAnimationState.Run:
                    return SetAnimation(_run, loop, onAnimationEnd);
                case PlayerAnimationState.Die:
                    return SetAnimation(_die, loop, onAnimationEnd);
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerState));
            }
        }

        private float SetAnimation(AnimationReferenceAsset animationReference, bool loop, Action onAnimationEnd)
        {
            if (animationReference == null)
            {
                return 0;
            }
            Spine.TrackEntry track = _skeletonAnimation.state.SetAnimation(0, animationReference, loop);
            track.timeScale = _animationSpeed;
            if (onAnimationEnd != null)
            {
                track.End += delegate { onAnimationEnd(); };
            }

            return track.AnimationEnd / _animationSpeed;
        }
    }
}
