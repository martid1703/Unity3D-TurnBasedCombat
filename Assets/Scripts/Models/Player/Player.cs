using System.Collections;

namespace UnfrozenTestWork
{
    public class Player : PlayerBase
    {
        public new IEnumerator TakeTurn()
        {
            yield return base.TakeTurn();
        }
    }
}