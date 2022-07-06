using TMPro;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        public GameManager _gameManager;

        [SerializeField]
        public BattleManager _battleManager;

        [SerializeField]
        public InGameUI InGameUI;

        [SerializeField]
        public GameOverUI GameOverUI;

        [SerializeField]
        private Texture2D _handCursor;

        [SerializeField]
        private Texture2D _swordCursor;
        private CursorMode _cursorMode = CursorMode.ForceSoftware;
        private Vector2 _cursorHotSpot = Vector2.zero;

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

            InGameUI.AutoBattle.onValueChanged.AddListener((v) =>
            {
                _battleManager.SwitchAutoBattle(v);
                _gameManager.SetupPlayers();
                SwitchToOverviewMode(false);
            });
            if (isNewGame)
            {
                InGameUI.AutoBattle.isOn = _gameManager.DefaultGameSettings.DefaultAutoBattle;
            }

            InGameUI.BattleSpeedSlider.onValueChanged.AddListener((v) =>
            {
                _battleManager.OnBattleSpeedChange(v);
            });
            if (isNewGame)
            {
                InGameUI.BattleSpeedSlider.value = _gameManager.DefaultGameSettings.DefaultBattleSpeed;
            }

            InGameUI.AddPlayerUnits.onClick.AddListener(() =>
           {
               _battleManager.IncrementUnits(UnitBelonging.Player);
           });

            InGameUI.RemovePlayerUnits.onClick.AddListener(() =>
           {
               _battleManager.DecrementUnits(UnitBelonging.Player);
           });

            InGameUI.AddEnemyUnits.onClick.AddListener(() =>
          {
              _battleManager.IncrementUnits(UnitBelonging.Enemy);
          });

            InGameUI.RemoveEnemyUnits.onClick.AddListener(() =>
           {
               _battleManager.DecrementUnits(UnitBelonging.Enemy);
           });
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
            InGameUI.SwitchToOverviewMode(_battleManager.IsAutoBattle);
        }

        public void SwitchToBattleMode()
        {
            SetRegularCursor();
            InGameUI.SwitchToBattleMode();
        }

        public void ShowGameOverUI(string msg, bool allowRestart)
        {
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
            GameOverUI.gameObject.SetActive(false);
        }

        public void ShowInGameUI(bool isNewGame)
        {
            HideGameOverUI();
            if (isNewGame)
            {
                SetupInGameUI(isNewGame);
            }
            InGameUI.gameObject.SetActive(true);
        }

        public void HideInGameUI()
        {
            InGameUI.gameObject.SetActive(false);
        }
    }
}