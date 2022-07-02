namespace UnfrozenTestWork
{
    public class DefaultGameSettings
    {
        public bool PlayerIsHuman { get; private set; }
        public bool EnemyIsHuman { get; private set; }
        public float DefaultBattleSpeed { get; private set; }
        public bool DefaultAutoBattle { get; private set; }

        public DefaultGameSettings(bool playerIsHuman, bool enemyIsHuman) : this(playerIsHuman, enemyIsHuman, 2f, false)
        {
        }

        public DefaultGameSettings(bool playerIsHuman, bool enemyIsHuman, float defaultBattleSpeed, bool defaultAutoBattle)
        {
            PlayerIsHuman = playerIsHuman;
            EnemyIsHuman = enemyIsHuman;
            DefaultBattleSpeed = defaultBattleSpeed;
            DefaultAutoBattle = defaultAutoBattle;
        }
    }
}