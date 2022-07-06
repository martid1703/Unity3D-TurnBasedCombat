using System.Collections.Generic;

namespace UnfrozenTestWork
{
    public class UnitComparer : IComparer<UnitModel>
    {

        public int Compare(UnitModel x, UnitModel y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {
                    int retval = x.UnitData.Initiative.CompareTo(y.UnitData.Initiative);
                    return retval;
                }
            }
        }
    }
}
