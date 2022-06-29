using System;

namespace UnfrozenTestWork
{
    public interface IUnitAnimatorController
    {
        void SetAnimationSpeed(float speed);
        float SetCharacterState(PlayerAnimationState playerState, bool loop, Action onAnimationEnd = null);
    }
}