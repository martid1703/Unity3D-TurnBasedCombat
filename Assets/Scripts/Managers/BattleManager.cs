using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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


        private CameraController _cameraController;
        private BattleManagerState _battleManagerstate;
        private TurnLogicProvider _turnLogicProvider;
        private bool _gameOver;
        private StateSwitcher _stateSwitcher;
        private UIManager _uiManager;
        private UnitSpawner _unitSpawner;

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
        }

        private void Start()
        {
            _unitSpawner = new UnitSpawner(
                            _overviewSpace,
                            OnUnitSelected,
                            IsUnitSelectable,
                            IsUnitSelectableAsTarget,
                            _playerUnitsContainer,
                            _enemyUnitsContainer);
        }

        public IEnumerator Restart(bool isNewGame)
        {
            yield return ValidateGameStartConditions();

            _gameOver = false;

            UnitManager.DestroyAllUnits(PlayerUnits, EnemyUnits);

            UnitSpawnerResult spawnResult = _unitSpawner.Spawn(_playerUnitsPrefabs, _enemyUnitsPrefabs);

            PlayerUnits = spawnResult.PlayerUnits.ToList();
            EnemyUnits = spawnResult.EnemyUnits.ToList();

            _stateSwitcher = new StateSwitcher(_overviewSpace, _battleSpace, spawnResult, _blur, _fade);

            yield return SwitchToOverview();

            SetBattleManagerState(BattleManagerState.Free);

            _uiManager.ShowInGameUI(isNewGame);

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
                    StartCoroutine(Player.TakeTurn());
                }
                else
                {
                    StartCoroutine(Enemy.TakeTurn());
                }

                yield return null;
            }
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

        public void SwitchAutoBattleMode()
        {
            Player.IsHuman = !Player.IsHuman;
        }

        public void DecrementUnits(UnitBelonging unitBelonging)
        {
            var unitQtyChanger = new UnitQtyChanger(_unitSpawner, PlayerUnits, EnemyUnits);
            unitQtyChanger.Decrement(unitBelonging);
            StartCoroutine(SwitchToOverview());
            OnBattleSpeedChange(_uiManager.InGameUI.BattleSpeedSlider.value);
        }

        public void IncrementUnits(UnitBelonging unitBelonging)
        {
            var unitQtyChanger = new UnitQtyChanger(_unitSpawner, PlayerUnits, EnemyUnits);
            unitQtyChanger.Increment(unitBelonging);
            StartCoroutine(SwitchToOverview());
            OnBattleSpeedChange(_uiManager.InGameUI.BattleSpeedSlider.value);
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

        public event EventHandler<BattleSpeedEventArgs> BattleSpeedChange;
        public void OnBattleSpeedChange(float speed)
        {
            BattleSpeedChange?.Invoke(this, new BattleSpeedEventArgs(speed));
        }

        public IEnumerator SwitchToBattle(UnitModel attackingUnit, UnitModel attackedUnit)
        {
            _stateSwitcher.SwitchToBattle(attackingUnit, attackedUnit);
            _uiManager.SwitchToBattleMode();
            BattleState = BattleState.Battle;
            yield return SetupCamera();
        }

        public IEnumerator SwitchToOverview()
        {
            _turnLogicProvider = new TurnLogicProvider(PlayerUnits, EnemyUnits, SetBattleManagerState, GameOver);
            _turnLogicProvider.CreateBattleQueue();
            _stateSwitcher.SwitchToOverview();
            _uiManager.SwitchToOverviewMode();
            BattleState = BattleState.Overview;
            yield return SetupCamera();
        }

        public void SetGameStatus(string message)
        {
            _uiManager.SetGameStatus(message);
        }

        public IEnumerator ReturnUnitsBack()
        {
            BattleState = BattleState.Overview;
            _stateSwitcher.RevertUnitsBack(AttackingUnit, AttackedUnit);
            _uiManager.SwitchToOverviewMode();
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

        public bool IsUnitSelectable(UnitModel unit)
        {
            if (BattleState == BattleState.Battle)
            {
                return false;
            }
            return true;
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
            _gameOver = true;
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
