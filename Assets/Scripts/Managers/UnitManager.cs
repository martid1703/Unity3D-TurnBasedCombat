using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class UnitManager : MonoBehaviour
    {
        [SerializeField]
        private UIManager _uiManager;

        public void OnUnitMouseOver(object sender, EventArgs args)
        {
            var unit = sender as UnitModel;
            if (unit.IsEnemy)
            {
                _uiManager.SetAttackCursor();
                return;
            }
            _uiManager.SetRegularCursor();
        }

        public static void DeselectUnitsExceptOne(IEnumerable<UnitModel> units, UnitModel exceptSelected)
        {
            foreach (var unit in units)
            {
                if (unit == exceptSelected)
                {
                    continue;
                }
                unit.Deselect();
            }
        }

        public static void DeselectUnits(IEnumerable<UnitModel> units)
        {
            foreach (var unit in units)
            {
                unit.Deselect();
            }
        }

        public static void DestroyAllUnits(IEnumerable<UnitModel> playerUnits, IEnumerable<UnitModel> enemyUnits)
        {
            if (playerUnits != null)
            {
                foreach (var unit in playerUnits)
                {
                    unit.Kill();
                }
            }

            if (enemyUnits != null)
            {
                foreach (var unit in enemyUnits)
                {
                    unit.Kill();
                }
            }
        }
    }
}