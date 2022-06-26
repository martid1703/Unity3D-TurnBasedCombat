using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class TurnLogicProvider
    {
        private Queue<UnitModel> _battleQueue;
        private List<UnitModel> _playerUnits;
        private List<UnitModel> _enemyUnits;
        private UnitModel _attackingUnit;
        private Action<UnitBelonging> _gameOver;
        private Action<BattleManagerState> _updateBattleManagerState;

        public TurnLogicProvider(
            List<UnitModel> playerUnits,
            List<UnitModel> enemyUnits,
            Action<BattleManagerState> updateBattleManagerState,
            Action<UnitBelonging> gameOver)
        {
            _playerUnits = playerUnits;
            _enemyUnits = enemyUnits;
            _gameOver = gameOver;
            _updateBattleManagerState = updateBattleManagerState;
        }

        public void CreateBattleQueue()
        {
            var units = new List<UnitModel>();
            units.AddRange(_playerUnits);
            units.AddRange(_enemyUnits);
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
            CheckIsAlive(attackedUnit);

            if (CheckWinCondition(out UnitBelonging winner))
            {
                Debug.Log("Win condition is met!");
                _gameOver(winner);
                return null;
            }

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
            if (unit == null)
            {
                return;
            }

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
            UnitModel attackingUnit = null;
            while (attackingUnit == null)
            {
                if (_battleQueue.Count == 0)
                {
                    throw new InvalidOperationException("Cannot get next attaking unit because battle queue is empty.");
                }

                attackingUnit = _battleQueue.Dequeue();
                if (!attackingUnit.IsAlive)
                {
                    attackingUnit = null;
                }
            }

            Debug.Log($"Next attacking unit is {attackingUnit}.");
            return attackingUnit;
        }
    }
}