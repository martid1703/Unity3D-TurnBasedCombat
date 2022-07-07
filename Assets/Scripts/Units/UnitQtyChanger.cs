using System;
using System.Collections.Generic;
using System.Linq;

namespace UnfrozenTestWork
{
    public class UnitQtyChanger
    {
        private readonly UnitSpawner _unitSpawner;
        private List<UnitModel> _player1Units;
        private List<UnitModel> _player2Units;

        public UnitQtyChanger(
            UnitSpawner unitSpawner,
            List<UnitModel> player1Units,
            List<UnitModel> player2Units)
        {
            _unitSpawner = unitSpawner;
            _player1Units = player1Units;
            _player2Units = player2Units;
        }

        public UnitModel Increment(UnitBelonging unitBelonging)
        {
            UnitModel unit;
            UnitType unitType;
            switch (unitBelonging)
            {
                case UnitBelonging.Player1:
                    unitType = _player1Units.Last().UnitData.Type;
                    unit = _unitSpawner.AddUnit(unitBelonging, unitType, _player1Units);
                    break;
                case UnitBelonging.Player2:
                    unitType = _player2Units.Last().UnitData.Type;
                    unit = _unitSpawner.AddUnit(unitBelonging, unitType, _player2Units);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitBelonging));
            }
            return unit;
        }

        public UnitModel Decrement(UnitBelonging unitBelonging)
        {
            UnitModel unit;
            switch (unitBelonging)
            {
                case UnitBelonging.Player1:
                    if (_player1Units.Count == 1)
                    {
                        return null;
                    }
                    unit = GetUnitToRemove(_player1Units.ToArray());
                    _player1Units.Remove(unit);
                    break;
                case UnitBelonging.Player2:
                    if (_player2Units.Count == 1)
                    {
                        return null;
                    }
                    unit = GetUnitToRemove(_player2Units.ToArray());
                    _player2Units.Remove(unit);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitBelonging));
            }

            return unit;
        }

        private UnitModel GetUnitToRemove(UnitModel[] units)
        {
            for (int i = units.Length - 1; i >= 0; i--)
            {
                var unit = units[i];
                if (unit.IsSelectedAsTarget || unit.IsSelectedAsAttacker)
                {
                    continue;
                }
                return unit;
            }
            throw new InvalidOperationException("Cannot find unit to remove");
        }
    }
}
