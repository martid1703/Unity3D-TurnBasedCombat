using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnfrozenTestWork
{
    [RequireComponent(typeof(Player))]
    [RequireComponent(typeof(Enemy))]
    public class BattleManager : SingletonMonobehaviour<BattleManager>
    {
        [SerializeField]
        private UnitType[] _playerUnitsPrefabs;

        [SerializeField]
        private UnitType[] _enemyUnitsPrefabs;

        [SerializeField]
        private Transform _blur;

        [SerializeField]
        private Transform _fade;

        [SerializeField]
        private RectTransform _battleSpace;

        [SerializeField]
        private RectTransform _overviewSpace;

        [SerializeField]
        private Transform _playerUnitsContainer;

        [SerializeField]
        private Transform _enemyUnitsContainer;

        [SerializeField]
        private Button _btnQuit;

        [SerializeField]
        private Button _btnRestart;

        [SerializeField]
        private Button _btnAttack;

        [SerializeField]
        private Button _btnSkip;

        [SerializeField]
        private Button _btnAutoBattle;

        [SerializeField]
        private Button _btnAddPlayerUnits;

        [SerializeField]
        private Button _btnRemovePlayerUnits;

        [SerializeField]
        private Button _btnAddEnemyUnits;

        [SerializeField]
        private Button _btnRemoveEnemyUnits;

        [SerializeField]
        private Transform _gameStatus;

        [SerializeField]
        private Texture2D _handCursor;

        [SerializeField]
        private Texture2D _swordCursor;
        private CursorMode _cursorMode = CursorMode.ForceSoftware;
        private Vector2 _cursorHotSpot = Vector2.zero;

        private RootUI _rootUI;
        private InGameUI _inGameUI;
        private GameOverUI _gameOverUI;
        private Slider _battleSpeedSlider;
        private readonly float _defaultBattleSpeed = 2f;

        private Enemy _enemy;
        private Player _player;
        private bool _playerIsHuman;
        private CameraController _cameraController;
        private BattleManagerState _battleManagerstate;
        private TurnLogicProvider _turnLogicProvider;

        private Scene _activeScene;
        private bool _gameOver;
        private UnitSelector _unitSelector;
        private StateSwitcher _stateSwitcher;

        public Transform OverviewSpace => _overviewSpace;
        public BattleState BattleState { get; private set; }
        public List<UnitModel> PlayerUnits { get; private set; }
        public List<UnitModel> EnemyUnits { get; private set; }
        public UnitModel AttackingUnit { get; private set; }
        public UnitModel AttackedUnit { get; private set; }

        private void Awake()
        {
            _activeScene = SceneManager.GetActiveScene();
            _cameraController = SceneObjectsFinder.FindFirstInRoot<CameraController>(_activeScene);
            _rootUI = SceneObjectsFinder.FindFirstInRoot<RootUI>(_activeScene);
            _inGameUI = _rootUI.GetComponentInChildren<InGameUI>(true);
            _gameOverUI = _rootUI.GetComponentInChildren<GameOverUI>(true);
            _battleSpeedSlider = _inGameUI.GetComponentInChildren<Slider>(true);
            _battleSpeedSlider.value = _defaultBattleSpeed;

            _unitSelector = new UnitSelector();
        }

        private void Start()
        {
            StopAllCoroutines();
            PutGameOnPause(false);
            StartCoroutine(Restart(true));
        }

        public IEnumerator Restart(bool isNewGame)
        {
            yield return ValidateGameStartConditions();

            SetupPlayers(isNewGame);

            if (isNewGame)
            {
                SetupUI();
            }

            _gameOver = false;

            DestroyAllUnits();
            SpawnUnits();

            _battleSpeedSlider.value = _defaultBattleSpeed;

            yield return SwitchToOverview();

            SetBattleManagerState(BattleManagerState.Free);

            HideGameOverUI();
            ShowInGameUI();



            StartCoroutine(StartGameCycle());
        }

        private IEnumerator StartGameCycle()
        {
            while (true)
            {
                yield return WaitBattleManagerState(BattleManagerState.Free);

                AttackingUnit = _turnLogicProvider.NextTurn(AttackedUnit);

                if (_gameOver)
                {
                    yield break;
                }

                if (_turnLogicProvider.IsPlayerTurn())
                {
                    StartCoroutine(_player.TakeTurn());
                }
                else
                {
                    StartCoroutine(_enemy.TakeTurn());
                }

                yield return null;
            }
        }

        private IEnumerator ValidateGameStartConditions()
        {
            if (_playerUnitsPrefabs.Count() == 0 || _enemyUnitsPrefabs.Count() == 0)
            {
                var msg = "Player or Enemy units are not set. Exiting...";
                ShowGameOverUI(msg, false);
                yield return new WaitForSeconds(3f);
                Quit();
                yield break;
            }
        }

        private void SetupPlayers(bool isNewGame)
        {
            _player = GetComponent<Player>();
            if (isNewGame)
            {
                _playerIsHuman = _player.IsHuman;
            }
            else
            {
                _player.IsHuman = _playerIsHuman;
            }
            _enemy = GetComponent<Enemy>();
        }

        private void SetupUI()
        {
            _btnQuit.onClick.AddListener(() =>
            {
                Quit();
            });
            _btnRestart.onClick.AddListener(() =>
            {
                StopAllCoroutines();
                PutGameOnPause(false);
                StartCoroutine(Restart(false));
            });
            _btnAttack.onClick.AddListener(() =>
            {
                _player.SetState(PlayerTurnState.TakeTurn);
            });
            _btnSkip.onClick.AddListener(() =>
            {
                _player.SetState(PlayerTurnState.SkipTurn);
            });
            _btnAutoBattle.onClick.AddListener(() =>
            {
                SwitchAutoBattleMode();
            });
            _battleSpeedSlider.onValueChanged.AddListener((v) => { OnBattleSpeedChange(v); });
            _btnAddPlayerUnits.onClick.AddListener(() =>
           {
               IncrementUnits(UnitBelonging.Player);
           });
            _btnRemovePlayerUnits.onClick.AddListener(() =>
           {
               DecrementUnits(UnitBelonging.Player);
           });
            _btnAddEnemyUnits.onClick.AddListener(() =>
          {
              IncrementUnits(UnitBelonging.Enemy);
          });
            _btnRemoveEnemyUnits.onClick.AddListener(() =>
           {
               DecrementUnits(UnitBelonging.Enemy);
           });
        }

        private void SwitchAutoBattleMode()
        {
            _player.IsHuman = !_player.IsHuman;
            if (_player.IsHuman)
            {
                SwitchUIToAutoBattleMode(false);
            }
            else
            {
                SwitchUIToAutoBattleMode(true);
            }
        }

        private void SwitchUIToAutoBattleMode(bool isOn)
        {
            _btnAttack.interactable = !isOn;
            _btnSkip.interactable = !isOn;
            _btnAddPlayerUnits.interactable = !isOn;
            _btnRemovePlayerUnits.interactable = !isOn;
            _btnAddEnemyUnits.interactable = !isOn;
            _btnRemoveEnemyUnits.interactable = !isOn;
        }

        private void DecrementUnits(UnitBelonging unitBelonging)
        {
            var unitQtyChanger = new UnitQtyChanger(BattleState, _overviewSpace, OnUnitSelected, IsUnitSelectable, _playerUnitsContainer, _enemyUnitsContainer, PlayerUnits, EnemyUnits);
            unitQtyChanger.Decrement(unitBelonging);
            StartCoroutine(SwitchToOverview());
            OnBattleSpeedChange(_battleSpeedSlider.value);
        }

        private void IncrementUnits(UnitBelonging unitBelonging)
        {
            var unitQtyChanger = new UnitQtyChanger(BattleState, _overviewSpace, OnUnitSelected, IsUnitSelectable, _playerUnitsContainer, _enemyUnitsContainer, PlayerUnits, EnemyUnits);
            unitQtyChanger.Increment(unitBelonging);
            StartCoroutine(SwitchToOverview());
            OnBattleSpeedChange(_battleSpeedSlider.value);
        }

        private void Update()
        {
            TrackKeyboard();
        }

        private IEnumerator SetupCamera()
        {
            switch (BattleState)
            {
                case BattleState.Overview:
                    yield return _cameraController.FitOverview(_overviewSpace.rect);
                    break;
                case BattleState.Battle:
                    yield return _cameraController.FitBattle(_battleSpace.rect);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(BattleState));
            }
        }

        public void SetAttackCursor()
        {
            Cursor.SetCursor(_swordCursor, _cursorHotSpot, _cursorMode);
        }

        public void SetRegularCursor()
        {
            Cursor.SetCursor(_handCursor, _cursorHotSpot, _cursorMode);
        }

        public event EventHandler<BattleSpeedEventArgs> BattleSpeedChange;
        private void OnBattleSpeedChange(float speed)
        {
            BattleSpeedChange?.Invoke(this, new BattleSpeedEventArgs(speed));
        }

        public IEnumerator SwitchToBattle(UnitModel attackingUnit, UnitModel attackedUnit)
        {
            _stateSwitcher.SwitchToBattle(attackingUnit, attackedUnit);
            SwitchUIToAutoBattleMode(true);
            BattleState = BattleState.Battle;
            yield return SetupCamera();
        }

        public IEnumerator SwitchToOverview()
        {
            _turnLogicProvider = new TurnLogicProvider(PlayerUnits, EnemyUnits, SetBattleManagerState, GameOver);
            _turnLogicProvider.CreateBattleQueue();
            _stateSwitcher = new StateSwitcher(_overviewSpace, _battleSpace, PlayerUnits.ToArray(), EnemyUnits.ToArray(), _blur, _inGameUI, _fade);
            _stateSwitcher.SwitchToOverview();
            if (_player.IsHuman)
            {
                SwitchUIToAutoBattleMode(false);
            }
            BattleState = BattleState.Overview;
            yield return SetupCamera();
        }

        public void SetGameStatus(string message)
        {
            _gameStatus.GetComponentInChildren<TMP_Text>().text = message;
        }

        public IEnumerator ReturnUnitsBack()
        {
            var stateSwitcher = new StateSwitcher(_overviewSpace, _battleSpace, PlayerUnits.ToArray(), EnemyUnits.ToArray(), _blur, _inGameUI, _fade);
            BattleState = BattleState.Overview;
            stateSwitcher.ReturnUnitsBack(AttackingUnit, AttackedUnit);
            if (_player.IsHuman)
            {
                SwitchUIToAutoBattleMode(false);
            }
            yield return SetupCamera();
        }

        private IEnumerator WaitBattleManagerState(BattleManagerState state)
        {
            Debug.Log($"BattleManagerState waits to enter free state.");
            while (true)
            {
                if (_battleManagerstate != state)
                {
                    yield return new WaitForSeconds(0.1f);
                }
                else
                {
                    Debug.Log($"BattleManagerState is free now.");
                    yield break;
                }
            }
        }

        private bool IsGameOnPause()
        {
            return Time.timeScale == 0;
        }

        private void PutGameOnPause(bool pause)
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

        private void TrackKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!IsGameOnPause())
                {
                    ShowGameOverUI("PAUSE...", true);
                    PutGameOnPause(true);
                }
                else
                {
                    HideGameOverUI();
                    PutGameOnPause(false);
                }
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                _player.SetState(PlayerTurnState.TakeTurn);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _player.SetState(PlayerTurnState.SkipTurn);
            }
        }

        internal void OnUnitSelected(object sender, EventArgs args)
        {
            AttackedUnit = sender as UnitModel;
            Debug.Log($"Player selected unit to attack. Unit:{AttackedUnit}.");
            if (AttackedUnit.IsEnemy)
            {
                _unitSelector.DeselectUnitsExceptOne(EnemyUnits.ToArray(), AttackedUnit);
                return;
            }
            _unitSelector.DeselectUnitsExceptOne(PlayerUnits.ToArray(), AttackedUnit);
        }

        internal bool IsUnitSelectable(UnitModel unit)
        {
            if (AttackingUnit == null || unit.UnitData.Belonging == AttackingUnit.UnitData.Belonging || unit.IsSelected)
            {
                return false;
            }
            return true;
        }

        public void SetBattleManagerState(BattleManagerState state)
        {
            _battleManagerstate = state;
        }

        private void GameOver(UnitBelonging winner)
        {
            var msg = $"Game Over! \nWinner is {winner}.";
            _gameOver = true;
            ShowGameOverUI(msg, true);
        }

        private void ShowGameOverUI(string msg, bool allowRestart)
        {
            HideInGameUI();
            var msgbox = _gameOverUI.transform.Find("Message");
            var tmp = msgbox.GetComponent<TMP_Text>();
            tmp.text = msg;
            ShowGameOverUI(allowRestart);
        }

        private void ShowGameOverUI(bool allowRestart)
        {
            _gameOverUI.gameObject.SetActive(true);
            if (!allowRestart)
            {
                _btnRestart.interactable = false;
            }
        }

        private void HideGameOverUI()
        {
            _gameOverUI.gameObject.SetActive(false);
            ShowInGameUI();
        }

        private void ShowInGameUI()
        {
            _inGameUI.gameObject.SetActive(true);
        }

        private void HideInGameUI()
        {
            _inGameUI.gameObject.SetActive(false);
        }

        private void DestroyAllUnits()
        {
            if (PlayerUnits != null)
            {
                foreach (var unit in PlayerUnits)
                {
                    unit.DestroySelf();
                }
            }

            if (EnemyUnits != null)
            {
                foreach (var unit in EnemyUnits)
                {
                    unit.DestroySelf();
                }
            }
        }

        private void SpawnUnits()
        {
            var unitSpawner = new UnitSpawner(
                _overviewSpace,
                OnUnitSelected,
                IsUnitSelectable,
                _playerUnitsContainer,
                _enemyUnitsContainer
            );

            UnitSpawnerResult units = unitSpawner.Spawn(_playerUnitsPrefabs, _enemyUnitsPrefabs);

            PlayerUnits = units.PlayerUnits.ToList();
            EnemyUnits = units.EnemyUnits.ToList();
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
