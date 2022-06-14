using UnityEngine;

namespace UnfrozenTestWork
{
    public class UnitSelector
    {
        public void DeselectUnits(Unit[] units, Unit exceptSelected)
        {
            for (int i = 0; i < units.Length; i++)
            {
                if (units[i] == exceptSelected)
                {
                    continue;
                }
                units[i].Deselect();
            }
        }

        public void DeselectUnits(Unit[] units)
        {
            for (int i = 0; i < units.Length; i++)
            {
                units[i].Deselect();
            }
        }
    }
}