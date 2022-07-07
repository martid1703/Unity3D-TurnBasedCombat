using System.Collections;

namespace UnfrozenTestWork
{
    public class Player1 : PlayerBase
    {
        public new IEnumerator TakeTurn()
        {
            yield return base.TakeTurn();
        }
    }
}