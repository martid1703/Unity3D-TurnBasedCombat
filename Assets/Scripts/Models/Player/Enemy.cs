using System.Collections;

namespace UnfrozenTestWork
{
    public class Enemy : PlayerBase
    {
        public new IEnumerator TakeTurn()
        {
            _attackingUnit = BattleManager.AttackingUnit;
            UnitManager.DeselectUnitsExceptOne(BattleManager.EnemyUnits, _attackingUnit);
            yield return base.TakeTurn();
        }
    }
}