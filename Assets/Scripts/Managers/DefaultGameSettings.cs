namespace UnfrozenTestWork
{
    public class DefaultGameSettings
    {
        public bool PlayerIsHuman { get; private set; }
        public bool EnemyIsHuman { get; private set; }
        public float DefaultBattleSpeed { get; private set; }

        public DefaultGameSettings(bool playerIsHuman, bool enemyIsHuman) : this(playerIsHuman, enemyIsHuman, 2f)
        {
        }

        public DefaultGameSettings(bool playerIsHuman, bool enemyIsHuman, float defaultBattleSpeed)
        {
            PlayerIsHuman = playerIsHuman;
            EnemyIsHuman = enemyIsHuman;
            DefaultBattleSpeed = defaultBattleSpeed;
        }
    }
}