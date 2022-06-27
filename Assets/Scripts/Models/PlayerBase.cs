using System;
using System.Collections;
using UnityEngine;
using Random = System.Random;

namespace UnfrozenTestWork
{
    public abstract class PlayerBase : MonoBehaviour
    {
        protected UnitModel _attackingUnit;
        protected UnitModel _attackedUnit;
        protected PlayerTurnState State { get; private set; }
        protected BattleManager BattleManager { get; private set; }
        protected UnitSelector UnitSelector { get; private set; }
        public bool IsHuman { get; set; }

        private Random _rnd;


        private void Awake()
        {
            State = PlayerTurnState.Wait;
            BattleManager = BattleManager.Instance;
            UnitSelector = new UnitSelector();
            _rnd = new Random();
        }

        public void SetState(PlayerTurnState state)
        {
            State = state;
        }

        public IEnumerator TakeTurn()
        {
            var msg = $"Waiting player decision...";
            BattleManager.SetGameStatus(msg);
            if (IsHuman)
            {
                yield return WaitHumanDecision();
            }
            else
            {
                yield return WaitAIDecision();
            }

            switch (State)
            {
                case PlayerTurnState.TakeTurn:
                    yield return PerformTurn();
                    yield return BattleManager.ReturnUnitsBack();
                    break;
                case PlayerTurnState.SkipTurn:
                    yield return SkipTurn();
                    break;
                case PlayerTurnState.Wait:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(State));
            }

            SetState(PlayerTurnState.Wait);
            BattleManager.SetBattleManagerState(BattleManagerState.Free);
        }

        protected virtual IEnumerator WaitAIDecision()
        {
            var msg = $"Waiting AI decision...";
            BattleManager.SetGameStatus(msg);
            yield return new WaitForSeconds(0.5f);

            UnitModel[] attackedUnits;

            if (this is Player)
            {
                attackedUnits = BattleManager.EnemyUnits.ToArray();
            }
            else
            {
                attackedUnits = BattleManager.PlayerUnits.ToArray();
            }

            _attackedUnit = SelectAttackedUnit(attackedUnits);

            var rnd = _rnd.Next(0, 100);
            if (rnd < 20)
            {
                SetState(PlayerTurnState.SkipTurn);
            }
            else
            {
                SetState(PlayerTurnState.TakeTurn);
            }

            if (State == PlayerTurnState.TakeTurn && _attackedUnit != null)
            {
                UnitSelector.DeselectUnitsExceptOne(attackedUnits, _attackedUnit);
                yield break;
            }

            if (State == PlayerTurnState.SkipTurn)
            {
                UnitSelector.DeselectUnits(attackedUnits);
                yield break;
            }
        }

        private UnitModel SelectAttackedUnit(UnitModel[] units)
        {
            var random = new Random();
            var rnd = random.Next(0, units.Length);
            var _attackedUnit = units[rnd];
            _attackedUnit.SelectAsTarget();
            return _attackedUnit;
        }

        private IEnumerator WaitHumanDecision()
        {
            var attackedUnits = BattleManager.EnemyUnits.ToArray();
            while (true)
            {
                if (!IsHuman)//in case we switch auto-mode
                {
                    yield return WaitAIDecision();
                    yield break;
                }

                _attackedUnit = BattleManager.GetAttackedUnit();

                if (State == PlayerTurnState.TakeTurn && _attackedUnit != null && _attackedUnit.UnitData.Belonging != UnitBelonging.Player)
                {
                    UnitSelector.DeselectUnitsExceptOne(attackedUnits, _attackedUnit);
                    yield break;
                }

                if (State == PlayerTurnState.SkipTurn)
                {
                    UnitSelector.DeselectUnits(attackedUnits);
                    yield break;
                }

                yield return null;
            }
        }

        protected IEnumerator PerformTurn()
        {
            var msg = $"Player takes his turn.";
            BattleManager.SetGameStatus(msg);
            yield return BattleManager.SwitchToBattle(_attackingUnit, _attackedUnit);
            yield return _attackingUnit.TakeTurn(_attackedUnit);
        }

        protected IEnumerator SkipTurn()
        {
            var msg = $"Player skips his turn.";
            Debug.Log(msg);
            BattleManager.SetGameStatus(msg);
            yield return _attackingUnit.SkipTurn();
        }
    }
}