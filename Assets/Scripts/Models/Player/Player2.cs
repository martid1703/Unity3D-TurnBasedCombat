using System.Collections;

namespace UnfrozenTestWork
{
    public class Player2 : PlayerBase
    {
        public new IEnumerator TakeTurn()
        {
            yield return base.TakeTurn();
        }
    }
}