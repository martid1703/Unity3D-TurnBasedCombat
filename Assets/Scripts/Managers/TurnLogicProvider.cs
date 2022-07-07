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
        private List<UnitModel> _player1Units;
        private List<UnitModel> _player2Units;

        public TurnLogicProvider(Action<UnitBelonging> gameOver)
        {
            _gameOver = gameOver ?? throw new ArgumentNullException(nameof(gameOver));
        }

        public Queue<UnitModel> CreateBattleQueue(List<UnitModel> player1Units, List<UnitModel> player2Units)
        {
            _player1Units = player1Units;
            _player2Units = player2Units;

            if (CheckWinCondition(out UnitBelonging winner))
            {
                Debug.Log("Win condition is met!");
                _gameOver(winner);
                return null;
            }

            var units = new List<UnitModel>();
            units.AddRange(player1Units);
            units.AddRange(player2Units);
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
            return _attackingUnit;
        }

        private bool CheckWinCondition(out UnitBelonging winner)
        {
            winner = UnitBelonging.Player1;
            if (_player1Units.Count == 0 & _player2Units.Count == 0)
            {
                return true;
            }

            if (_player1Units.Count == 0)
            {
                winner = UnitBelonging.Player2;
                return true;
            }

            if (_player2Units.Count == 0)
            {
                winner = UnitBelonging.Player1;
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