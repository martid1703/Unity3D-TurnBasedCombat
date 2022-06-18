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
        private Transform[] _playerUnitPrefabs;

        [SerializeField]
        private Transform[] _enemyUnitPrefabs;

        [SerializeField]
        private Transform _blur;

        [SerializeField]
        private Transform _battleSpace;

        [SerializeField]
        private Transform _overviewSpace;

        [SerializeField]
        private Transform _unitsContainer;

        [SerializeField]
        private Button _btnQuit;

        [SerializeField]
        private Button _btnRestart;

        [SerializeField]
        private Button _btnAttack;

        [SerializeField]
        private Button _btnSkip;

        private RootUI _rootUI;
        private InGameUI _inGameUI;
        private GameOverUI _gameOverUI;
        private Enemy _enemy;
        private Player _player;
        private CameraController _cameraController;
        private BattleManagerState _battleManagerstate;
        private BattleState _battleState;
        private TurnLogicProvider _turnLogicProvider;

        private Scene _activeScene;
        private bool _gameOver;
        private UnitSelector _unitSelector;


        public List<Unit> PlayerUnits { get; private set; }
        public List<Unit> EnemyUnits { get; private set; }
        public Unit AttackingUnit { get; private set; }
        public Unit AttackedUnit { get; private set; }

        private void Awake()
        {
            _unitSelector = new UnitSelector();
        }

        private void Start()
        {
            _activeScene = SceneManager.GetActiveScene();
            _cameraController = SceneObjectsFinder.FindFirstInRoot<CameraController>(_activeScene);

            PlayerUnits = new List<Unit>();
            EnemyUnits = new List<Unit>();

            _player = GetComponent<Player>();
            _enemy = GetComponent<Enemy>();

            SetupUI();
            Restart();
        }

        private void Update()
        {
            TrackKeyboard();
        }

        private void SetupUI()
        {
            _rootUI = SceneObjectsFinder.FindFirstInRoot<RootUI>(_activeScene);
            _inGameUI = _rootUI.GetComponentInChildren<InGameUI>(true);
            _gameOverUI = _rootUI.GetComponentInChildren<GameOverUI>(true);

            _btnQuit.onClick.AddListener(() =>
            {
                Quit();
            });
            _btnRestart.onClick.AddListener(() =>
            {
                Restart();
            });
            _btnAttack.onClick.AddListener(() =>
            {
                _player.SetState(PlayerState.Attack);
            });
            _btnSkip.onClick.AddListener(() =>
            {
                _player.SetState(PlayerState.Skip);
            });
        }

        private void SetupCamera()
        {
            switch (_battleState)
            {
                case BattleState.Overview:
                    _cameraController.Fit(_overviewSpace.transform);
                    break;
                case BattleState.Battle:
                    _cameraController.Fit(_battleSpace.transform);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_battleState));
            }
        }

        public void SwitchToBattle(Unit attackingUnit, Unit attackedUnit)
        {
            if (_battleState == BattleState.Battle)
            {
                return;
            }

            _battleState = BattleState.Battle;
            SetupCamera();

            var stateSwitcher = new StateSwitcher(_overviewSpace, _battleSpace, PlayerUnits.ToArray(), EnemyUnits.ToArray());
            stateSwitcher.SwitchToBattle(attackingUnit, attackedUnit);
        }

        public void SwitchToOverview()
        {
            if (_battleState == BattleState.Overview)
            {
                return;
            }

            _battleState = BattleState.Overview;
            SetupCamera();

            var stateSwitcher = new StateSwitcher(_overviewSpace, _battleSpace, PlayerUnits.ToArray(), EnemyUnits.ToArray());
            stateSwitcher.SwitchToOverview();
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

                switch (AttackingUnit.UnitData.Type)
                {
                    case UnitType.Player:
                        StartCoroutine(_player.TakeTurn());
                        break;
                    case UnitType.Enemy:
                        StartCoroutine(_enemy.TakeTurn());
                        break;
                    case UnitType.Neutral:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(AttackingUnit.UnitData.Type));
                }
                yield return null;
            }
        }

        private IEnumerator WaitBattleManagerState(BattleManagerState state)
        {
            while (true)
            {
                if (_battleManagerstate != state)
                {
                    Debug.Log($"BattleManagerState waits to enter free state.");
                    yield return null;
                }
                else
                {
                    yield break;
                }
            }
        }

        private void TrackKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                _player.SetState(PlayerState.Attack);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _player.SetState(PlayerState.Skip);
            }
        }

        private void OnUnitSelected(object sender, EventArgs args)
        {
            AttackedUnit = sender as Unit;
            Debug.Log($"Player selected unit to attack. Unit:{AttackedUnit}.");
            _unitSelector.DeselectUnits(EnemyUnits.ToArray(), AttackedUnit);
        }

        public Unit GetAttackedUnit()
        {
            return AttackedUnit;
        }

        private bool IsUnitSelectable(Unit unit)
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

        private void GameOver(UnitType winner)
        {
            _gameOver = true;
            ShowGameOverUI(winner);
        }

        private void ShowGameOverUI(UnitType winner)
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

        public void Restart()
        {
            _gameOver = false;

            DestroyAllUnits();
            SpawnUnits();

            SwitchToOverview();

            SetBattleManagerState(BattleManagerState.Free);

            HideGameOverUI();
            ShowInGameUI();

            _turnLogicProvider = new TurnLogicProvider(
                AttackingUnit,
                PlayerUnits,
                EnemyUnits,
                SetBattleManagerState,
                GameOver
            );
            _turnLogicProvider.CreateBattleQueue();

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
                _playerUnitPrefabs,
                _enemyUnitPrefabs,
                _overviewSpace,
                OnUnitSelected,
                _unitsContainer,
                IsUnitSelectable
            );
            var units = unitSpawner.Spawn();

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
