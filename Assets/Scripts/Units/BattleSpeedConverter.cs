using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UnfrozenTestWork
{
    public static class BattleSpeedConverter
    {
        public static float GetAnimationSpeed(float battleSpeed)
        {
            if (battleSpeed <= 1)
            {
                return 0.25f;
            }

            if (battleSpeed <= 2)
            {
                return 0.5f;
            }

            if (battleSpeed <= 3)
            {
                return 1f;
            }

            if (battleSpeed <= 4)
            {
                return 2f;
            }

            if (battleSpeed <= 5)
            {
                return 3f;
            }

            return 5f;
        }

        public static float GetUnitMoveSpeed(float battleSpeed)
        {
            if (battleSpeed <= 1)
            {
                return 0.5f;
            }

            if (battleSpeed <= 2)
            {
                return 1f;
            }

            if (battleSpeed <= 3)
            {
                return 3f;
            }

            if (battleSpeed <= 4)
            {
                return 5f;
            }

            if (battleSpeed <= 5)
            {
                return 7f;
            }

            return 10f;
        }
    }
}
