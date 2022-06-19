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
            Position(playerUnits, enemyUnits, fitInto, out var playerUnitPositions, out var enemyUnitPositions);
        }

        public void PositionUnitsOverview(Unit[] playerUnits, Unit[] enemyUnits, RectTransform fitInto)
        {
            Position(playerUnits, enemyUnits, fitInto, out var playerUnitPositions, out var enemyUnitPositions);

            _playerUnitPositions = playerUnitPositions;
            _enemyUnitPositions = enemyUnitPositions;
        }

        public void RestoreUnitsPositions(Unit attackingUnit, Unit attackedUnit)
        {
            if (_playerUnitPositions == null || _enemyUnitPositions == null)
            {
                return;
            }

            var position = attackingUnit.UnitData.Type == UnitType.Player ? _playerUnitPositions[attackingUnit] : _enemyUnitPositions[attackingUnit];
            attackingUnit.transform.position = position;

            position = attackedUnit.UnitData.Type == UnitType.Player ? _playerUnitPositions[attackedUnit] : _enemyUnitPositions[attackedUnit];
            attackedUnit.transform.position = position;
        }

        private void Position(Unit[] playerUnits, Unit[] enemyUnits, RectTransform fitInto, out Dictionary<Unit, Vector3> playerUnitPositions, out Dictionary<Unit, Vector3> enemyUnitPositions)
        {
            Vector3 initialOffsetX = GetInitialUnitOffset(playerUnits, enemyUnits, fitInto);

            playerUnitPositions = GetPositions(
                playerUnits,
                fitInto.position,
                initialOffsetX
            );
            PlaceUnits(playerUnits, playerUnitPositions);

            enemyUnitPositions = GetPositions(
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
                var renderers = units[i].GetComponentsInChildren<Renderer>();
                for (int j = 0; j < renderers.Length; j++)
                {
                    renderers[j].sortingLayerID = SortingLayer.NameToID(layerName);
                }
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
                float offsetX;
                if (i == 0)
                {
                    offsetX = initialOffset.x;
                }
                else
                {
                    var boxCollider = units[i].GetComponentInChildren<BoxCollider2D>();
                    var boxColliderPrevious = units[i - 1].GetComponentInChildren<BoxCollider2D>();
                    offsetX = boxColliderPrevious.bounds.size.x / 2 + boxCollider.bounds.size.x / 2f;
                }

                offsetX = units[i].UnitData.Type == UnitType.Player ? -offsetX : offsetX;
                Vector3 position = new Vector3(lastUnitPosition.x + offsetX, startPosition.y - initialOffset.y);
                unitsPositions.Add(units[i], position);
                lastUnitPosition = position;
            }
            return unitsPositions;
        }

        private void PlaceUnits(Unit[] units, Dictionary<Unit, Vector3> positions)
        {
            for (int i = 0; i < units.Length; i++)
            {
                units[i].transform.position = positions[units[i]];
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
