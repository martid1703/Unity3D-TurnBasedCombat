using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class StateSwitcher
    {
        private readonly RectTransform _overviewSpace;
        private readonly RectTransform _battleSpace;
        private readonly List<UnitModel> _playerUnits;
        private readonly List<UnitModel> _enemyUnits;

        private UnitPositioner _unitPositioner;
        private UIManager _uiManager;

        public StateSwitcher(
            RectTransform overviewSpace,
            RectTransform battleSpace,
            List<UnitModel> playerUnits,
            List<UnitModel> enemyUnits,
            UIManager uiManager)
        {
            _overviewSpace = overviewSpace ?? throw new ArgumentNullException(nameof(overviewSpace));
            _battleSpace = battleSpace ?? throw new ArgumentNullException(nameof(battleSpace));
            _playerUnits = playerUnits ?? throw new ArgumentNullException(nameof(playerUnits));
            _enemyUnits = enemyUnits ?? throw new ArgumentNullException(nameof(enemyUnits));
            _uiManager = uiManager ?? throw new ArgumentNullException(nameof(uiManager));

            _unitPositioner = new UnitPositioner();
        }

        public void SwitchToOverview()
        {
            _unitPositioner.PositionUnitsOverview(_playerUnits.ToArray(), _enemyUnits.ToArray(), _overviewSpace.rect);
            RemoveBackgroundBattleEffects();
        }

        public void RevertUnitsBack(UnitModel attackingUnit, UnitModel attackedUnit)
        {
            _unitPositioner.RevertUnitsBack(attackingUnit, attackedUnit);
            RemoveBackgroundBattleEffects();
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

            _unitPositioner.PositionUnitsBattle(playerUnits, enemyUnits, _battleSpace.rect);
            AddBackgroundBattleEffects();
        }

        private void AddBackgroundBattleEffects()
        {
            _uiManager.AddBackgroundBattleEffects();
        }

        private void RemoveBackgroundBattleEffects()
        {
            _uiManager.RemoveBackgroundBattleEffects();
        }
    }
}
