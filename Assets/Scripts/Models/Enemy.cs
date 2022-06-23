using System.Collections;

namespace UnfrozenTestWork
{
    public class Enemy : PlayerBase
    {
        public IEnumerator TakeTurn()
        {
            _attackingUnit = BattleManager.AttackingUnit;
            UnitSelector.DeselectUnitsExceptOne(BattleManager.EnemyUnits.ToArray(), _attackingUnit);

            yield return base.TakeTurn();
        }
    }
}