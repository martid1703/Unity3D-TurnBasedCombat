using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class Enemy : MonoBehaviour
    {
        private Unit[] _playerUnits;
        private Unit[] _enemyUnits;
        Action<BattleState> _ChangeBattleState;
        Unit _attackingUnit;
        Unit _attackedUnit;
        Action<BattleManagerState> _changeBattleManagerState;
        private UnitSelector _unitSelector;

        private void Awake()
        {
            _unitSelector = new UnitSelector();
        }

        public IEnumerator TakeTurn(
            Unit[] playerUnits,
            Unit[] enemyUnits,
            Unit attackingUnit,
            Action<BattleState> changeBattleState,
            Action<BattleManagerState> changeBattleManagerState)
        {
            _playerUnits = playerUnits ?? throw new ArgumentNullException(nameof(playerUnits));
            _enemyUnits = enemyUnits ?? throw new ArgumentNullException(nameof(enemyUnits));
            _attackingUnit = attackingUnit ?? throw new ArgumentNullException(nameof(attackingUnit));
            _ChangeBattleState = changeBattleState ?? throw new ArgumentNullException(nameof(changeBattleState));
            _changeBattleManagerState = changeBattleManagerState ?? throw new ArgumentNullException(nameof(changeBattleManagerState));

            _unitSelector.DeselectUnits(_enemyUnits.ToArray(), exceptSelected: _attackingUnit);
            yield return EnemyTakeTurn();
            _changeBattleManagerState(BattleManagerState.Free);
        }

        private IEnumerator EnemyTakeTurn()
        {
            yield return SelectAttackedUnit(_playerUnits);
            _ChangeBattleState(BattleState.Battle);
            _unitSelector.DeselectUnits(_enemyUnits.ToArray(), exceptSelected: _attackedUnit);
            yield return _attackingUnit.TakeTurn(_attackedUnit);
        }

        private IEnumerator SelectAttackedUnit(Unit[] units)
        {
            var random = new System.Random();
            var rnd = random.Next(0, units.Length);
            _attackedUnit = units[rnd];
            _attackedUnit.SelectAsTarget();
            yield return null;
        }
    }
}