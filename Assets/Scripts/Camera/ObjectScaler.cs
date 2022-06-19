using UnityEngine;

namespace UnfrozenTestWork
{
    public class ObjectScaler : SingletonBase<ObjectScaler>
    {
        public float FindScaleCoefficientBetweenRectangles(Rect scaleWhat, Rect scaleTo)
        {
            float scaleX = scaleWhat.width / scaleTo.width;
            float scaleY = scaleWhat.height / scaleTo.height;

            // object is smaller that target on both axis, select max to fit bigger size
            if (scaleX < 1 && scaleY < 1)
            {
                return Mathf.Max(scaleX, scaleY);
            }

            // object is bigger that target on both axis, select min to fit bigger size
            if (scaleX >= 1 && scaleY >= 1)
            {
                return Mathf.Max(scaleX, scaleY);
            }

            // is bigger than target at least on one axis and lesser than target on the other. We need to shrinken the object, so select min coeff, which will be < 1
            return Mathf.Max(scaleX, scaleY);
        }
    }
}