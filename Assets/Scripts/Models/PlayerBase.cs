using System;
using System.Collections;
using UnityEngine;

namespace UnfrozenTestWork
{
    public abstract class PlayerBase : MonoBehaviour
    {
        private Unit _attackingUnit;
        private Unit _attackedUnit;

        protected PlayerState State { get; private set; }
        protected BattleManager BattleManager { get; private set; }
        protected UnitSelector UnitSelector { get; private set; }


        private void Awake()
        {
            State = PlayerState.Neutral;
            BattleManager = BattleManager.Instance;
            UnitSelector = new UnitSelector();
            _attackingUnit = BattleManager.AttackingUnit;
            _attackedUnit = BattleManager.AttackedUnit;
        }

        public void SetState(PlayerState state)
        {
            State = state;
        }

        public IEnumerator TakeTurn()
        {
            UnitSelector.DeselectUnits(BattleManager.PlayerUnits.ToArray(), _attackingUnit);

            yield return WaitPlayerDecision();

            switch (State)
            {
                case PlayerState.Attack:
                    yield return PerformTurn();
                    break;
                case PlayerState.Skip:
                    yield return SkipTurn();
                    break;
                case PlayerState.Neutral:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(State));
            }

            SetState(PlayerState.Neutral);
            BattleManager.SetBattleManagerState(BattleManagerState.Free);
        }

        protected abstract IEnumerator WaitPlayerDecision();

        protected IEnumerator PerformTurn()
        {
            BattleManager.SwitchToBattle(_attackingUnit, _attackedUnit);
            yield return _attackingUnit.TakeTurn(_attackedUnit);
        }

        protected IEnumerator SkipTurn()
        {
            Debug.Log($"Player skips his turn.");
            StopCoroutine(PerformTurn());
            BattleManager.SwitchToOverview();
            yield return _attackingUnit.SkipTurn();
        }
    }
}