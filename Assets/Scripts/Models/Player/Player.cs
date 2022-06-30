using System.Collections;

namespace UnfrozenTestWork
{
    public class Player : PlayerBase
    {
        public new IEnumerator TakeTurn()
        {
            _attackingUnit = BattleManager.AttackingUnit;
            UnitManager.DeselectUnitsExceptOne(BattleManager.PlayerUnits, _attackingUnit);
            yield return base.TakeTurn();
        }
    }
}