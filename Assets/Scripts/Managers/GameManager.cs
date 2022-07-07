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
            _uiManager.DisableUI();
            DefaultGameSettings = new DefaultGameSettings(_battleManager.Player1.IsHuman, _battleManager.Player2.IsHuman);
            SetupPlayers(DefaultGameSettings.DefaultAutoBattle);
            StartCoroutine(_battleManager.StartGame());
        }

        public void Restart()
        {
            _uiManager.DisableUI();
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
                _battleManager.Player1.IsHuman = false;
                _battleManager.Player2.IsHuman = false;
                return;
            }
            _battleManager.Player1.IsHuman = _uiManager.InGameUI.IsHumanPlayer1;
            _battleManager.Player2.IsHuman = _uiManager.InGameUI.IsHumanPlayer2;
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
            _uiManager.SetRegularCursor();
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
