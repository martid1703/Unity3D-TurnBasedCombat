using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class Player: MonoBehaviour
    {
        public PlayerState State { get; set; }
        private Unit[] _playerUnits;
        private Unit[] _enemyUnits;
        Action<BattleState> _changeBattleState;
        Unit _attackingUnit;
        private Func<Unit> _getAttackedUnit;
        Unit _attackedUnit;
        Action<BattleManagerState> _changeBattleManagerState;
        private UnitSelector _unitSelector;

        private void Awake()
        {
            State = PlayerState.Neutral;
            _unitSelector = new UnitSelector();
        }

        public IEnumerator TakeTurn(
            Unit[] playerUnits,
            Unit[] enemyUnits,
            Unit attackingUnit,
            Func<Unit> getAttackedUnit,
            Action<BattleState> changeBattleState,
            Action<BattleManagerState> changeBattleManagerState)
        {
            _playerUnits = playerUnits ?? throw new ArgumentNullException(nameof(playerUnits));
            _enemyUnits = enemyUnits ?? throw new ArgumentNullException(nameof(enemyUnits));
            _attackingUnit = attackingUnit ?? throw new ArgumentNullException(nameof(attackingUnit));
            _getAttackedUnit = getAttackedUnit ?? throw new ArgumentNullException(nameof(getAttackedUnit));
            _changeBattleState = changeBattleState ?? throw new ArgumentNullException(nameof(changeBattleState));
            _changeBattleManagerState = changeBattleManagerState ?? throw new ArgumentNullException(nameof(changeBattleManagerState));

            _unitSelector.DeselectUnits(_playerUnits, exceptSelected: attackingUnit);

            yield return WaitPlayerDecision();

            switch (State)
            {
                case PlayerState.Attack:
                    yield return PlayerAttack();
                    break;
                case PlayerState.Skip:
                    yield return PlayerSkip();
                    break;
                case PlayerState.Neutral:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(State));
            }

            State = PlayerState.Neutral;
            _changeBattleManagerState(BattleManagerState.Free);
        }

        private IEnumerator WaitPlayerDecision()
        {
            while (true)
            {

                _attackedUnit = _getAttackedUnit();

                if (State == PlayerState.Attack && _attackedUnit != null && _attackedUnit.UnitData.Type != UnitType.Player)
                {
                    _unitSelector.DeselectUnits(_enemyUnits.ToArray(), _attackedUnit);
                    yield break;
                }

                if (State == PlayerState.Skip)
                {
                    _unitSelector.DeselectUnits(_enemyUnits.ToArray());
                    yield break;
                }

                yield return null;
            }
        }

        private IEnumerator PlayerAttack()
        {
            _changeBattleState(BattleState.Battle);
            yield return _attackingUnit.TakeTurn(_attackedUnit);
        }

        private IEnumerator PlayerSkip()
        {
            Debug.Log($"Player skips his turn.");
            StopCoroutine(PlayerAttack());
            _changeBattleState(BattleState.Overview);
            yield return _attackingUnit.SkipTurn();
        }
    }
}