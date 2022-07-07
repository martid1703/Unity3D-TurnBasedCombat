using System;
using System.Collections.Generic;

namespace UnfrozenTestWork
{
    public class UnitPositionResult
    {
        public Dictionary<UnitModel, UnitTransform> Player1UnitTransforms { get; private set; }
        public Dictionary<UnitModel, UnitTransform> Player2UnitTransforms { get; private set; }

        public UnitPositionResult(Dictionary<UnitModel, UnitTransform> player1UnitTransforms, Dictionary<UnitModel, UnitTransform> player2UnitTransforms)
        {
            Player1UnitTransforms = player1UnitTransforms ?? throw new ArgumentNullException(nameof(player1UnitTransforms));
            Player2UnitTransforms = player2UnitTransforms ?? throw new ArgumentNullException(nameof(player2UnitTransforms));
        }
    }
}
