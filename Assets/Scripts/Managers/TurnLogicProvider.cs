using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class TurnLogicProvider
    {
        private Queue<UnitModel> _battleQueue;
        private UnitModel _attackingUnit;
        private Action<UnitBelonging> _gameOver;
        private Action<BattleManagerState> _updateBattleManagerState;
        private List<UnitModel> _playerUnits;
        private List<UnitModel> _enemyUnits;

        public TurnLogicProvider(
            Action<BattleManagerState> updateBattleManagerState,
            Action<UnitBelonging> gameOver)
        {
            _gameOver = gameOver;
            _updateBattleManagerState = updateBattleManagerState;
        }

        public void CreateBattleQueue(List<UnitModel> playerUnits, List<UnitModel> enemyUnits)
        {
            _playerUnits = playerUnits;
            _enemyUnits = enemyUnits;

            var units = new List<UnitModel>();
            units.AddRange(playerUnits);
            units.AddRange(enemyUnits);
            units.Sort(new UnitComparer());

            var battleQueue = new Queue<UnitModel>(units.Count);
            for (int i = units.Count; i > 0; i--)
            {
                battleQueue.Enqueue(units[i - 1]);
            }

            _battleQueue = battleQueue;
        }

        public UnitModel NextTurn(UnitModel attackedUnit)
        {
            if (attackedUnit != null)
            {
                CheckIsAlive(attackedUnit);
            }

            if (CheckWinCondition(out UnitBelonging winner))
            {
                Debug.Log("Win condition is met!");
                _gameOver(winner);
                return null;
            }

            return GetAttackingUnit();
        }

        private UnitModel GetAttackingUnit()
        {
            // On the first turn of the game attackingUnit is not set yet
            if (_attackingUnit != null)
            {
                _battleQueue.Enqueue(_attackingUnit);
            }

            _attackingUnit = GetNextAttackingUnit();
            _attackingUnit.SelectAsAttacker();
            _updateBattleManagerState(BattleManagerState.Busy);
            return _attackingUnit;
        }

        private bool CheckWinCondition(out UnitBelonging winner)
        {
            winner = UnitBelonging.Player;
            if (_playerUnits.Count == 0 & _enemyUnits.Count == 0)
            {
                return true;
            }

            if (_playerUnits.Count == 0)
            {
                winner = UnitBelonging.Enemy;
                return true;
            }

            if (_enemyUnits.Count == 0)
            {
                winner = UnitBelonging.Player;
                return true;
            }
            return false;
        }

        private void CheckIsAlive(UnitModel unit)
        {
            if (unit.IsAlive)
            {
                return;
            }

            switch (unit.UnitData.Belonging)
            {
                case UnitBelonging.Player:
                    _playerUnits.Remove(unit);
                    break;
                case UnitBelonging.Enemy:
                    _enemyUnits.Remove(unit);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unit.UnitData.Type));
            }
            unit.DestroySelf();
        }

        private UnitModel GetNextAttackingUnit()
        {
            //UnitModel attackingUnit = null;
            while (true)
            {
                if (_battleQueue.Count == 0)
                {
                    throw new InvalidOperationException("Cannot get next attaking unit because battle queue is empty.");
                }

                var attackingUnit = _battleQueue.Dequeue();
                if (!attackingUnit.IsAlive)
                {
                    continue;
                }
                Debug.Log($"Next attacking unit is {attackingUnit}.");
                return attackingUnit;
            }
        }
    }
}