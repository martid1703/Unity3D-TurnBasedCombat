using System.Collections;

namespace UnfrozenTestWork
{
    public class Enemy : PlayerBase
    {
        public new IEnumerator TakeTurn()
        {
            yield return base.TakeTurn();
        }
    }
}