namespace UnfrozenTestWork
{
    public class UnitSpawnerResult
    {
        public Unit[] PlayerUnits { get; private set; }
        public Unit[] EnemyUnits { get; private set; }
        public Unit[] NeutralUnits { get; private set; }

        public UnitSpawnerResult(Unit[] playerUnits, Unit[] enemyUnits, Unit[] neutralUnits)
        {
            PlayerUnits = playerUnits ?? throw new System.ArgumentNullException(nameof(playerUnits));
            EnemyUnits = enemyUnits ?? throw new System.ArgumentNullException(nameof(enemyUnits));
            NeutralUnits = neutralUnits ?? throw new System.ArgumentNullException(nameof(neutralUnits));
        }
    }
}
