using UnityEngine;
using UnityEngine.UI;

namespace UnfrozenTestWork
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField]
        public Button Quit;

        [SerializeField]
        public Button Restart;

        private void OnDisable()
        {
            Quit.onClick.RemoveAllListeners();
            Restart.onClick.RemoveAllListeners();
        }
    }
}
