using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class StateSwitcher
    {
        private readonly RectTransform _overviewSpace;
        private readonly RectTransform _battleSpace;
        private readonly Transform _blur;
        private readonly Transform _fade;
        private readonly List<UnitModel> _playerUnits;
        private readonly List<UnitModel> _enemyUnits;

        public StateSwitcher(RectTransform overviewSpace, RectTransform battleSpace, Transform blur, Transform fade, List<UnitModel> playerUnits, List<UnitModel> enemyUnits)
        {
            _overviewSpace = overviewSpace ?? throw new ArgumentNullException(nameof(overviewSpace));
            _battleSpace = battleSpace ?? throw new ArgumentNullException(nameof(battleSpace));
            _blur = blur ?? throw new ArgumentNullException(nameof(blur));
            _fade = fade ?? throw new ArgumentNullException(nameof(fade));
            _playerUnits = playerUnits ?? throw new ArgumentNullException(nameof(playerUnits));
            _enemyUnits = enemyUnits ?? throw new ArgumentNullException(nameof(enemyUnits));
        }

        public void SwitchToOverview()
        {
            UnitPositioner.Instance.PositionUnitsOverview(_playerUnits.ToArray(), _enemyUnits.ToArray(), _overviewSpace.rect);
            RemoveBackgroundBattleEffects();
        }

        public void RevertUnitsBack(UnitModel attackingUnit, UnitModel attackedUnit)
        {
            UnitPositioner.Instance.RevertUnitsBack(attackingUnit, attackedUnit);
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

            UnitPositioner.Instance.PositionUnitsBattle(playerUnits, enemyUnits, _battleSpace.rect);
            AddBackgroundBattleEffects();
        }

        private void AddBackgroundBattleEffects()
        {
            _blur.gameObject.SetActive(true);
            _fade.gameObject.SetActive(true);
        }

        private void RemoveBackgroundBattleEffects()
        {
            _blur.gameObject.SetActive(false);
            _fade.gameObject.SetActive(false);
        }
    }
}
