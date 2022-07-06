using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class TurnLogicProvider
    {
        public Queue<UnitModel> _battleQueue;
        private UnitModel _attackingUnit;
        private Action<UnitBelonging> _gameOver;
        private Action<BattleManagerState> _updateBattleManagerState;
        private List<UnitModel> _playerUnits;
        private List<UnitModel> _enemyUnits;
        private BattleQueuePresenter _battleQueuePresenter;

        public TurnLogicProvider(
            Action<BattleManagerState> updateBattleManagerState,
            Action<UnitBelonging> gameOver,
            BattleQueuePresenter battleQueuePresenter)
        {
            _gameOver = gameOver ?? throw new ArgumentNullException(nameof(gameOver));
            _updateBattleManagerState = updateBattleManagerState ?? throw new ArgumentNullException(nameof(updateBattleManagerState));
            _battleQueuePresenter = battleQueuePresenter ?? throw new ArgumentNullException(nameof(battleQueuePresenter));
        }

        public Queue<UnitModel> CreateBattleQueue(List<UnitModel> playerUnits, List<UnitModel> enemyUnits)
        {
            _playerUnits = playerUnits;
            _enemyUnits = enemyUnits;

            if (CheckWinCondition(out UnitBelonging winner))
            {
                Debug.Log("Win condition is met!");
                _gameOver(winner);
                return null;
            }

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
            return _battleQueue;
        }

        public void AddToBattleQueue(UnitModel unit)
        {
            _battleQueue.Enqueue(unit);
        }

        public UnitModel NextTurn()
        {
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

        private UnitModel GetNextAttackingUnit()
        {
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