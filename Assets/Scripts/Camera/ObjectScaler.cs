using UnityEngine;

namespace UnfrozenTestWork
{
    public class ObjectScaler : SingletonBase<ObjectScaler>
    {
        public float GetScaleCoefficient(Transform objToScale, Transform scaleTo)
        {
            var scaleWhatRect = objToScale.GetComponent<RectTransform>().rect;
            var scaleToRect = scaleTo.GetComponent<RectTransform>().rect;

            return ScaleTwoRectangles(objToScale.transform, scaleWhatRect, scaleToRect);
        }

        private float ScaleTwoRectangles(Transform objToScale, Rect scaleWhatRect, Rect scaleToRect)
        {
            float scale = FindScaleCoefficientBetweenRectangles(scaleWhatRect, scaleToRect);
            return scale;
        }

        private float FindScaleCoefficientBetweenRectangles(Rect scaleWhat, Rect scaleTo)
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