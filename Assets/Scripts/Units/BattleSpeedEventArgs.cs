using System;

namespace UnfrozenTestWork
{
    public class BattleSpeedEventArgs : EventArgs
    {
        public float Speed { get; private set; }

        public BattleSpeedEventArgs(float speed)
        {
            Speed = speed;
        }
    }
}