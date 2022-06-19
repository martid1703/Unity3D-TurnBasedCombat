using System;
using System.Collections;
using UnityEngine;
using Random = System.Random;

namespace UnfrozenTestWork
{
    public abstract class PlayerBase : MonoBehaviour
    {
        protected Unit _attackingUnit;
        protected Unit _attackedUnit;
        protected PlayerState State { get; private set; }
        protected BattleManager BattleManager { get; private set; }
        protected UnitSelector UnitSelector { get; private set; }
        public bool IsHuman { get; set; }

        private Random _rnd;


        private void Awake()
        {
            State = PlayerState.Neutral;
            BattleManager = BattleManager.Instance;
            UnitSelector = new UnitSelector();
            _rnd = new Random();
        }

        public void SetState(PlayerState state)
        {
            State = state;
        }

        public IEnumerator TakeTurn()
        {
            _attackingUnit = BattleManager.AttackingUnit;
            UnitSelector.DeselectUnits(BattleManager.PlayerUnits.ToArray(), _attackingUnit);

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
                case PlayerState.Attack:
                    yield return PerformTurn();
                    yield return BattleManager.RestoreUnitPositions();
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

        protected virtual IEnumerator WaitAIDecision()
        {
            Unit[] attackedUnits;

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
            if (rnd < 30)
            {
                SetState(PlayerState.Skip);
            }
            else
            {
                SetState(PlayerState.Attack);
            }

            if (State == PlayerState.Attack && _attackedUnit != null)
            {
                UnitSelector.DeselectUnits(attackedUnits, _attackedUnit);
                yield break;
            }

            if (State == PlayerState.Skip)
            {
                UnitSelector.DeselectUnits(attackedUnits);
                yield break;
            }
        }

        private Unit SelectAttackedUnit(Unit[] units)
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
                if (!IsHuman)
                {
                    yield return WaitAIDecision();
                    yield break;
                }

                _attackedUnit = BattleManager.GetAttackedUnit();

                if (State == PlayerState.Attack && _attackedUnit != null && _attackedUnit.UnitData.Type != UnitType.Player)
                {
                    UnitSelector.DeselectUnits(attackedUnits, _attackedUnit);
                    yield break;
                }

                if (State == PlayerState.Skip)
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