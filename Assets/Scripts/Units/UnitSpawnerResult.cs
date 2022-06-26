namespace UnfrozenTestWork
{
    public class UnitSpawnerResult
    {
        public UnitModel[] PlayerUnits { get; private set; }
        public UnitModel[] EnemyUnits { get; private set; }
        public UnitModel[] NeutralUnits { get; private set; }

        public UnitSpawnerResult(UnitModel[] playerUnits, UnitModel[] enemyUnits, UnitModel[] neutralUnits)
        {
            PlayerUnits = playerUnits ?? throw new System.ArgumentNullException(nameof(playerUnits));
            EnemyUnits = enemyUnits ?? throw new System.ArgumentNullException(nameof(enemyUnits));
            NeutralUnits = neutralUnits ?? throw new System.ArgumentNullException(nameof(neutralUnits));
        }
    }
}
