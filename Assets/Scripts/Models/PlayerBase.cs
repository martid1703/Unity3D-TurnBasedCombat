using System;
using System.Collections;
using UnityEngine;

namespace UnfrozenTestWork
{
    public abstract class PlayerBase : MonoBehaviour
    {
        protected Unit _attackingUnit;
        protected Unit _attackedUnit;

        protected PlayerState State { get; private set; }
        protected BattleManager BattleManager { get; private set; }
        protected UnitSelector UnitSelector { get; private set; }


        private void Awake()
        {
            State = PlayerState.Neutral;
            BattleManager = BattleManager.Instance;
            UnitSelector = new UnitSelector();
        }

        public void SetState(PlayerState state)
        {
            State = state;
        }

        public IEnumerator TakeTurn()
        {
            _attackingUnit = BattleManager.AttackingUnit;
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
            yield return BattleManager.RestoreUnitPositions();
            BattleManager.SetBattleManagerState(BattleManagerState.Free);
        }

        protected abstract IEnumerator WaitPlayerDecision();

        protected IEnumerator PerformTurn()
        {
            yield return BattleManager.SwitchToBattle(_attackingUnit, _attackedUnit);
            yield return _attackingUnit.TakeTurn(_attackedUnit);
        }

        protected IEnumerator SkipTurn()
        {
            Debug.Log($"Player skips his turn.");
            StopCoroutine(PerformTurn());
            yield return _attackingUnit.SkipTurn();
        }
    }
}