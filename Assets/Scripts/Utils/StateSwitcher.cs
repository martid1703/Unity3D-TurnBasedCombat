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

        public StateSwitcher(Transform overviewSpace, Transform battleSpace, Unit[] playerUnits, Unit[] enemyUnits)
        {
            _overviewSpace = overviewSpace;
            _battleSpace = battleSpace;
            _playerUnits = playerUnits;
            _enemyUnits = enemyUnits;

            _unitPositioner = new UnitPositioner();
        }

        public void SwitchToOverview()
        {
            _unitPositioner.PositionUnits(_playerUnits, _enemyUnits, _overviewSpace.GetComponent<RectTransform>());
            _unitPositioner.ChangeSortingLayer(_playerUnits, "UnitsOverview");
            _unitPositioner.ChangeSortingLayer(_enemyUnits, "UnitsOverview");
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
            //_unitPositioner.PositionUnits(_playerUnits, _enemyUnits, _battleSpace.GetComponent<RectTransform>());
            _unitPositioner.ChangeSortingLayer(_playerUnits, "UnitsBattle");
            _unitPositioner.ChangeSortingLayer(_enemyUnits, "UnitsBattle");
            _blur.gameObject.SetActive(true);
        }
    }
}
