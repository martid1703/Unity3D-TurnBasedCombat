using System;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class StateSwitcher
    {
        private readonly RectTransform _overviewSpace;
        private readonly RectTransform _battleSpace;
        private readonly Transform _blur;
        private readonly Transform _fade;
        private readonly InGameUI _inGameUI;
        private readonly Unit[] _playerUnits;
        private readonly Unit[] _enemyUnits;

        private readonly UnitPositioner _unitPositioner;

        public StateSwitcher(RectTransform overviewSpace, RectTransform battleSpace, Unit[] playerUnits, Unit[] enemyUnits, Transform blur, InGameUI inGameUI, Transform fade)
        {
            _overviewSpace = overviewSpace ?? throw new System.ArgumentNullException(nameof(overviewSpace));
            _battleSpace = battleSpace ?? throw new System.ArgumentNullException(nameof(battleSpace));
            _playerUnits = playerUnits ?? throw new System.ArgumentNullException(nameof(playerUnits));
            _enemyUnits = enemyUnits ?? throw new System.ArgumentNullException(nameof(enemyUnits));
            _blur = blur ?? throw new System.ArgumentNullException(nameof(blur));
            _inGameUI = inGameUI ?? throw new System.ArgumentNullException(nameof(inGameUI));
            _fade = fade ?? throw new ArgumentNullException(nameof(fade));

            _unitPositioner = new UnitPositioner();
        }

        public void SwitchToOverview()
        {
            _unitPositioner.PositionUnitsOverview(_playerUnits, _enemyUnits, _overviewSpace.rect);
            SwitchUIToOverview();
        }

        public void RestoreUnitPositions(Unit attackingUnit, Unit attackedUnit)
        {
            _unitPositioner.ReturnToOverview(attackingUnit, attackedUnit);
            SwitchUIToOverview();
        }

        private void SwitchUIToOverview()
        {
            _blur.gameObject.SetActive(false);
            _fade.gameObject.SetActive(false);
            _inGameUI.gameObject.SetActive(true);
        }

        public void SwitchToBattle(Unit attackingUnit, Unit attackedUnit)
        {
            Unit[] playerUnits;
            Unit[] enemyUnits;

            if (attackingUnit.UnitData.Type == UnitType.Player)
            {
                playerUnits = new Unit[] { attackingUnit };
                enemyUnits = new Unit[] { attackedUnit };
            }
            else
            {
                playerUnits = new Unit[] { attackedUnit };
                enemyUnits = new Unit[] { attackingUnit };
            }

            var fitInto = _battleSpace.GetComponent<RectTransform>();
            _unitPositioner.PositionUnitsBattle(playerUnits, enemyUnits, fitInto.rect);

            SwitchUIToBattle();
        }

        private void SwitchUIToBattle()
        {
            _blur.gameObject.SetActive(true);
            _fade.gameObject.SetActive(true);
            _inGameUI.gameObject.SetActive(false);
        }
    }
}
