using UnityEngine;

namespace UnfrozenTestWork
{
    public class UnitPositioner
    {
        public void PositionUnits(Unit[] playerUnits, Unit[] enemyUnits, RectTransform fitInto)
        {
            Vector3 initialOffsetX = GetInitialUnitOffset(playerUnits, enemyUnits, fitInto);

            Vector3[] playerUnitPositions = GetPositions(
                playerUnits,
                fitInto.position,
                initialOffsetX
            );
            PlaceUnits(playerUnits, playerUnitPositions);

            Vector3[] enemyUnitPositions = GetPositions(
                enemyUnits,
                fitInto.position,
                initialOffsetX
            );
            PlaceUnits(enemyUnits, enemyUnitPositions);
        }

        public void ChangeSortingLayer(Unit[] units, string layerName)
        {
            for (int i = 0; i < units.Length; i++)
            {
                units[i].GetComponentInChildren<Renderer>().sortingLayerID = SortingLayer.NameToID(layerName);
            }
        }

        private Vector3 GetInitialUnitOffset(Unit[] playerUnits, Unit[] enemyUnits, RectTransform fitInto)
        {
            Unit playerBiggestUnit = FindWidestUnit(playerUnits);
            Unit enemyBiggestUnit = FindWidestUnit(enemyUnits);
            var playerBox = playerBiggestUnit.GetComponent<BoxCollider2D>();
            var enemyBox = enemyBiggestUnit.GetComponent<BoxCollider2D>();
            float initialOffsetX = playerBox.size.x > enemyBox.size.x ? playerBox.size.x : enemyBox.size.x;
            return new Vector3(initialOffsetX, fitInto.rect.height / 2);
        }

        private static Vector3[] GetPositions(Unit[] units, Vector3 startPosition, Vector3 initialOffset)
        {
            var unitsPositions = new Vector3[units.Length];
            var lastUnitPosition = startPosition;
            for (int i = 0; i < units.Length; i++)
            {
                // use boxCollider and not meshRenderer.bounds, because some units have attack mesh included in their mesh, which makes it very big
                var boxCollider = units[i].GetComponentInChildren<BoxCollider2D>();
                Vector3 diffInTransforms = boxCollider.transform.position - units[i].transform.position;
                float unitRectHeight = boxCollider.bounds.size.y;

                float offsetX;
                if (i == 0) // first unit offset is fixed no matter what
                {
                    offsetX = initialOffset.x;
                }
                else
                {
                    offsetX = boxCollider.bounds.size.x;
                }

                offsetX = units[i].UnitData.Type == UnitType.Player ? offsetX * (-1) : offsetX;
                Vector3 position = new Vector3(lastUnitPosition.x + offsetX, startPosition.y - initialOffset.y);
                unitsPositions[i] = position;// - diffInTransforms;
                lastUnitPosition = unitsPositions[i];
            }
            return unitsPositions;
        }

        private void PlaceUnits(Unit[] units, Vector3[] positions)
        {
            for (int i = 0; i < units.Length; i++)
            {
                units[i].transform.position = positions[i];
            }
        }

        private Unit FindWidestUnit(Unit[] units)
        {
            Unit biggestUnit = units[0];

            for (int i = 1; i < units.Length; i++)
            {
                var unitBox = units[i].GetComponent<BoxCollider2D>();
                var biggestUnitBox = biggestUnit.GetComponent<BoxCollider2D>();
                if (unitBox.size.x > biggestUnitBox.size.x)
                {
                    biggestUnit = units[i];
                }
            }
            return biggestUnit;
        }
    }
}
