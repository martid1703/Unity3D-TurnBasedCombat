using UnityEngine;

namespace UnfrozenTestWork
{
    public class StateSwitcher
    {
        private readonly Transform _overviewSpace;
        private readonly Transform _battleSpace;
        private readonly Transform _blur;
        private Unit[] _playerUnits;
        private Unit[] _enemyUnits;

        private readonly UnitPositioner _unitPositioner;

        public StateSwitcher(Transform overviewSpace, Transform battleSpace, Unit[] playerUnits, Unit[] enemyUnits, Transform blur)
        {
            _overviewSpace = overviewSpace ?? throw new System.ArgumentNullException(nameof(overviewSpace));
            _battleSpace = battleSpace ?? throw new System.ArgumentNullException(nameof(battleSpace));
            _playerUnits = playerUnits ?? throw new System.ArgumentNullException(nameof(playerUnits));
            _enemyUnits = enemyUnits ?? throw new System.ArgumentNullException(nameof(enemyUnits));
            _blur = blur ?? throw new System.ArgumentNullException(nameof(blur));

            _unitPositioner = new UnitPositioner();
        }

        public void SwitchToOverview()
        {
            _unitPositioner.ChangeSortingLayer(_playerUnits, "UnitsOverview");
            _unitPositioner.ChangeSortingLayer(_enemyUnits, "UnitsOverview");
            _unitPositioner.PositionUnitsOverview(_playerUnits, _enemyUnits, _overviewSpace.GetComponent<RectTransform>());
            _blur.gameObject.SetActive(false);
        }

        public void SwitchToBattle(Unit attackingUnit, Unit attackedUnit)
        {
            if (attackingUnit.UnitData.Type == UnitType.Player)
            {
                _playerUnits = new Unit[] { attackingUnit };
                _enemyUnits = new Unit[] { attackedUnit };
            }
            else
            {
                _playerUnits = new Unit[] { attackedUnit };
                _enemyUnits = new Unit[] { attackingUnit };
            }
            _unitPositioner.ChangeSortingLayer(_playerUnits, "UnitsBattle");
            _unitPositioner.ChangeSortingLayer(_enemyUnits, "UnitsBattle");
            _unitPositioner.PositionUnitsBattle(_playerUnits, _enemyUnits, _battleSpace.GetComponent<RectTransform>());
            _blur.gameObject.SetActive(true);
        }
    }
}
