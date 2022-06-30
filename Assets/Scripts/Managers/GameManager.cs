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

        public IEnumerator Restart(bool isNewGame)
        {
            SetupPlayers(isNewGame);
            _uiManager.HideGameOverUI();
            yield return _battleManager.Restart(isNewGame);
        }

        private void Start()
        {
            DefaultGameSettings = new DefaultGameSettings(_battleManager.Player.IsHuman);
            StartCoroutine(Restart(true));
        }

        private void Update()
        {
            TrackKeyboard();
        }

        private void SetupPlayers(bool isNewGame)
        {
            if (!isNewGame)
            {
                _battleManager.Player.IsHuman = DefaultGameSettings.PlayerIsHuman;
            }
        }

        private void TrackKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!IsGameOnPause())
                {
                    _uiManager.HideInGameUI();
                    _uiManager.ShowGameOverUI("PAUSE...", true);
                    PauseGame(true);
                }
                else
                {
                    _uiManager.HideGameOverUI();
                    _uiManager.ShowInGameUI(false);
                    PauseGame(false);
                }
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                _battleManager.Player.SetState(PlayerTurnState.TakeTurn);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _battleManager.Player.SetState(PlayerTurnState.SkipTurn);
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
