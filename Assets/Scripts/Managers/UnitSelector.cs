namespace UnfrozenTestWork
{
    public class UnitSelector
    {
        public void DeselectUnitsExceptOne(UnitModel[] units, UnitModel exceptSelected)
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

        public void DeselectUnits(UnitModel[] units)
        {
            for (int i = 0; i < units.Length; i++)
            {
                units[i].Deselect();
            }
        }
    }
}