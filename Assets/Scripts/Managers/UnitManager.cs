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
            if (unit.IsPlayer2)
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

        public static void DestroyAllUnits(IEnumerable<UnitModel> player1Units, IEnumerable<UnitModel> player2Units)
        {
            if (player1Units != null)
            {
                foreach (var unit in player1Units)
                {
                    unit.Kill();
                }
            }

            if (player2Units != null)
            {
                foreach (var unit in player2Units)
                {
                    unit.Kill();
                }
            }
        }
    }
}