using UnityEngine;

namespace UnfrozenTestWork
{
    public class UnitTransform
    {
        public Vector3 Position { get; private set; }
        public Vector3 Scale { get; private set; }

        public UnitTransform(Vector3 position, Vector3 scale)
        {
            Position = position;
            Scale = scale;
        }
    }
}
