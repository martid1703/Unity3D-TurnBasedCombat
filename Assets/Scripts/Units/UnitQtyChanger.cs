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

        public void Increment(UnitBelonging unitBelonging)
        {
            
            UnitType unitType;
            switch (unitBelonging)
            {
                case UnitBelonging.Player:
                    unitType = _playerUnits.Last().UnitData.Type;
                    _unitSpawner.AddUnit(unitBelonging, unitType, _playerUnits);
                    break;
                case UnitBelonging.Enemy:
                    unitType = _enemyUnits.Last().UnitData.Type;
                    _unitSpawner.AddUnit(unitBelonging, unitType, _enemyUnits);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitBelonging));
            }
        }

        public void Decrement(UnitBelonging unitBelonging)
        {
            UnitModel unit;
            switch (unitBelonging)
            {
                case UnitBelonging.Player:
                    if (_playerUnits.Count == 1)
                    {
                        return;
                    }
                    unit = GetUnitToRemove(_playerUnits);
                    _playerUnits.Remove(unit);
                    break;
                case UnitBelonging.Enemy:
                    if (_enemyUnits.Count == 1)
                    {
                        return;
                    }
                    unit = GetUnitToRemove(_enemyUnits);
                    _enemyUnits.Remove(unit);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitBelonging));
            }

            unit.DestroySelf();
        }

        private UnitModel GetUnitToRemove(IEnumerable<UnitModel> units)
        {
            foreach (var unit in units)
            {
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
