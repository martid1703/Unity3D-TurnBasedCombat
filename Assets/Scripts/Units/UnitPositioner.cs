using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class UnitPositioner : SingletonBase<UnitPositioner>
    {
        private const string _overviewLayer = "UnitsOverview";
        private const string _battleLayer = "UnitsBattle";

        private readonly UnitScaler _unitScaler;

        // need to keep these field unchanged for revert unit position
        private static UnitPositionResult _unitPositionResult;

        private float _spaceBetweenUnits;

        public UnitPositioner()
        {
            _unitScaler = new UnitScaler();
        }

        public void PositionUnitsOverview(UnitModel[] playerUnits, UnitModel[] enemyUnits, Rect fitInto, float spaceBetweenUnits = 0f)
        {
            _spaceBetweenUnits = spaceBetweenUnits;
            _unitPositionResult = PerformPositioning(playerUnits, enemyUnits, fitInto, _overviewLayer);
        }

        public void PositionUnitsBattle(UnitModel[] playerUnits, UnitModel[] enemyUnits, Rect fitInto, float spaceBetweenUnits = 2f)
        {
            _spaceBetweenUnits = spaceBetweenUnits;
            PerformPositioning(playerUnits, enemyUnits, fitInto, _battleLayer);
        }

        public void RevertUnitsBack(UnitModel attackingUnit, UnitModel attackedUnit)
        {
            if (_unitPositionResult == null)
            {
                Debug.LogWarning("Cannot return unit to overview, because initial positions are not set.");
                return;
            }

            RevertUnit(attackingUnit);
            RevertUnit(attackedUnit);
        }

        private void RevertUnit(UnitModel unit)
        {
            UnitTransform unitTransform;
            switch (unit.UnitData.Belonging)
            {
                case UnitBelonging.Player:
                    unitTransform = _unitPositionResult.PlayerUnitTransforms[unit];
                    break;
                case UnitBelonging.Enemy:
                    unitTransform = _unitPositionResult.EnemyUnitTransforms[unit];
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unit.UnitData.Belonging));
            }

            unit.transform.position = unitTransform.Position;
            unit.transform.localScale = unitTransform.Scale;
            ChangeSortingLayer(new UnitModel[] { unit }, _overviewLayer);
        }

        private UnitPositionResult PerformPositioning(UnitModel[] playerUnits, UnitModel[] enemyUnits, Rect fitInto, string unitLayer)
        {
            var playerFitInto = GetPlayerFitInto(fitInto);
            var enemyFitInto = GetEnemyFitInto(fitInto);

            Scale(playerUnits, enemyUnits, playerFitInto, enemyFitInto);
            UnitPositionResult unitPositionResult = PlaceUnits(playerUnits, enemyUnits, playerFitInto, enemyFitInto, unitLayer);
            return unitPositionResult;
        }

        private UnitPositionResult PlaceUnits(UnitModel[] playerUnits, UnitModel[] enemyUnits, Rect playerFitInto, Rect enemyFitInto, string layerName)
        {
            Dictionary<UnitModel, UnitTransform> PlayerUnitTransforms = PlaceUnits(playerUnits, playerFitInto);
            ChangeSortingLayer(playerUnits, layerName);

            Dictionary<UnitModel, UnitTransform> EnemyUnitTransforms = PlaceUnits(enemyUnits, enemyFitInto);
            ChangeSortingLayer(enemyUnits, layerName);

            return new UnitPositionResult(PlayerUnitTransforms, EnemyUnitTransforms);
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

        private void Scale(UnitModel[] playerUnits, UnitModel[] enemyUnits, Rect playerFitInto, Rect enemyFitInto)
        {
            Dictionary<UnitModel, UnitTransform> playerUnitPositionResult = GetUnitPositionResult(playerUnits, playerFitInto);
            Dictionary<UnitModel, UnitTransform> enemyPositionResult = GetUnitPositionResult(enemyUnits, enemyFitInto);

            float smallestScale = _unitScaler.FindSmallestScale(playerUnitPositionResult, enemyPositionResult, playerFitInto, enemyFitInto, _spaceBetweenUnits);
            _unitScaler.ScaleUnits(playerUnits, smallestScale);
            _unitScaler.ScaleUnits(enemyUnits, smallestScale);
        }

        private void ChangeSortingLayer(UnitModel[] units, string layerName)
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

        private Dictionary<UnitModel, UnitTransform> GetUnitPositionResult(UnitModel[] units, Rect fitInto)
        {
            var unitsTransforms = new Dictionary<UnitModel, UnitTransform>();

            float lastUnitPositionX = units[0].UnitData.Belonging == UnitBelonging.Player ? fitInto.center.x + fitInto.width / 2 : fitInto.center.x - fitInto.width / 2;

            for (int i = 0; i < units.Length; i++)
            {
                float totalOffsetX;
                var unitBoxCollider = units[i].GetComponentInChildren<BoxCollider2D>();
                if (i == 0)
                {
                    totalOffsetX = unitBoxCollider.size.x / 2 + _spaceBetweenUnits;
                }
                else
                {
                    var prevUnitSize = units[i - 1].GetComponentInChildren<BoxCollider2D>().size;
                    totalOffsetX = unitBoxCollider.size.x / 2 + prevUnitSize.x / 2 + _spaceBetweenUnits;
                }

                totalOffsetX = units[i].UnitData.Belonging == UnitBelonging.Player ? -totalOffsetX : totalOffsetX;

                // box collider size and offset values are not affected by unit current scale, so need to adjust
                totalOffsetX *= units[i].transform.localScale.x;
                float deltaY = (unitBoxCollider.offset.y - unitBoxCollider.size.y / 2) * units[i].transform.localScale.y;

                float targetRectBottom = fitInto.center.y - fitInto.height / 2;
                Vector3 position = new Vector3(lastUnitPositionX + totalOffsetX, targetRectBottom - deltaY);

                unitsTransforms.Add(units[i], new UnitTransform(position, units[i].transform.localScale));
                lastUnitPositionX = position.x;
            }
            return unitsTransforms;
        }

        private Dictionary<UnitModel, UnitTransform> PlaceUnits(UnitModel[] units, Rect fitInto)
        {
            Dictionary<UnitModel, UnitTransform> unitPositionResult = GetUnitPositionResult(units, fitInto);
            for (int i = 0; i < units.Length; i++)
            {
                units[i].transform.position = unitPositionResult[units[i]].Position;
            }
            return unitPositionResult;
        }
    }
}
