using System;
using System.Collections.Generic;

namespace UnfrozenTestWork
{
    public class UnitComparer : IComparer<Unit>
    {

        public int Compare(Unit x, Unit y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    // If x is null and y is null, they're
                    // equal.
                    return 0;
                }
                else
                {
                    // If x is null and y is not null, y
                    // is greater.
                    return -1;
                }
            }
            else
            {
                // If x is not null...
                //
                if (y == null)
                // ...and y is null, x is greater.
                {
                    return 1;
                }
                else
                {
                    // ...and y is not null, compare the
                    // lengths of the two strings.
                    //
                    int retval = x.UnitData.Initiative.CompareTo(y.UnitData.Initiative);

                    return retval;

                }
            }
        }
    }
}
