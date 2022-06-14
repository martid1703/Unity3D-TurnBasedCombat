using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class TurnLogicProvider
    {
        private Queue<Unit> _battleQueue;
        private List<Unit> _playerUnits;
        private List<Unit> _enemyUnits;
        private Unit _attackingUnit;
        private Action<UnitType> _gameOver;
        private Action<BattleManagerState> _updateBattleManagerState;

        public TurnLogicProvider(
            Unit attackingUnit,
            List<Unit> playerUnits,
            List<Unit> enemyUnits,
            Action<BattleManagerState> updateBattleManagerState,
            Action<UnitType> gameOver)
        {
            _attackingUnit = attackingUnit;
            _playerUnits = playerUnits;
            _enemyUnits = enemyUnits;
            _gameOver = gameOver;
            _updateBattleManagerState = updateBattleManagerState;
        }

        public void CreateBattleQueue()
        {
            var units = new List<Unit>();
            units.AddRange(_playerUnits);
            units.AddRange(_enemyUnits);
            units.Sort(new UnitComparer());

            var battleQueue = new Queue<Unit>(units.Count);
            for (int i = units.Count; i > 0; i--)
            {
                battleQueue.Enqueue(units[i - 1]);
            }

            _battleQueue = battleQueue;
        }

        public Unit NextTurn(Unit attackedUnit)
        {
            CheckIsAlive(attackedUnit);

            if (CheckWinCondition(out UnitType winner))
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

        private bool CheckWinCondition(out UnitType winner)
        {
            winner = UnitType.Neutral;
            if (_playerUnits.Count == 0 & _enemyUnits.Count == 0)
            {
                return true;
            }

            if (_playerUnits.Count == 0)
            {
                winner = UnitType.Enemy;
                return true;
            }

            if (_enemyUnits.Count == 0)
            {
                winner = UnitType.Player;
                return true;
            }
            return false;
        }

        private void CheckIsAlive(Unit unit)
        {
            if (unit == null)
            {
                return;
            }

            if (unit.IsAlive)
            {
                return;
            }

            switch (unit.UnitData.Type)
            {
                case UnitType.Player:
                    _playerUnits.Remove(unit);
                    break;
                case UnitType.Enemy:
                    _enemyUnits.Remove(unit);
                    break;
                case UnitType.Neutral:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unit.UnitData.Type));
            }
            //unit.PlayDeadAnimation();
            unit.DestroySelf();
        }

        private Unit GetNextAttackingUnit()
        {
            Unit attackingUnit = null;
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