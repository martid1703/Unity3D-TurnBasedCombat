using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnfrozenTestWork
{
    public class InGameUI : MonoBehaviour
    {
        [SerializeField]
        public Transform GameStatus;

        [SerializeField]
        public Transform BattleQueue;

        [SerializeField]
        public Button Attack;

        [SerializeField]
        public Button Skip;

        [SerializeField]
        public Button AddPlayer1Units;

        [SerializeField]
        public Button RemovePlayer1Units;

        [SerializeField]
        public Button AddPlayer2Units;

        [SerializeField]
        public Button RemovePlayer2Units;

        [SerializeField]
        public Slider BattleSpeedSlider;

        [SerializeField]
        public Toggle IsHumanPlayer1;

        [SerializeField]
        public Toggle IsHumanPlayer2;

        private TMP_Text _gameStatus;


        private void Awake()
        {
            _gameStatus = GameStatus.GetComponentInChildren<TMP_Text>();
        }

        public void SetGameStatus(string message)
        {
            _gameStatus.text = message;
        }

        public void SwitchToOverviewMode()
        {
            _gameStatus.gameObject.SetActive(true);
            BattleQueue.gameObject.SetActive(true);
            SwitchUIToAutoBattleMode(false);
        }

        public void SwitchToBattleMode()
        {
            _gameStatus.gameObject.SetActive(false);
            BattleQueue.gameObject.SetActive(false);
            SwitchUIToAutoBattleMode(true);
        }

        public void SwitchUIToAutoBattleMode(bool isAutoBattle)
        {
            IsHumanPlayer1.interactable = !isAutoBattle;
            IsHumanPlayer2.interactable = !isAutoBattle;
            Attack.interactable = !isAutoBattle;
            Skip.interactable = !isAutoBattle;
            AddPlayer1Units.interactable = !isAutoBattle;
            RemovePlayer1Units.interactable = !isAutoBattle;
            AddPlayer2Units.interactable = !isAutoBattle;
            RemovePlayer2Units.interactable = !isAutoBattle;
        }

        private void OnDisable()
        {
            Attack.onClick.RemoveAllListeners();
            Skip.onClick.RemoveAllListeners();
            AddPlayer1Units.onClick.RemoveAllListeners();
            RemovePlayer1Units.onClick.RemoveAllListeners();
            AddPlayer2Units.onClick.RemoveAllListeners();
            RemovePlayer2Units.onClick.RemoveAllListeners();
            BattleSpeedSlider.onValueChanged.RemoveAllListeners();
            IsHumanPlayer1.onValueChanged.RemoveAllListeners();
            IsHumanPlayer2.onValueChanged.RemoveAllListeners();
        }
    }
}
