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
        private readonly UnitModel[] _playerUnits;
        private readonly UnitModel[] _enemyUnits;

        public StateSwitcher(RectTransform overviewSpace, RectTransform battleSpace, UnitModel[] playerUnits, UnitModel[] enemyUnits, Transform blur, InGameUI inGameUI, Transform fade)
        {
            _overviewSpace = overviewSpace ?? throw new System.ArgumentNullException(nameof(overviewSpace));
            _battleSpace = battleSpace ?? throw new System.ArgumentNullException(nameof(battleSpace));
            _playerUnits = playerUnits ?? throw new System.ArgumentNullException(nameof(playerUnits));
            _enemyUnits = enemyUnits ?? throw new System.ArgumentNullException(nameof(enemyUnits));
            _blur = blur ?? throw new System.ArgumentNullException(nameof(blur));
            _inGameUI = inGameUI ?? throw new System.ArgumentNullException(nameof(inGameUI));
            _fade = fade ?? throw new ArgumentNullException(nameof(fade));

        }

        public void SwitchToOverview()
        {
            UnitPositioner.Instance.PositionUnitsOverview(_playerUnits, _enemyUnits, _overviewSpace.rect);
            SwitchUIToOverview();
        }

        public void ReturnUnitsBack(UnitModel attackingUnit, UnitModel attackedUnit)
        {
            UnitPositioner.Instance.ReturnUnitsBack(attackingUnit, attackedUnit);
            SwitchUIToOverview();
        }

        private void SwitchUIToOverview()
        {
            _blur.gameObject.SetActive(false);
            _fade.gameObject.SetActive(false);
            _inGameUI.gameObject.SetActive(true);
            //_overviewSpace.gameObject.SetActive(true);
            //_battleSpace.gameObject.SetActive(false);
        }

        public void SwitchToBattle(UnitModel attackingUnit, UnitModel attackedUnit)
        {
            UnitModel[] playerUnits;
            UnitModel[] enemyUnits;

            if (attackingUnit.UnitData.Belonging == UnitBelonging.Player)
            {
                playerUnits = new UnitModel[] { attackingUnit };
                enemyUnits = new UnitModel[] { attackedUnit };
            }
            else
            {
                playerUnits = new UnitModel[] { attackedUnit };
                enemyUnits = new UnitModel[] { attackingUnit };
            }

            var fitInto = _battleSpace.GetComponent<RectTransform>();
            UnitPositioner.Instance.PositionUnitsBattle(playerUnits, enemyUnits, fitInto.rect);

            SwitchUIToBattle();
        }

        private void SwitchUIToBattle()
        {
            _blur.gameObject.SetActive(true);
            _fade.gameObject.SetActive(true);
            //_inGameUI.gameObject.SetActive(false);
            //_overviewSpace.gameObject.SetActive(false);
            //_battleSpace.gameObject.SetActive(true);
        }
    }
}
