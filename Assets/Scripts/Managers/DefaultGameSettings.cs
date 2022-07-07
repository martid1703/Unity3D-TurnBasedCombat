namespace UnfrozenTestWork
{
    public class DefaultGameSettings
    {
        public bool Player1IsHuman { get; private set; }
        public bool Player2IsHuman { get; private set; }
        public float DefaultBattleSpeed { get; private set; }
        public bool DefaultAutoBattle { get; private set; }

        public DefaultGameSettings(bool player1IsHuman, bool player2IsHuman) : this(player1IsHuman, player2IsHuman, 2f, false)
        {
        }

        public DefaultGameSettings(bool player1IsHuman, bool player2IsHuman, float defaultBattleSpeed, bool defaultAutoBattle)
        {
            Player1IsHuman = player1IsHuman;
            Player2IsHuman = player2IsHuman;
            DefaultBattleSpeed = defaultBattleSpeed;
            DefaultAutoBattle = defaultAutoBattle;
        }
    }
}