using System;
using System.Collections.Generic;

namespace UnfrozenTestWork
{
    public class UnitPositionResult
    {
        public Dictionary<UnitModel, UnitTransform> PlayerUnitTransforms { get; private set; }
        public Dictionary<UnitModel, UnitTransform> EnemyUnitTransforms { get; private set; }

        public UnitPositionResult(Dictionary<UnitModel, UnitTransform> playerUnitTransforms, Dictionary<UnitModel, UnitTransform> enemyUnitTransforms)
        {
            PlayerUnitTransforms = playerUnitTransforms ?? throw new ArgumentNullException(nameof(playerUnitTransforms));
            EnemyUnitTransforms = enemyUnitTransforms ?? throw new ArgumentNullException(nameof(enemyUnitTransforms));
        }
    }
}
