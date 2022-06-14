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
    public class BattleManager : MonoBehaviour
    {
        [SerializeField]
        private Transform[] _playerUnitPrefabs;

        [SerializeField]
        private Transform[] _enemyUnitPrefabs;

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
        private List<Unit> _playerUnits; // in case of many units - change to dictionary
        private List<Unit> _enemyUnits;
        private TurnLogicProvider _turnLogicProvider;
        private Unit _attackingUnit;
        private Unit _attackedUnit;

        private Scene _activeScene;
        private bool _gameOver;

        private UnitSelector _unitSelector;

        private void Awake() {
            _unitSelector = new UnitSelector();
        }

        private void Start()
        {
            _activeScene = SceneManager.GetActiveScene();
            _cameraController = SceneObjectsFinder.FindFirstInRoot<CameraController>(_activeScene);

            _playerUnits = new List<Unit>();
            _enemyUnits = new List<Unit>();

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
                _player.State = PlayerState.Attack;
            });
            _btnSkip.onClick.AddListener(() =>
            {
                _player.State = PlayerState.Skip;
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

        private void SetBattleState(BattleState state)
        {
            _battleState = state;
            switch (_battleState)
            {
                case BattleState.Overview:
                    break;
                case BattleState.Battle:
                    // TODO: move attacking and attacked unit to battleSpace
                    // TODO: make overviewSpace blurred or darkened
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_battleState));

            }
            SetupCamera();
        }

        private IEnumerator StartGameCycle()
        {
            while (!_gameOver)
            {
                yield return WaitBattleManagerState(BattleManagerState.Free);

                _attackingUnit = _turnLogicProvider.NextTurn(_attackedUnit);

                if (_gameOver)
                {
                    yield break;
                }

                switch (_attackingUnit.UnitData.Type)
                {
                    case UnitType.Player:
                        StartCoroutine(_player.TakeTurn(_playerUnits.ToArray(), _enemyUnits.ToArray(), _attackingUnit, GetAttackedUnit, SetBattleState, SetBattleManagerState));
                        break;
                    case UnitType.Enemy:
                        StartCoroutine(_enemy.TakeTurn(_playerUnits.ToArray(), _enemyUnits.ToArray(), _attackingUnit, SetBattleState, SetBattleManagerState));
                        break;
                    case UnitType.Neutral:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(_attackingUnit.UnitData.Type));
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
                    yield return new WaitForSeconds(5f);
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
                _player.State = PlayerState.Attack;
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _player.State = PlayerState.Skip;
            }
        }

        private void OnUnitSelected(object sender, EventArgs args)
        {
            _attackedUnit = sender as Unit;
            Debug.Log($"Player selected unit to attack. Unit:{_attackedUnit}.");
            _unitSelector.DeselectUnits(_enemyUnits.ToArray(), _attackedUnit);
        }

        private Unit GetAttackedUnit()
        {
            return _attackedUnit;
        }

        private bool IsUnitSelectable(Unit unit)
        {
            if (_attackingUnit == null || unit.UnitData.Type == _attackingUnit.UnitData.Type)
            {
                return false;
            }
            return true;
        }

        private void SetBattleManagerState(BattleManagerState state)
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

            SetBattleState(BattleState.Overview);
            _battleManagerstate = BattleManagerState.Free;
            _battleState = BattleState.Overview;

            HideGameOverUI();
            ShowInGameUI();

            DestroyAllUnits();
            SpawnUnits();

            _turnLogicProvider = new TurnLogicProvider(
                _attackingUnit,
                _playerUnits,
                _enemyUnits,
                SetBattleManagerState,
                GameOver
            );
            _turnLogicProvider.CreateBattleQueue();

            StartCoroutine(StartGameCycle());
        }

        private void DestroyAllUnits()
        {
            foreach (var unit in _playerUnits)
            {
                unit.DestroySelf();
            }

            foreach (var unit in _enemyUnits)
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

            _playerUnits = units.PlayerUnits.ToList();
            _enemyUnits = units.EnemyUnits.ToList();
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
