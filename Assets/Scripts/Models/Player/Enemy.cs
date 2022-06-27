using System.Collections;

namespace UnfrozenTestWork
{
    public class Enemy : PlayerBase
    {
        public new IEnumerator TakeTurn()
        {
            _attackingUnit = BattleManager.AttackingUnit;
            UnitSelector.DeselectUnitsExceptOne(BattleManager.EnemyUnits.ToArray(), _attackingUnit);
            yield return base.TakeTurn();
        }
    }
}