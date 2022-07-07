using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace UnfrozenTestWork
{
    [RequireComponent(typeof(Player1))]
    [RequireComponent(typeof(Player2))]
    public class BattleManager : MonoBehaviour
    {
        [SerializeField]
        private UnitType[] _player1UnitsPrefabs;

        [SerializeField]
        private UnitType[] _player2UnitsPrefabs;

        [SerializeField]
        private RectTransform _battleSpace;

        [SerializeField]
        private RectTransform _overviewSpace;

        [SerializeField]
        private Transform _player1UnitsContainer;

        [SerializeField]
        private Transform _player2UnitsContainer;


        private CameraController _cameraController;
        private TurnLogicProvider _turnLogicProvider;
        public bool IsGameOver { get; private set; }
        private StateSwitcher _stateSwitcher;
        private UIManager _uiManager;
        private UnitSpawner _unitSpawner;
        private UnitModelProvider _unitModelProvider;
        private BattleQueuePresenter _battleQueuePresenter;
        private float _battleSpeed;


        public bool IsAutoBattle { get; private set; }
        public Player2 Player2 { get; private set; }
        public Player1 Player1 { get; private set; }
        public Transform OverviewSpace => _overviewSpace;
        public BattleState BattleState { get; private set; }
        public List<UnitModel> Player1Units { get; private set; }
        public List<UnitModel> Player2Units { get; private set; }
        public UnitModel AttackingUnit { get; private set; }
        public UnitModel AttackedUnit { get; private set; }

        private void Awake()
        {
            var _activeScene = SceneManager.GetActiveScene();
            _cameraController = SceneObjectsFinder.FindFirstInRoot<CameraController>(_activeScene);
            _uiManager = FindObjectOfType<UIManager>();
            Player1 = GetComponent<Player1>();
            Player2 = GetComponent<Player2>();
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
                _player1UnitsContainer,
                _player2UnitsContainer,
                OnUnitMouseOnExit
                );

            BattleSpeedChange += _unitSpawner.OnBattleSpeedChange;
            _turnLogicProvider = new TurnLogicProvider(GameOver);
        }

        public IEnumerator StartGame()
        {
            yield return _uiManager.FadeScreen(false);

            yield return ValidateGameStartConditions();

            IsGameOver = false;

            UnitManager.DestroyAllUnits(Player1Units, Player2Units);

            UnitSpawnerResult spawnResult = _unitSpawner.Spawn(_player1UnitsPrefabs, _player2UnitsPrefabs);

            Player1Units = spawnResult.Player1Units.ToList();
            Player2Units = spawnResult.Player2Units.ToList();

            var battleQueue = _turnLogicProvider.CreateBattleQueue(Player1Units, Player2Units);
            yield return _battleQueuePresenter.AddUnitIcons(battleQueue);

            _stateSwitcher = new StateSwitcher(_overviewSpace, _battleSpace, Player1Units, Player2Units, _uiManager);

            yield return SwitchToOverview(true);

            yield return _uiManager.FadeScreen(true);

            yield return RunGameCycle();
        }

        private IEnumerator RunGameCycle()
        {
            while (true)
            {
                AttackingUnit = _turnLogicProvider.NextTurn();
                AttackedUnit = null;

                if (IsGameOver)
                {
                    yield break;
                }

                if (IsPlayer1Turn())
                {
                    yield return Player1.TakeTurn();
                }
                else
                {
                    yield return Player2.TakeTurn();
                }

                _battleQueuePresenter.TakeTurn(AttackingUnit);

                yield return null;
            }
        }

        private bool IsPlayer1Turn()
        {
            if (AttackingUnit == null || AttackingUnit.UnitData.Belonging != UnitBelonging.Player1)
            {
                return false;
            }
            return true;
        }

        private IEnumerator ValidateGameStartConditions()
        {
            if (_player1UnitsPrefabs.Count() == 0 || _player2UnitsPrefabs.Count() == 0)
            {
                var msg = "Player1 or Player2 units are not set. Exiting...";
                Debug.LogError(msg);
                _uiManager.ShowGameOverUI(msg, false);
                yield return new WaitForSecondsRealtime(3f);
                Quit();
                yield break;
            }
        }

        public void SetPlayerState(PlayerTurnState state)
        {
            if (IsPlayer1Turn())
            {
                Player1.SetState(state);
            }
            else
            {
                Player2.SetState(state);
            }
        }

        public void DecrementUnits(UnitBelonging unitBelonging)
        {
            var unitQtyChanger = new UnitQtyChanger(_unitSpawner, Player1Units, Player2Units);
            var unit = unitQtyChanger.Decrement(unitBelonging);
            OnUnitIsDead(unit, new EventArgs());

            StartCoroutine(SwitchToOverview(false));
        }

        public void IncrementUnits(UnitBelonging unitBelonging)
        {
            var unitQtyChanger = new UnitQtyChanger(_unitSpawner, Player1Units, Player2Units);
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

        public void OnUnitSelected(object sender, EventArgs args)
        {
            AttackedUnit = sender as UnitModel;
            Debug.Log($"Player selected unit to attack. Unit:{AttackedUnit}.");
            if (AttackedUnit.IsPlayer2)
            {
                UnitManager.DeselectUnitsExceptOne(Player2Units, AttackedUnit);
                return;
            }
            UnitManager.DeselectUnitsExceptOne(Player1Units, AttackedUnit);
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
                case UnitBelonging.Player1:
                    Player1Units.Remove(unit);
                    break;
                case UnitBelonging.Player2:
                    Player2Units.Remove(unit);
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

            if (IsPlayer1Turn() & unit.IsPlayer2)
            {
                _uiManager.SetAttackCursor();
            }
            if (!IsPlayer1Turn() & !unit.IsPlayer2)
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
