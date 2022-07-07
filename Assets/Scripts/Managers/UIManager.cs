using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnfrozenTestWork
{
    [RequireComponent(typeof(SceneFader))]
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private GameManager _gameManager;

        [SerializeField]
        private BattleManager _battleManager;

        [SerializeField]
        public InGameUI InGameUI;

        [SerializeField]
        public GameOverUI GameOverUI;

        [SerializeField]
        public Image LoadingScreen;

        [SerializeField]
        private Transform _blur;

        [SerializeField]
        private Transform _fade;

        [SerializeField]
        private Texture2D _handCursor;

        [SerializeField]
        private Texture2D _swordCursor;
        private CursorMode _cursorMode = CursorMode.ForceSoftware;
        private Vector2 _cursorHotSpot = Vector2.zero;
        private SceneFader _sceneFader;

        private void Awake()
        {
            _sceneFader = GetComponent<SceneFader>();
        }

        public void DisableUI()
        {
            GameOverUI.gameObject.SetActive(false);
            InGameUI.gameObject.SetActive(false);
        }

        public IEnumerator FadeScreen(bool fadeIn)
        {
            if (fadeIn)
            {
                yield return _sceneFader.FadeIn(LoadingScreen);
                LoadingScreen.gameObject.SetActive(false);
            }
            else
            {
                LoadingScreen.gameObject.SetActive(true);
                yield return _sceneFader.FadeOut(LoadingScreen);
            }
        }

        public void AddBackgroundBattleEffects()
        {
            _blur.gameObject.SetActive(true);
            _fade.gameObject.SetActive(true);
        }

        public void RemoveBackgroundBattleEffects()
        {
            _blur.gameObject.SetActive(false);
            _fade.gameObject.SetActive(false);
        }

        public void SetGameStatus(string message)
        {
            InGameUI.SetGameStatus(message);
        }

        public void SetAttackCursor()
        {
            Cursor.SetCursor(_swordCursor, _cursorHotSpot, _cursorMode);
        }

        public void SetRegularCursor()
        {
            Cursor.SetCursor(_handCursor, _cursorHotSpot, _cursorMode);
        }

        public void SwitchToOverviewMode(bool isNewGame)
        {
            ShowInGameUI(isNewGame);
            InGameUI.SwitchToOverviewMode();
        }

        public void SwitchToBattleMode()
        {
            SetRegularCursor();
            InGameUI.SwitchToBattleMode();
        }

        public void ShowGameOverUI(string msg, bool allowRestart)
        {
            _gameManager.PauseGame(true);
            var msgbox = GameOverUI.transform.Find("Message");
            var tmp = msgbox.GetComponent<TMP_Text>();
            tmp.text = msg;
            ShowGameOverUI(allowRestart);
        }

        private void ShowGameOverUI(bool allowRestart)
        {
            HideInGameUI();
            SetupGameOverUI();
            GameOverUI.gameObject.SetActive(true);
            if (!allowRestart)
            {
                GameOverUI.Restart.interactable = false;
            }
        }

        public void HideGameOverUI()
        {
            _gameManager.PauseGame(false);
            GameOverUI.gameObject.SetActive(false);
        }

        public void ShowInGameUI(bool isNewGame)
        {
            HideGameOverUI();
            if (InGameUI.gameObject.activeSelf == false)
            {
                SetupInGameUI(isNewGame);
                InGameUI.gameObject.SetActive(true);
            }
        }

        public void HideInGameUI()
        {
            InGameUI.gameObject.SetActive(false);
        }

        private void SetupGameOverUI()
        {
            GameOverUI.Quit.onClick.AddListener(() =>
            {
                _battleManager.Quit();
            });

            GameOverUI.Restart.onClick.AddListener(() =>
            {
                _gameManager.Restart();
            });
        }

        private void SetupInGameUI(bool isNewGame)
        {
            InGameUI.Attack.onClick.AddListener(() =>
            {
                _battleManager.SetPlayerState(PlayerTurnState.TakeTurn);
            });

            InGameUI.Skip.onClick.AddListener(() =>
            {
                _battleManager.SetPlayerState(PlayerTurnState.SkipTurn);
            });

            InGameUI.IsHumanPlayer1.onValueChanged.AddListener((v) =>
            {
                _battleManager.Player1.IsHuman = v;
                bool isAutoBattle = IsAutoBattle();
            });
            if (isNewGame)
            {
                InGameUI.IsHumanPlayer1.isOn = _gameManager.DefaultGameSettings.Player1IsHuman;
            }

            InGameUI.IsHumanPlayer2.onValueChanged.AddListener((v) =>
            {
                _battleManager.Player2.IsHuman = v;
                bool isAutoBattle = IsAutoBattle();
            });
            if (isNewGame)
            {
                InGameUI.IsHumanPlayer2.isOn = _gameManager.DefaultGameSettings.Player2IsHuman;
            }

            InGameUI.BattleSpeedSlider.onValueChanged.AddListener((v) =>
            {
                _battleManager.OnBattleSpeedChange(v);
            });
            if (isNewGame)
            {
                InGameUI.BattleSpeedSlider.value = _gameManager.DefaultGameSettings.DefaultBattleSpeed;
            }

            InGameUI.AddPlayer1Units.onClick.AddListener(() =>
           {
               _battleManager.IncrementUnits(UnitBelonging.Player1);
           });

            InGameUI.RemovePlayer1Units.onClick.AddListener(() =>
           {
               _battleManager.DecrementUnits(UnitBelonging.Player1);
           });

            InGameUI.AddPlayer2Units.onClick.AddListener(() =>
          {
              _battleManager.IncrementUnits(UnitBelonging.Player2);
          });

            InGameUI.RemovePlayer2Units.onClick.AddListener(() =>
           {
               _battleManager.DecrementUnits(UnitBelonging.Player2);
           });
        }

        private bool IsAutoBattle()
        {
            return !_battleManager.Player1.IsHuman & !_battleManager.Player2.IsHuman;
        }
    }
}