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

        private float _spaceBetweenUnits;

        public UnitPositioner()
        {
            _unitScaler = new UnitScaler();
        }

        public void PositionUnitsOverview(UnitModel[] playerUnits, UnitModel[] enemyUnits, Rect fitInto, float spaceBetweenUnits = 0f)
        {
            _spaceBetweenUnits = spaceBetweenUnits;

            var playerFitInto = GetPlayerFitInto(fitInto);
            _playerUnitPositions = TransformUnits(playerUnits, playerFitInto);
            ChangeSortingLayer(playerUnits, _overviewLayer);

            var enmemyFitInto = GetEnemyFitInto(fitInto);
            _enemyUnitPositions = TransformUnits(enemyUnits, enmemyFitInto);
            ChangeSortingLayer(enemyUnits, _overviewLayer);
        }

        public void PositionUnitsBattle(UnitModel[] playerUnits, UnitModel[] enemyUnits, Rect fitInto, float spaceBetweenUnits = 2f)
        {
            _spaceBetweenUnits = spaceBetweenUnits;

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

        public void ReturnUnitsBack(UnitModel attackingUnit, UnitModel attackedUnit)
        {
            if (_playerUnitPositions == null || _enemyUnitPositions == null)
            {
                Debug.LogWarning("Cannot return unit to overview, because initial positions are not set.");
                return;
            }

            RevertUnit(attackingUnit);
            RevertUnit(attackedUnit);
        }

        private void RevertUnit(UnitModel unit)
        {
            var unitTransform = unit.UnitData.Belonging == UnitBelonging.Player ? _playerUnitPositions.UnitTransforms[unit] : _enemyUnitPositions.UnitTransforms[unit];
            unit.transform.position = unitTransform.Position;
            unit.transform.localScale = unitTransform.Scale;
            ChangeSortingLayer(new UnitModel[] { unit }, _overviewLayer);
        }

        private UnitPositionResult TransformUnits(UnitModel[] units, Rect fitInto)
        {
            Dictionary<UnitModel, UnitTransform> unitPositionResult = GetUnitPositionResult(units, fitInto);
            var unitsRect = GetUnitsRect(unitPositionResult);
            _unitScaler.ScaleUnits(units, unitsRect, fitInto);
            unitPositionResult = GetUnitPositionResult(units, fitInto);
            PlaceUnits(units, unitPositionResult);
            return new UnitPositionResult(unitPositionResult, unitsRect);
        }

        public void ChangeSortingLayer(UnitModel[] units, string layerName)
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
                //var newPosition = new Vector3(position.x, position.y - deltaY);

                unitsTransforms.Add(units[i], new UnitTransform(position, units[i].transform.localScale));
                lastUnitPositionX = position.x;
            }
            return unitsTransforms;
        }

        private Rect GetUnitsRect(Dictionary<UnitModel, UnitTransform> unitPositionResult)
        {
            var width = FindWidth(unitPositionResult.Keys.ToArray());
            var height = FindHight(unitPositionResult.Keys.ToArray());

            Rect unitsRect = new Rect(
                0,
                0,
                width,
                height
            );
            return unitsRect;
        }

        private float FindWidth(UnitModel[] units)
        {
            float width = 0;
            for (int i = 0; i < units.Length; i++)
            {
                width += units[i].GetComponent<BoxCollider2D>().size.x + _spaceBetweenUnits;
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

        private void PlaceUnits(UnitModel[] units, Dictionary<UnitModel, UnitTransform> unitsTransforms)
        {
            for (int i = 0; i < units.Length; i++)
            {
                units[i].transform.position = unitsTransforms[units[i]].Position;
            }
        }
    }
}
