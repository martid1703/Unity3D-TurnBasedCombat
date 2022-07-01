using System.Collections;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class GameManager : SingletonMonobehaviour<GameManager>
    {
        [SerializeField]
        private BattleManager _battleManager;

        [SerializeField]
        private UIManager _uiManager;

        public DefaultGameSettings DefaultGameSettings { get; private set; }

        public IEnumerator Restart()
        {
            SetupPlayers();
            yield return _battleManager.Restart();
        }

        private void Start()
        {
            DefaultGameSettings = new DefaultGameSettings(_battleManager.Player.IsHuman, _battleManager.Enemy.IsHuman);
            StartCoroutine(Restart());
        }

        private void Update()
        {
            TrackKeyboard();
        }

        public void SetupPlayers()
        {
            if (_battleManager.IsAutoBattle)
            {
                _battleManager.Player.IsHuman = false;
                _battleManager.Enemy.IsHuman = false;
                return;
            }
            _battleManager.Player.IsHuman = DefaultGameSettings.PlayerIsHuman;
            _battleManager.Enemy.IsHuman = DefaultGameSettings.EnemyIsHuman;
        }

        private void TrackKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!IsGameOnPause())
                {
                    _uiManager.ShowGameOverUI("PAUSE...", true);
                    PauseGame(true);
                }
                else
                {
                    _uiManager.ShowInGameUI(false);
                    PauseGame(false);
                }
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                _battleManager.SetPlayerState(PlayerTurnState.TakeTurn);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _battleManager.SetPlayerState(PlayerTurnState.SkipTurn);
            }
        }

        private bool IsGameOnPause()
        {
            return Time.timeScale == 0;
        }

        public void PauseGame(bool pause)
        {
            if (pause)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
    }
}
