using System;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class StateSwitcher
    {
        private readonly Transform _overviewSpace;
        private readonly Transform _battleSpace;
        private readonly Transform _blur;
        private readonly InGameUI _inGameUI;
        private Unit[] _playerUnits;
        private Unit[] _enemyUnits;

        private readonly UnitPositioner _unitPositioner;
        private readonly UnitScaler _unitScaler;

        public StateSwitcher(Transform overviewSpace, Transform battleSpace, Unit[] playerUnits, Unit[] enemyUnits, Transform blur, InGameUI inGameUI)
        {
            _unitPositioner = new UnitPositioner();
            _unitScaler = new UnitScaler();

            _overviewSpace = overviewSpace ?? throw new System.ArgumentNullException(nameof(overviewSpace));
            _battleSpace = battleSpace ?? throw new System.ArgumentNullException(nameof(battleSpace));
            _playerUnits = playerUnits ?? throw new System.ArgumentNullException(nameof(playerUnits));
            _enemyUnits = enemyUnits ?? throw new System.ArgumentNullException(nameof(enemyUnits));
            _blur = blur ?? throw new System.ArgumentNullException(nameof(blur));
            _inGameUI = inGameUI ?? throw new System.ArgumentNullException(nameof(inGameUI));
        }

        public void SwitchToOverview()
        {
            _unitPositioner.ChangeSortingLayer(_playerUnits, "UnitsOverview");
            _unitPositioner.ChangeSortingLayer(_enemyUnits, "UnitsOverview");
            RectTransform fitInto = _overviewSpace.GetComponent<RectTransform>();

            _unitPositioner.PositionUnitsOverview(_playerUnits, _enemyUnits, fitInto);

            var units = new Unit[_playerUnits.Length + _enemyUnits.Length];
            Array.Copy(_playerUnits, 0, units, 0, _playerUnits.Length);
            Array.Copy(_enemyUnits, 0, units, _playerUnits.Length, _enemyUnits.Length);

            _unitScaler.ScaleUnits(units, fitInto.rect);

            _unitPositioner.PositionUnitsOverview(_playerUnits, _enemyUnits, fitInto);



            _blur.gameObject.SetActive(false);
            _inGameUI.gameObject.SetActive(true);
        }

        public void RestoreUnitPositions(Unit attackingUnit, Unit attackedUnit)
        {
            _unitPositioner.ChangeSortingLayer(new Unit[] { attackingUnit }, "UnitsOverview");
            _unitPositioner.ChangeSortingLayer(new Unit[] { attackedUnit }, "UnitsOverview");
            _unitPositioner.RestoreUnitsPositions(attackingUnit, attackedUnit);

            _blur.gameObject.SetActive(false);
            _inGameUI.gameObject.SetActive(true);
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
            _inGameUI.gameObject.SetActive(false);
        }
    }
}
