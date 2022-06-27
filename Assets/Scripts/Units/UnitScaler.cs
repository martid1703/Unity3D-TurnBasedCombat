using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class UnitScaler
    {
        public void ScaleUnits(UnitModel[] units, float scale)
        {
            for (int i = 0; i < units.Length; i++)
            {
                units[i].transform.localScale = Vector3.one * scale;
            }
        }

        public float FindSmallestScale(
            Dictionary<UnitModel, UnitTransform> playerUnitPositionResult,
            Dictionary<UnitModel, UnitTransform> enemyPositionResult,
            Rect playerFitInto,
            Rect enmemyFitInto,
            float spaceBetweenUnits)
        {
            var playerUnitsRect = GetUnitsRect(playerUnitPositionResult, spaceBetweenUnits);
            float playerScale = GetUnitScale(playerUnitsRect, playerFitInto);

            var enemyUnitsRect = GetUnitsRect(enemyPositionResult, spaceBetweenUnits);
            float enemyScale = GetUnitScale(enemyUnitsRect, enmemyFitInto);

            float smallestScale = playerScale < enemyScale ? playerScale : enemyScale;
            return smallestScale;
        }

        private float GetUnitScale(Rect unitsRect, Rect fitInto)
        {
            float scaleX = fitInto.width < unitsRect.width ? fitInto.width / unitsRect.width : 1;
            float scaleY = fitInto.height < unitsRect.height ? fitInto.height / unitsRect.height : 1;
            float minScale = scaleX < scaleY ? scaleX : scaleY;
            return minScale;
        }



        private Rect GetUnitsRect(Dictionary<UnitModel, UnitTransform> unitPositionResult, float spaceBetweenUnits)
        {
            var width = FindWidth(unitPositionResult.Keys.ToArray(), spaceBetweenUnits);
            var height = FindHight(unitPositionResult.Keys.ToArray());

            Rect unitsRect = new Rect(
                0,
                0,
                width,
                height
            );
            return unitsRect;
        }

        private float FindWidth(UnitModel[] units, float spaceBetweenUnits)
        {
            float width = 0;
            for (int i = 0; i < units.Length; i++)
            {
                width += units[i].GetComponent<BoxCollider2D>().size.x + spaceBetweenUnits;
            }
            return width;
        }

        private float FindHight(UnitModel[] units)
        {
            float height = 0;

            for (int i = 0; i < units.Length; i++)
            {
                var unitBox = units[i].GetComponent<BoxCollider2D>();
                if (unitBox.size.y > height)
                {
                    height = unitBox.size.y;
                }
            }
            return height;
        }
    }
}
