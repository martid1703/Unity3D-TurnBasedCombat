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
        private UnitType[] _playerUnits;

        [SerializeField]
        private UnitType[] _enemyUnits;

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

        private Enemy _enemy;
        private Player _player;
        private CameraController _cameraController;
        private BattleManagerState _battleManagerstate;
        private BattleState _battleState;
        private TurnLogicProvider _turnLogicProvider;

        private Scene _activeScene;
        private bool _gameOver;
        private UnitSelector _unitSelector;
        private StateSwitcher _stateSwitcher;

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

            _unitSelector = new UnitSelector();
        }

        private void Start()
        {
            PlayerUnits = new List<UnitModel>();
            EnemyUnits = new List<UnitModel>();

            SetupPlayers();
            SetupUI();
            StartCoroutine(Restart());
        }

        private void SetupPlayers()
        {
            _player = GetComponent<Player>();
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
                StartCoroutine(Restart());
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
                _player.IsHuman = !_player.IsHuman;
                if (_player.IsHuman)
                {
                    _btnAttack.interactable = true;
                    _btnSkip.interactable = true;
                    _btnAddPlayerUnits.interactable = true;
                    _btnRemovePlayerUnits.interactable = true;
                }
                else
                {
                    _btnAttack.interactable = false;
                    _btnSkip.interactable = false;
                    _btnAddPlayerUnits.interactable = false;
                    _btnRemovePlayerUnits.interactable = false;
                }
            });
            _battleSpeedSlider.onValueChanged.AddListener((v) => { OnBattleSpeedChange(v); });
            _btnAddPlayerUnits.onClick.AddListener(() =>
           {
               IncrementPlayerUnits(UnitBelonging.Player);
           });
            _btnRemovePlayerUnits.onClick.AddListener(() =>
           {
               DecrementPlayerUnits(UnitBelonging.Player);
           });
            _btnAddEnemyUnits.onClick.AddListener(() =>
          {
              IncrementPlayerUnits(UnitBelonging.Enemy);
          });
            _btnRemoveEnemyUnits.onClick.AddListener(() =>
           {
               DecrementPlayerUnits(UnitBelonging.Enemy);
           });
        }

        private void DecrementPlayerUnits(UnitBelonging unitBelonging)
        {
            UnitModel unit;
            switch (unitBelonging)
            {
                case UnitBelonging.Player:
                    if (PlayerUnits.Count == 1)
                    {
                        return;
                    }
                    unit = GetUnitToRemove(PlayerUnits.ToArray());
                    PlayerUnits.Remove(unit);
                    break;
                case UnitBelonging.Enemy:
                    if (EnemyUnits.Count == 1)
                    {
                        return;
                    }
                    unit = GetUnitToRemove(EnemyUnits.ToArray());
                    EnemyUnits.Remove(unit);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitBelonging));
            }

            unit.DestroySelf();
            StartCoroutine(SwitchToOverview());
        }

        private void IncrementPlayerUnits(UnitBelonging unitBelonging)
        {
            if (_battleState != BattleState.Overview)
            {
                return;
            }

            var unitSpawner = new UnitSpawner(
                            _overviewSpace,
                            OnUnitSelected,
                            IsUnitSelectable,
                            _playerUnitsContainer,
                            _enemyUnitsContainer
                        );

            UnitType unitType;
            List<UnitModel> units;

            switch (unitBelonging)
            {
                case UnitBelonging.Player:
                    unitType = PlayerUnits.Last().UnitData.Type;
                    units = PlayerUnits;
                    break;
                case UnitBelonging.Enemy:
                    unitType = EnemyUnits.Last().UnitData.Type;
                    units = EnemyUnits;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitBelonging));
            }

            unitSpawner.AddUnit(unitBelonging, unitType, units);
            StartCoroutine(SwitchToOverview());
        }

        private UnitModel GetUnitToRemove(UnitModel[] units)
        {
            for (int i = 0; i < units.Length; i++)
            {
                var unit = units[i];
                if (unit.IsSelected)
                {
                    continue;
                }
                return unit;
            }
            throw new InvalidOperationException("Cannot find unit to remove");
        }

        private void Update()
        {
            TrackKeyboard();
        }

        private IEnumerator SetupCamera()
        {
            switch (_battleState)
            {
                case BattleState.Overview:
                    yield return _cameraController.FitOverview(_overviewSpace.rect);
                    break;
                case BattleState.Battle:
                    yield return _cameraController.FitBattle(_battleSpace.rect);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_battleState));
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
            _battleState = BattleState.Battle;
            yield return SetupCamera();
        }

        public IEnumerator SwitchToOverview()
        {
            _turnLogicProvider = new TurnLogicProvider(PlayerUnits, EnemyUnits, SetBattleManagerState, GameOver);
            _turnLogicProvider.CreateBattleQueue();
            _stateSwitcher = new StateSwitcher(_overviewSpace, _battleSpace, PlayerUnits.ToArray(), EnemyUnits.ToArray(), _blur, _inGameUI, _fade);
            _stateSwitcher.SwitchToOverview();
            _battleState = BattleState.Overview;
            yield return SetupCamera();
        }

        public void SetGameStatus(string message)
        {
            _gameStatus.GetComponentInChildren<TMP_Text>().text = message;
        }

        public IEnumerator ReturnUnitsBack()
        {
            var stateSwitcher = new StateSwitcher(_overviewSpace, _battleSpace, PlayerUnits.ToArray(), EnemyUnits.ToArray(), _blur, _inGameUI, _fade);
            _battleState = BattleState.Overview;
            stateSwitcher.ReturnUnitsBack(AttackingUnit, AttackedUnit);
            yield return SetupCamera();
        }

        private IEnumerator StartGameCycle()
        {
            while (!_gameOver)
            {
                yield return WaitBattleManagerState(BattleManagerState.Free);

                AttackingUnit = _turnLogicProvider.NextTurn(AttackedUnit);

                if (_gameOver)
                {
                    yield break;
                }

                switch (AttackingUnit.UnitData.Belonging)
                {
                    case UnitBelonging.Player:
                        StartCoroutine(_player.TakeTurn());
                        break;
                    case UnitBelonging.Enemy:
                        StartCoroutine(_enemy.TakeTurn());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(AttackingUnit.UnitData.Type));
                }
                yield return null;
            }
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

        private void TrackKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                _player.SetState(PlayerTurnState.TakeTurn);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _player.SetState(PlayerTurnState.SkipTurn);
            }
        }

        private void OnUnitSelected(object sender, EventArgs args)
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

        public UnitModel GetAttackedUnit()
        {
            return AttackedUnit;
        }

        private bool IsUnitSelectable(UnitModel unit)
        {
            if (AttackingUnit == null || unit.UnitData.Type == AttackingUnit.UnitData.Type || unit.IsSelected)
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
            _gameOver = true;
            ShowGameOverUI(winner);
        }

        private void ShowGameOverUI(UnitBelonging winner)
        {
            HideInGameUI();

            var msgbox = _gameOverUI.transform.Find("Message");
            var tmp = msgbox.GetComponent<TMP_Text>();
            tmp.text = $"Game Over! \nWinner is {winner}.";
            _gameOverUI.gameObject.SetActive(true);
        }

        private void HideGameOverUI()
        {
            _gameOverUI.gameObject.SetActive(false);
        }

        private void ShowInGameUI()
        {
            _inGameUI.gameObject.SetActive(true);
        }

        private void HideInGameUI()
        {
            _inGameUI.gameObject.SetActive(false);
        }

        public IEnumerator Restart()
        {
            _gameOver = false;
            _player.IsHuman = true;
            _enemy.IsHuman = false;

            DestroyAllUnits();
            SpawnUnits();
            OnBattleSpeedChange(_battleSpeedSlider.value);

            yield return SwitchToOverview();

            SetBattleManagerState(BattleManagerState.Free);

            HideGameOverUI();
            ShowInGameUI();

            StartCoroutine(StartGameCycle());
        }

        private void DestroyAllUnits()
        {
            foreach (var unit in PlayerUnits)
            {
                unit.DestroySelf();
            }

            foreach (var unit in EnemyUnits)
            {
                unit.DestroySelf();
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

            UnitSpawnerResult units = unitSpawner.Spawn(_playerUnits, _enemyUnits);

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
