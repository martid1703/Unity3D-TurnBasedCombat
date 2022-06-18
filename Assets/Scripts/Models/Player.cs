using System.Collections;

namespace UnfrozenTestWork
{
    public class Player : PlayerBase
    {
        protected override IEnumerator WaitPlayerDecision()
        {
            var attackedUnits = BattleManager.EnemyUnits.ToArray();
            while (true)
            {
                var _attackedUnit = BattleManager.GetAttackedUnit();

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
    }
}