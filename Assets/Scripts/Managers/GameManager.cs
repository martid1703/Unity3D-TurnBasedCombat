using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnfrozenTestWork
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private BattleManager _battleManager;

        [SerializeField]
        private UIManager _uiManager;

        public DefaultGameSettings DefaultGameSettings { get; private set; }

        private void Start()
        {
            DefaultGameSettings = new DefaultGameSettings(_battleManager.Player.IsHuman, _battleManager.Enemy.IsHuman);
            SetupPlayers(DefaultGameSettings.DefaultAutoBattle);
            StartCoroutine(_battleManager.StartGame());
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            PauseGame(false);
        }

        private void Update()
        {
            TrackKeyboard();
        }

        public void SetupPlayers(bool isAutoBattle)
        {
            if (isAutoBattle)
            {
                _battleManager.Player.IsHuman = false;
                _battleManager.Enemy.IsHuman = false;
                return;
            }
            _battleManager.Player.IsHuman = _uiManager.InGameUI.IsHumanPlayer;
            _battleManager.Enemy.IsHuman = _uiManager.InGameUI.IsHumanEnemy;
        }

        private void TrackKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_battleManager.IsGameOver)
                {
                    return;
                }
                if (!IsGameOnPause())
                {
                    _uiManager.ShowGameOverUI("PAUSE...", true);
                }
                else
                {
                    _uiManager.ShowInGameUI(false);
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
