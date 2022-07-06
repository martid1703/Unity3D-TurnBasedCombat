using System;
using System.Collections.Generic;
using System.Linq;

namespace UnfrozenTestWork
{
    public class UnitQtyChanger
    {
        private readonly UnitSpawner _unitSpawner;
        private List<UnitModel> _playerUnits;
        private List<UnitModel> _enemyUnits;

        public UnitQtyChanger(
            UnitSpawner unitSpawner,
            List<UnitModel> playerUnits,
            List<UnitModel> enemyUnits)
        {
            _unitSpawner = unitSpawner;
            _playerUnits = playerUnits;
            _enemyUnits = enemyUnits;
        }

        public UnitModel Increment(UnitBelonging unitBelonging)
        {
            UnitModel unit;
            UnitType unitType;
            switch (unitBelonging)
            {
                case UnitBelonging.Player:
                    unitType = _playerUnits.Last().UnitData.Type;
                    unit = _unitSpawner.AddUnit(unitBelonging, unitType, _playerUnits);
                    break;
                case UnitBelonging.Enemy:
                    unitType = _enemyUnits.Last().UnitData.Type;
                    unit = _unitSpawner.AddUnit(unitBelonging, unitType, _enemyUnits);
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
                case UnitBelonging.Player:
                    if (_playerUnits.Count == 1)
                    {
                        return null;
                    }
                    unit = GetUnitToRemove(_playerUnits.ToArray());
                    _playerUnits.Remove(unit);
                    break;
                case UnitBelonging.Enemy:
                    if (_enemyUnits.Count == 1)
                    {
                        return null;
                    }
                    unit = GetUnitToRemove(_enemyUnits.ToArray());
                    _enemyUnits.Remove(unit);
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
