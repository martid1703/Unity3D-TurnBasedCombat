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
            float minScale = scaleX < scaleY ? scaleX : scaleY;

            for (int i = 0; i < units.Length; i++)
            {
                Vector3 unitLocalScale = units[i].transform.localScale;
                units[i].transform.localScale = new Vector3(
                    unitLocalScale.x * minScale,
                    unitLocalScale.y * minScale
                );
            }
        }

        private Rect GetUnitsRect(Unit[] units, Vector3 startPosition)
        {
            Unit heighestUnit = FindHighestUnit(units);
            Unit leftMostUnit = FindLeftMostUnit(units);
            Unit rightMostUnit = FindRightMostUnit(units);

            var leftMostUnitSize = leftMostUnit.GetComponentInChildren<BoxCollider2D>().size.x;
            var rightMostUnitSize = rightMostUnit.GetComponentInChildren<BoxCollider2D>().size.x;
            var distanceBetweenFirstAndLastUnit = Math.Abs(leftMostUnit.transform.position.x - rightMostUnit.transform.position.x);

            var unitRectWidth = distanceBetweenFirstAndLastUnit + leftMostUnitSize / 2 + rightMostUnitSize / 2;
            Rect unitsRect = new Rect(
                startPosition.x,
                startPosition.y,
                unitRectWidth,
                heighestUnit.GetComponent<BoxCollider2D>().size.y
            );
            return unitsRect;
        }

        private Unit FindLeftMostUnit(Unit[] units)
        {
            Unit unit = units[0];

            for (int i = 1; i < units.Length; i++)
            {
                if (unit.transform.position.x > units[i].transform.position.x)
                {
                    unit = units[i];
                }
            }
            return unit;
        }

        private Unit FindRightMostUnit(Unit[] units)
        {
            Unit unit = units[0];

            for (int i = 1; i < units.Length; i++)
            {
                if (unit.transform.position.x < units[i].transform.position.x)
                {
                    unit = units[i];
                }
            }
            return unit;
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
