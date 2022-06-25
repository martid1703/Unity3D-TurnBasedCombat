using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class UnitPositioner : SingletonBase<UnitPositioner>
    {
        private const string _overviewLayer = "UnitsOverview";
        private const string _battleLayer = "UnitsBattle";

        private readonly UnitScaler _unitScaler;
        private UnitPositionResult _playerUnitPositions;
        private UnitPositionResult _enemyUnitPositions;

        private readonly float _offsetX;

        public UnitPositioner()
        {
            _offsetX = 0f;
            _unitScaler = new UnitScaler();
        }
        public UnitPositioner(float offsetX)
        {
            _offsetX = offsetX;
            _unitScaler = new UnitScaler();
        }

        public void PositionUnitsOverview(Unit[] playerUnits, Unit[] enemyUnits, Rect fitInto)
        {
            var playerFitInto = GetPlayerFitInto(fitInto);
            _playerUnitPositions = TransformUnits(playerUnits, playerFitInto);
            ChangeSortingLayer(playerUnits, _overviewLayer);

            var enmemyFitInto = GetEnemyFitInto(fitInto);
            _enemyUnitPositions = TransformUnits(enemyUnits, enmemyFitInto);
            ChangeSortingLayer(enemyUnits, _overviewLayer);
        }

        public void PositionUnitsBattle(Unit[] playerUnits, Unit[] enemyUnits, Rect fitInto)
        {
            var playerFitInto = GetPlayerFitInto(fitInto);
            TransformUnits(playerUnits, playerFitInto);
            ChangeSortingLayer(playerUnits, _battleLayer);

            var enmemyFitInto = GetEnemyFitInto(fitInto);
            TransformUnits(enemyUnits, enmemyFitInto);
            ChangeSortingLayer(enemyUnits, _battleLayer);
        }

        private static Rect GetPlayerFitInto(Rect fitInto)
        {
            return new Rect(
                 fitInto.position.x,
                 fitInto.position.y,
                 fitInto.width / 2,
                 fitInto.height);
        }

        private static Rect GetEnemyFitInto(Rect fitInto)
        {
            return new Rect(
                 fitInto.position.x + fitInto.width / 2,
                 fitInto.position.y,
                 fitInto.width / 2,
                 fitInto.height);
        }

        public void ReturnToOverview(Unit attackingUnit, Unit attackedUnit)
        {
            if (_playerUnitPositions == null || _enemyUnitPositions == null)
            {
                Debug.LogWarning("Cannot return unit to overview, because initial positions are not set.");
                return;
            }

            RevertUnit(attackingUnit);
            RevertUnit(attackedUnit);
        }

        private UnitPositionResult TransformUnits(Unit[] units, Rect fitInto)
        {
            Dictionary<Unit, UnitTransform> unitPositionResult = GetUnitPositionResult(units, fitInto);
            var unitsRect = GetUnitsRect(unitPositionResult);
            _unitScaler.ScaleUnits(units, unitsRect, fitInto);
            unitPositionResult = GetUnitPositionResult(units, fitInto);
            PlaceUnits(units, unitPositionResult);
            return new UnitPositionResult(unitPositionResult, unitsRect);
        }

        private void RevertUnit(Unit unit)
        {
            var unitTransform = unit.UnitData.Type == UnitType.Player ? _playerUnitPositions.UnitTransforms[unit] : _enemyUnitPositions.UnitTransforms[unit];
            unit.transform.position = unitTransform.Position;
            unit.transform.localScale = unitTransform.Scale;
            ChangeSortingLayer(new Unit[] { unit }, "UnitsBattle");
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

        private Dictionary<Unit, UnitTransform> GetUnitPositionResult(Unit[] units, Rect fitInto)
        {
            var unitsTransforms = new Dictionary<Unit, UnitTransform>();

            float lastUnitPositionX = units[0].UnitData.Type == UnitType.Player ? fitInto.center.x + fitInto.width / 2 : fitInto.center.x - fitInto.width / 2;

            for (int i = 0; i < units.Length; i++)
            {
                float totalOffsetX;
                var unitBoxCollider = units[i].GetComponentInChildren<BoxCollider2D>();
                if (i == 0)
                {
                    totalOffsetX = unitBoxCollider.size.x / 2 + _offsetX;
                }
                else
                {
                    var prevUnitSize = units[i - 1].GetComponentInChildren<BoxCollider2D>().size;
                    totalOffsetX = unitBoxCollider.size.x / 2 + prevUnitSize.x / 2 + _offsetX;
                }

                totalOffsetX = units[i].UnitData.Type == UnitType.Player ? -totalOffsetX : totalOffsetX;

                float targetRectBottom = fitInto.center.y - fitInto.height / 2;
                Vector3 position = new Vector3(lastUnitPositionX + totalOffsetX, targetRectBottom);

                // box collider size and offset values are not affected by unit current scale, so need to adjust
                var unitPosition = units[i].transform.position;
                var boxColliderCenter = targetRectBottom + unitBoxCollider.offset.y;
                float boxColliderBottom = boxColliderCenter - unitBoxCollider.size.y / 2;
                float deltaY = (boxColliderBottom - targetRectBottom) * units[i].transform.localScale.y;

                var newPosition = new Vector3(position.x, position.y + deltaY);

                unitsTransforms.Add(units[i], new UnitTransform(position, units[i].transform.localScale));
                lastUnitPositionX = position.x;
            }
            return unitsTransforms;
        }

        private Rect GetUnitsRect(Dictionary<Unit, UnitTransform> unitPositionResult)
        {
            var firstUnit = unitPositionResult.Keys.First();
            var lastUnit = unitPositionResult.Keys.Last();

            var dist = unitPositionResult[firstUnit].Position - unitPositionResult[lastUnit].Position;
            Vector3 startPosition = dist / 2;

            var firstUnitSize = firstUnit.GetComponentInChildren<BoxCollider2D>().size.x;
            var lastUnitSize = lastUnit.GetComponentInChildren<BoxCollider2D>().size.x;

            var unitRectWidth = dist.x + firstUnitSize / 2 + lastUnitSize / 2 + _offsetX;

            var heighestUnit = FindHighestUnit(unitPositionResult.Keys.ToArray());

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

        private void PlaceUnits(Unit[] units, Dictionary<Unit, UnitTransform> unitsTransforms)
        {
            for (int i = 0; i < units.Length; i++)
            {
                units[i].transform.position = unitsTransforms[units[i]].Position;
            }
        }
    }
}
