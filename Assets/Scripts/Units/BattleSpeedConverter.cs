
namespace UnfrozenTestWork
{
    public static class BattleSpeedConverter
    {
        public static float GetAnimationSpeed(float battleSpeed)
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
                return 2f;
            }

            if (battleSpeed <= 4)
            {
                return 3f;
            }

            if (battleSpeed <= 5)
            {
                return 5f;
            }

            return 10f;
        }

        public static float GetUnitMoveSpeed(float battleSpeed)
        {
            if (battleSpeed <= 1)
            {
                return 1f;
            }

            if (battleSpeed <= 2)
            {
                return 5f;
            }

            if (battleSpeed <= 3)
            {
                return 10f;
            }

            if (battleSpeed <= 4)
            {
                return 20f;
            }

            if (battleSpeed <= 5)
            {
                return 30f;
            }

            return 50f;
        }

        public static float GetHPReduceSpeed(float battleSpeed)
        {
            if (battleSpeed <= 1)
            {
                return 10f;
            }

            if (battleSpeed <= 5)
            {
                return 30f;
            }

            return 50f;
        }
    }
}
