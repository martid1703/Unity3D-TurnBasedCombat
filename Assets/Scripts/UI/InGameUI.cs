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
        public Button Attack;

        [SerializeField]
        public Button Skip;

        [SerializeField]
        public Button AutoBattle;

        [SerializeField]
        public Button AddPlayerUnits;

        [SerializeField]
        public Button RemovePlayerUnits;

        [SerializeField]
        public Button AddEnemyUnits;

        [SerializeField]
        public Button RemoveEnemyUnits;

        [SerializeField]
        public Slider BattleSpeedSlider;
        private TMP_Text _gameStatus;

        private void Awake()
        {
            _gameStatus = GameStatus.GetComponentInChildren<TMP_Text>();
        }

        public void SetGameStatus(string message)
        {
            _gameStatus.text = message;
        }

        public void SwitchToOverviewMode(bool isAutoBattle)
        {
            _gameStatus.gameObject.SetActive(true);
            SwitchUIToAutoBattleMode(isAutoBattle);
        }

        public void SwitchToBattleMode()
        {
            _gameStatus.gameObject.SetActive(false);
            SwitchUIToAutoBattleMode(true);
        }

        private void SwitchUIToAutoBattleMode(bool isAutoBattle)
        {
            Attack.interactable = !isAutoBattle;
            Skip.interactable = !isAutoBattle;
            AddPlayerUnits.interactable = !isAutoBattle;
            RemovePlayerUnits.interactable = !isAutoBattle;
            AddEnemyUnits.interactable = !isAutoBattle;
            RemoveEnemyUnits.interactable = !isAutoBattle;
        }

        private void OnDisable()
        {
            Attack.onClick.RemoveAllListeners();
            Skip.onClick.RemoveAllListeners();
            AutoBattle.onClick.RemoveAllListeners();
            AddPlayerUnits.onClick.RemoveAllListeners();
            RemovePlayerUnits.onClick.RemoveAllListeners();
            AddEnemyUnits.onClick.RemoveAllListeners();
            RemoveEnemyUnits.onClick.RemoveAllListeners();
            BattleSpeedSlider.onValueChanged.RemoveAllListeners();
        }
    }
}
