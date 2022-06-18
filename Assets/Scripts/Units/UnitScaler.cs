using System;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class UnitScaler
    {
        public void ScaleUnits(Unit[] units, Rect fitInto)
        {
            var unitsRect = GetUnitsRect(units, fitInto.position);

            float scaleX = fitInto.width < unitsRect.width ? fitInto.width / unitsRect.width : 1;
            float scaleY = fitInto.height < unitsRect.height ? fitInto.height / unitsRect.height : 1;

            for (int i = 0; i < units.Length; i++)
            {
                Vector3 unitLocalScale = units[i].transform.localScale;
                units[i].transform.localScale = new Vector3(
                    unitLocalScale.x * scaleX,
                    unitLocalScale.y * scaleY
                );
            }
        }

        private Rect GetUnitsRect(Unit[] units, Vector3 startPosition)
        {
            Unit heighestUnit = FindHighestUnit(units);

            var leftMostUnitSize = units[0].GetComponentInChildren<BoxCollider2D>().size.x;
            var rightMostUnitSize = units[units.Length - 1].GetComponentInChildren<BoxCollider2D>().size.x;
            var distanceBetweenFirstAndLastUnit = Math.Abs(units[units.Length - 1].transform.position.x - units[0].transform.position.x);

            var unitRectWidth = distanceBetweenFirstAndLastUnit + leftMostUnitSize / 2 + rightMostUnitSize / 2;
            Rect unitsRect = new Rect(
                startPosition.x,
                startPosition.y,
                unitRectWidth,
                heighestUnit.GetComponent<BoxCollider2D>().size.y
            );
            return unitsRect;
        }

        private Unit FindHighestUnit(Unit[] units)
        {
            Unit biggestUnit = units[0];

            for (int i = 1; i < units.Length; i++)
            {
                var unitBox = units[i].GetComponent<BoxCollider2D>();
                var biggestUnitBox = biggestUnit.GetComponent<BoxCollider2D>();
                if (unitBox.size.y > biggestUnitBox.size.y)
                {
                    biggestUnit = units[i];
                }
            }
            return biggestUnit;
        }
    }
}
