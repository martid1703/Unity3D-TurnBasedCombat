using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnfrozenTestWork
{
    public abstract class PlayerBase : MonoBehaviour
    {
        [SerializeField]
        public bool IsHuman;

        protected UnitModel _attackingUnit;
        protected UnitModel _attackedUnit;
        protected PlayerTurnState State { get; private set; }
        protected BattleManager BattleManager { get; private set; }

        private void Awake()
        {
            State = PlayerTurnState.Wait;
            BattleManager = FindObjectOfType<BattleManager>();
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

        private IEnumerator WaitHumanDecision()
        {
            while (true)
            {
                List<UnitModel> attackedUnits;
                if (this is Player)
                {
                    attackedUnits = BattleManager.EnemyUnits;
                }
                else
                {
                    attackedUnits = BattleManager.PlayerUnits;
                }
                if (!IsHuman)//in case we switch auto-mode after player turn begins
                {
                    yield return WaitAIDecision();
                    yield break;
                }

                _attackedUnit = BattleManager.AttackedUnit;
                _attackingUnit = BattleManager.AttackingUnit;

                if (State == PlayerTurnState.TakeTurn && _attackedUnit != null)
                {
                    UnitManager.DeselectUnitsExceptOne(attackedUnits, _attackedUnit);
                    yield break;
                }

                if (State == PlayerTurnState.SkipTurn)
                {
                    UnitManager.DeselectUnits(attackedUnits);
                    yield break;
                }

                yield return null;
            }
        }

        protected virtual IEnumerator WaitAIDecision()
        {
            var msg = $"Waiting AI decision...";
            BattleManager.SetGameStatus(msg);

            List<UnitModel> attackedUnits;

            if (this is Player)
            {
                attackedUnits = BattleManager.EnemyUnits;
            }
            else
            {
                attackedUnits = BattleManager.PlayerUnits;
            }

            _attackedUnit = SelectAttackedUnit(attackedUnits.ToArray());
            _attackingUnit = BattleManager.AttackingUnit;

            var rnd = Random.Range(0, 100);
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
                UnitManager.DeselectUnitsExceptOne(attackedUnits, _attackedUnit);
                yield break;
            }

            if (State == PlayerTurnState.SkipTurn)
            {
                UnitManager.DeselectUnits(attackedUnits);
                yield break;
            }
        }

        private UnitModel SelectAttackedUnit(UnitModel[] units)
        {
            var rnd = Random.Range(0, units.Length);
            var _attackedUnit = units[rnd];
            _attackedUnit.SelectAsTarget();
            return _attackedUnit;
        }

        protected IEnumerator PerformTurn()
        {
            var msg = $"Player takes his turn.";
            BattleManager.SetGameStatus(msg);
            if (!IsHuman)
            {
                yield return new WaitForSeconds(1f);
            }
            yield return BattleManager.SwitchToBattle(_attackingUnit, _attackedUnit);
            yield return _attackingUnit.TakeTurn(_attackedUnit);
        }

        protected IEnumerator SkipTurn()
        {
            var msg = $"Player skips his turn.";
            Debug.Log(msg);
            BattleManager.SetGameStatus(msg);
            if (!IsHuman)
            {
                yield return new WaitForSeconds(1f);
            }
            yield return _attackingUnit.SkipTurn();
        }
    }
}