using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class StateSwitcher
    {
        private readonly RectTransform _overviewSpace;
        private readonly RectTransform _battleSpace;
        private readonly List<UnitModel> _player1Units;
        private readonly List<UnitModel> _player2Units;

        private UnitPositioner _unitPositioner;
        private UIManager _uiManager;

        public StateSwitcher(
            RectTransform overviewSpace,
            RectTransform battleSpace,
            List<UnitModel> player1Units,
            List<UnitModel> player2Units,
            UIManager uiManager)
        {
            _overviewSpace = overviewSpace ?? throw new ArgumentNullException(nameof(overviewSpace));
            _battleSpace = battleSpace ?? throw new ArgumentNullException(nameof(battleSpace));
            _player1Units = player1Units ?? throw new ArgumentNullException(nameof(player1Units));
            _player2Units = player2Units ?? throw new ArgumentNullException(nameof(player2Units));
            _uiManager = uiManager ?? throw new ArgumentNullException(nameof(uiManager));

            _unitPositioner = new UnitPositioner();
        }

        public void SwitchToOverview()
        {
            _unitPositioner.PositionUnitsOverview(_player1Units.ToArray(), _player2Units.ToArray(), _overviewSpace.rect);
            RemoveBackgroundBattleEffects();
        }

        public void RevertUnitsBack(UnitModel attackingUnit, UnitModel attackedUnit)
        {
            _unitPositioner.RevertUnitsBack(attackingUnit, attackedUnit);
            RemoveBackgroundBattleEffects();
        }

        public void SwitchToBattle(UnitModel attackingUnit, UnitModel attackedUnit)
        {
            UnitModel[] player1Units;
            UnitModel[] player2Units;

            if (attackingUnit.UnitData.Belonging == UnitBelonging.Player1)
            {
                player1Units = new UnitModel[] { attackingUnit };
                player2Units = new UnitModel[] { attackedUnit };
            }
            else
            {
                player1Units = new UnitModel[] { attackedUnit };
                player2Units = new UnitModel[] { attackingUnit };
            }

            _unitPositioner.PositionUnitsBattle(player1Units, player2Units, _battleSpace.rect);
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
