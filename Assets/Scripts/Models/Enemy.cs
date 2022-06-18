using System.Collections;

namespace UnfrozenTestWork
{
    public class Enemy : PlayerBase
    {
        protected override IEnumerator WaitPlayerDecision()
        {
            Unit[] attackedUnits = BattleManager.PlayerUnits.ToArray();
            var _attackedUnit = SelectAttackedUnit(attackedUnits);

            if (State == PlayerState.Attack && _attackedUnit != null && _attackedUnit.UnitData.Type != UnitType.Enemy)
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
            var random = new System.Random();
            var rnd = random.Next(0, units.Length);
            var _attackedUnit = units[rnd];
            _attackedUnit.SelectAsTarget();
            return _attackedUnit;
        }
    }
}