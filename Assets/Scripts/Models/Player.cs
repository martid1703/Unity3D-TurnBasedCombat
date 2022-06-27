using System.Collections;

namespace UnfrozenTestWork
{
    public class Player : PlayerBase
    {
        public new IEnumerator TakeTurn()
        {
            _attackingUnit = BattleManager.AttackingUnit;
            UnitSelector.DeselectUnitsExceptOne(BattleManager.PlayerUnits.ToArray(), _attackingUnit);
            yield return base.TakeTurn();
        }
    }
}