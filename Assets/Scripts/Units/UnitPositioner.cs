using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class UnitPositioner : SingletonBase<UnitPositioner>
    {
        Dictionary<Unit, Vector3> _playerUnitPositions;
        Dictionary<Unit, Vector3> _enemyUnitPositions;

        public void PositionUnitsBattle(Unit[] playerUnits, Unit[] enemyUnits, RectTransform fitInto)
        {
            PositionUnits(playerUnits, enemyUnits, fitInto);
        }

        public void PositionUnitsOverview(Unit[] playerUnits, Unit[] enemyUnits, RectTransform fitInto)
        {
            if (_playerUnitPositions != null && _enemyUnitPositions != null)
            {
                RestorePositions();
                return;
            }
            PositionUnits(playerUnits, enemyUnits, fitInto);
        }

        private void RestorePositions()
        {
            foreach (var item in _playerUnitPositions)
            {
                item.Key.transform.position = item.Value;
            }
        }

        private void PositionUnits(Unit[] playerUnits, Unit[] enemyUnits, RectTransform fitInto)
        {
            Vector3 initialOffsetX = GetInitialUnitOffset(playerUnits, enemyUnits, fitInto);

            _playerUnitPositions = GetPositions(
                playerUnits,
                fitInto.position,
                initialOffsetX
            );
            PlaceUnits(playerUnits, _playerUnitPositions.Values.ToArray());

            _enemyUnitPositions = GetPositions(
                enemyUnits,
                fitInto.position,
                initialOffsetX
            );
            PlaceUnits(enemyUnits, _enemyUnitPositions.Values.ToArray());
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
            Unit playerWidestUnit = FindWidestUnit(playerUnits);
            Unit enemyWidestUnit = FindWidestUnit(enemyUnits);
            var playerBox = playerWidestUnit.GetComponent<BoxCollider2D>();
            var enemyBox = enemyWidestUnit.GetComponent<BoxCollider2D>();
            float initialOffsetX = playerBox.size.x > enemyBox.size.x ? playerBox.size.x : enemyBox.size.x;
            return new Vector3(initialOffsetX, fitInto.rect.height / 2);
        }

        private static Dictionary<Unit, Vector3> GetPositions(Unit[] units, Vector3 startPosition, Vector3 initialOffset)
        {
            var unitsPositions = new Dictionary<Unit, Vector3>();
            var lastUnitPosition = startPosition;
            for (int i = 0; i < units.Length; i++)
            {
                // use boxCollider and not meshRenderer.bounds, because some units have attack mesh included in their mesh, which makes it very big
                var boxCollider = units[i].GetComponentInChildren<BoxCollider2D>();

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
                unitsPositions.Add(units[i], position);// - diffInTransforms;
                lastUnitPosition = position;
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
