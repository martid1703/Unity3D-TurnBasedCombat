using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class UnitPositionResult
    {
        public Dictionary<UnitModel, UnitTransform> UnitTransforms { get; private set; }
        public Rect UnitsRect { get; private set; }

        public UnitPositionResult(Dictionary<UnitModel, UnitTransform> unitTransforms, Rect unitsRect)
        {
            UnitTransforms = unitTransforms ?? throw new ArgumentNullException(nameof(unitTransforms));
            UnitsRect = unitsRect;
        }
    }
}
