using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace UnfrozenTestWork
{
    [RequireComponent(typeof(Player))]
    [RequireComponent(typeof(Enemy))]
    public class BattleManager : MonoBehaviour
    {
        [SerializeField]
        private UnitType[] _playerUnitsPrefabs;

        [SerializeField]
        private UnitType[] _enemyUnitsPrefabs;

        [SerializeField]
        private RectTransform _battleSpace;

        [SerializeField]
        private RectTransform _overviewSpace;

        [SerializeField]
        private Transform _playerUnitsContainer;

        [SerializeField]
        private Transform _enemyUnitsContainer;


        private CameraController _cameraController;
        private BattleManagerState _battleManagerstate;
        private TurnLogicProvider _turnLogicProvider;
        public bool IsGameOver { get; private set; }
        private StateSwitcher _stateSwitcher;
        private UIManager _uiManager;
        private UnitSpawner _unitSpawner;
        private UnitModelProvider _unitModelProvider;
        private BattleQueuePresenter _battleQueuePresenter;
        private float _battleSpeed;


        public bool IsAutoBattle { get; private set; }
        public Enemy Enemy { get; private set; }
        public Player Player { get; private set; }
        public Transform OverviewSpace => _overviewSpace;
        public BattleState BattleState { get; private set; }
        public List<UnitModel> PlayerUnits { get; private set; }
        public List<UnitModel> EnemyUnits { get; private set; }
        public UnitModel AttackingUnit { get; private set; }
        public UnitModel AttackedUnit { get; private set; }

        private void Awake()
        {
            var _activeScene = SceneManager.GetActiveScene();
            _cameraController = SceneObjectsFinder.FindFirstInRoot<CameraController>(_activeScene);
            _uiManager = FindObjectOfType<UIManager>();
            Player = GetComponent<Player>();
            Enemy = GetComponent<Enemy>();
            _unitModelProvider = FindObjectOfType<UnitModelProvider>();
            _battleQueuePresenter = FindObjectOfType<BattleQueuePresenter>();
        }

        private void Start()
        {
            _unitSpawner = new UnitSpawner(
                _unitModelProvider,
                _overviewSpace,
                OnUnitSelected,
                OnUnitIsDead,
                IsUnitSelectable,
                IsUnitSelectableAsTarget,
                _playerUnitsContainer,
                _enemyUnitsContainer,
                OnUnitMouseOnExit
                );

            BattleSpeedChange += _unitSpawner.OnBattleSpeedChange;
            _turnLogicProvider = new TurnLogicProvider(SetBattleManagerState, GameOver, _battleQueuePresenter);
        }

        public IEnumerator StartGame()
        {
            yield return _uiManager.FadeScreen(false);

            yield return ValidateGameStartConditions();

            IsGameOver = false;

            UnitManager.DestroyAllUnits(PlayerUnits, EnemyUnits);

            UnitSpawnerResult spawnResult = _unitSpawner.Spawn(_playerUnitsPrefabs, _enemyUnitsPrefabs);

            PlayerUnits = spawnResult.PlayerUnits.ToList();
            EnemyUnits = spawnResult.EnemyUnits.ToList();

            var battleQueue = _turnLogicProvider.CreateBattleQueue(PlayerUnits, EnemyUnits);
            yield return _battleQueuePresenter.AddUnitIcons(battleQueue);

            _stateSwitcher = new StateSwitcher(_overviewSpace, _battleSpace, PlayerUnits, EnemyUnits, _uiManager);

            yield return SwitchToOverview(true);

            yield return _uiManager.FadeScreen(true);

            SetBattleManagerState(BattleManagerState.Free);

            yield return StartGameCycle();
        }

        private IEnumerator StartGameCycle()
        {
            while (true)
            {
                yield return WaitBattleManagerState(BattleManagerState.Free);

                AttackingUnit = _turnLogicProvider.NextTurn();
                AttackedUnit = null;

                if (IsGameOver)
                {
                    yield break;
                }

                if (IsPlayerTurn())
                {
                    yield return Player.TakeTurn();
                }
                else
                {
                    yield return Enemy.TakeTurn();
                }

                _battleQueuePresenter.TakeTurn(AttackingUnit);

                yield return null;
            }
        }

        private bool IsPlayerTurn()
        {
            if (AttackingUnit == null || AttackingUnit.UnitData.Belonging != UnitBelonging.Player)
            {
                return false;
            }
            return true;
        }

        private IEnumerator ValidateGameStartConditions()
        {
            if (_playerUnitsPrefabs.Count() == 0 || _enemyUnitsPrefabs.Count() == 0)
            {
                var msg = "Player or Enemy units are not set. Exiting...";
                _uiManager.ShowGameOverUI(msg, false);
                yield return new WaitForSeconds(3f);
                Quit();
                yield break;
            }
        }

        public void SetPlayerState(PlayerTurnState state)
        {
            if (IsPlayerTurn())
            {
                Player.SetState(state);
            }
            else
            {
                Enemy.SetState(state);
            }
        }

        public void SwitchAutoBattle(bool value)
        {
            IsAutoBattle = value;
        }

        public void DecrementUnits(UnitBelonging unitBelonging)
        {
            var unitQtyChanger = new UnitQtyChanger(_unitSpawner, PlayerUnits, EnemyUnits);
            var unit = unitQtyChanger.Decrement(unitBelonging);
            OnUnitIsDead(unit, new EventArgs());

            StartCoroutine(SwitchToOverview(false));
        }

        public void IncrementUnits(UnitBelonging unitBelonging)
        {
            var unitQtyChanger = new UnitQtyChanger(_unitSpawner, PlayerUnits, EnemyUnits);
            var unit = unitQtyChanger.Increment(unitBelonging);
            _turnLogicProvider.AddToBattleQueue(unit);
            _battleQueuePresenter.AddUnitIcon(unit);
            OnBattleSpeedChange(_battleSpeed);
            StartCoroutine(SwitchToOverview(false));
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

        public EventHandler<BattleSpeedEventArgs> BattleSpeedChange;
        public void OnBattleSpeedChange(float speed)
        {
            _battleSpeed = speed;
            BattleSpeedChange?.Invoke(this, new BattleSpeedEventArgs(speed));
        }

        public IEnumerator SwitchToBattle(UnitModel attackingUnit, UnitModel attackedUnit)
        {
            _uiManager.SwitchToBattleMode();
            _stateSwitcher.SwitchToBattle(attackingUnit, attackedUnit);
            BattleState = BattleState.Battle;
            yield return SetupCamera();
        }

        public IEnumerator SwitchToOverview(bool isNewGame)
        {
            _uiManager.SwitchToOverviewMode(isNewGame);
            _stateSwitcher.SwitchToOverview();
            BattleState = BattleState.Overview;
            yield return SetupCamera();
        }

        public void SetGameStatus(string message)
        {
            _uiManager.SetGameStatus(message);
        }

        public IEnumerator ReturnUnitsBack()
        {
            _uiManager.SwitchToOverviewMode(false);
            BattleState = BattleState.Overview;
            _stateSwitcher.RevertUnitsBack(AttackingUnit, AttackedUnit);
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

        public void OnUnitSelected(object sender, EventArgs args)
        {
            AttackedUnit = sender as UnitModel;
            Debug.Log($"Player selected unit to attack. Unit:{AttackedUnit}.");
            if (AttackedUnit.IsEnemy)
            {
                UnitManager.DeselectUnitsExceptOne(EnemyUnits, AttackedUnit);
                return;
            }
            UnitManager.DeselectUnitsExceptOne(PlayerUnits, AttackedUnit);
        }

        public void OnUnitIsDead(object sender, EventArgs args)
        {
            if (sender == null)
            {
                return;
            }

            var unit = sender as UnitModel;

            switch (unit.UnitData.Belonging)
            {
                case UnitBelonging.Player:
                    PlayerUnits.Remove(unit);
                    break;
                case UnitBelonging.Enemy:
                    EnemyUnits.Remove(unit);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unit.UnitData.Type));
            }
            _battleQueuePresenter.RemoveUnitIcon(unit);
            unit.Kill();
        }

        public bool IsUnitSelectable(UnitModel unit)
        {
            if (BattleState == BattleState.Battle || EventSystem.current.IsPointerOverGameObject())
            {
                return false;
            }

            if (IsPlayerTurn() & unit.IsEnemy)
            {
                _uiManager.SetAttackCursor();
            }
            if (!IsPlayerTurn() & !unit.IsEnemy)
            {
                _uiManager.SetAttackCursor();
            }
            return true;
        }

        public void OnUnitMouseOnExit(object sender, EventArgs args)
        {
            if (BattleState == BattleState.Battle)
            {
                return;
            }
            _uiManager.SetRegularCursor();
        }

        public bool IsUnitSelectableAsTarget(UnitModel unit)
        {
            if (!IsUnitSelectable(unit))
            {
                return false;
            }

            if (AttackingUnit == null || unit.UnitData.Belonging == AttackingUnit.UnitData.Belonging || unit.IsSelectedAsTarget)
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
            IsGameOver = true;
            _uiManager.HideInGameUI();
            _uiManager.ShowGameOverUI(msg, true);
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
