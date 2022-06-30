using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            if (BattleManager.Instance.BattleState != BattleState.Overview)
            {
                return;
            }

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
                    unit = GetUnitToRemove(_playerUnits.ToArray());
                    _playerUnits.Remove(unit);
                    break;
                case UnitBelonging.Enemy:
                    if (_enemyUnits.Count == 1)
                    {
                        return;
                    }
                    unit = GetUnitToRemove(_enemyUnits.ToArray());
                    _enemyUnits.Remove(unit);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitBelonging));
            }

            unit.DestroySelf();
        }

        private UnitModel GetUnitToRemove(UnitModel[] units)
        {
            for (int i = 0; i < units.Length; i++)
            {
                var unit = units[i];
                if (unit.IsSelectedAsTarget)
                {
                    continue;
                }
                return unit;
            }
            throw new InvalidOperationException("Cannot find unit to remove");
        }
    }
}
