namespace UnfrozenTestWork
{
    public class DefaultGameSettings
    {
        public bool PlayerIsHuman { get; private set; }
        public float DefaultBattleSpeed { get; private set; }

        public DefaultGameSettings(bool playerIsHuman)
        {
            PlayerIsHuman = playerIsHuman;
            DefaultBattleSpeed = 2f;
        }

        public DefaultGameSettings(bool playerIsHuman, float defaultBattleSpeed)
        {
            PlayerIsHuman = playerIsHuman;
            DefaultBattleSpeed = defaultBattleSpeed;
        }
    }
}