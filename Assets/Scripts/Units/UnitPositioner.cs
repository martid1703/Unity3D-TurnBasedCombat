using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class UnitPositioner
    {
        private const string _overviewLayer = "UnitsOverview";
        private const string _battleLayer = "UnitsBattle";

        private readonly UnitScaler _unitScaler;

        private UnitPositionResult _unitPositionResult;

        private float _spaceBetweenUnits;

        public UnitPositioner()
        {
            _unitScaler = new UnitScaler();
        }

        public void PositionUnitsOverview(UnitModel[] player1Units, UnitModel[] player2Units, Rect fitInto, float spaceBetweenUnits = 0f)
        {
            _spaceBetweenUnits = spaceBetweenUnits;
            _unitPositionResult = PerformPositioning(player1Units, player2Units, fitInto, _overviewLayer);
        }

        public void PositionUnitsBattle(UnitModel[] player1Units, UnitModel[] player2Units, Rect fitInto, float spaceBetweenUnits = 2f)
        {
            _spaceBetweenUnits = spaceBetweenUnits;
            PerformPositioning(player1Units, player2Units, fitInto, _battleLayer);
        }

        public void RevertUnitsBack(UnitModel attackingUnit, UnitModel attackedUnit)
        {
            if (_unitPositionResult == null)
            {
                Debug.LogWarning("Cannot revert units, because initial positions are not set.");
                return;
            }

            RevertUnit(attackingUnit);
            RevertUnit(attackedUnit);
        }

        private void RevertUnit(UnitModel unit)
        {
            if (unit == null)
            {
                return;
            }

            UnitTransform unitTransform;
            try
            {
                switch (unit.UnitData.Belonging)
                {
                    case UnitBelonging.Player1:
                        unitTransform = _unitPositionResult.Player1UnitTransforms[unit];
                        break;
                    case UnitBelonging.Player2:
                        unitTransform = _unitPositionResult.Player2UnitTransforms[unit];
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(unit.UnitData.Belonging));
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Cannot find unit in unit position result.");
                Debug.LogError(ex);
                throw;
            }

            unit.transform.position = unitTransform.Position;
            unit.transform.localScale = unitTransform.Scale;
            ChangeSortingLayer(new UnitModel[] { unit }, _overviewLayer);
        }

        private UnitPositionResult PerformPositioning(UnitModel[] player1Units, UnitModel[] player2Units, Rect fitInto, string unitLayer)
        {
            var player1FitInto = GetPlayer1FitInto(fitInto);
            var player2FitInto = GetPlayer2FitInto(fitInto);

            Scale(player1Units, player2Units, player1FitInto, player2FitInto);
            UnitPositionResult unitPositionResult = PlaceUnits(player1Units, player2Units, player1FitInto, player2FitInto, unitLayer);
            return unitPositionResult;
        }

        private UnitPositionResult PlaceUnits(UnitModel[] player1Units, UnitModel[] player2Units, Rect player1FitInto, Rect player2FitInto, string layerName)
        {
            Dictionary<UnitModel, UnitTransform> Player1UnitTransforms = PlaceUnits(player1Units, player1FitInto);
            ChangeSortingLayer(player1Units, layerName);

            Dictionary<UnitModel, UnitTransform> Player2UnitTransforms = PlaceUnits(player2Units, player2FitInto);
            ChangeSortingLayer(player2Units, layerName);

            return new UnitPositionResult(Player1UnitTransforms, Player2UnitTransforms);
        }

        private static Rect GetPlayer1FitInto(Rect fitInto)
        {
            return new Rect(
                 fitInto.position.x,
                 fitInto.position.y,
                 fitInto.width / 2,
                 fitInto.height);
        }

        private static Rect GetPlayer2FitInto(Rect fitInto)
        {
            return new Rect(
                 fitInto.position.x + fitInto.width / 2,
                 fitInto.position.y,
                 fitInto.width / 2,
                 fitInto.height);
        }

        private void Scale(UnitModel[] player1Units, UnitModel[] player2Units, Rect player1FitInto, Rect player2FitInto)
        {
            Dictionary<UnitModel, UnitTransform> player1UnitPositionResult;
            Dictionary<UnitModel, UnitTransform> player2PositionResult;
            try
            {
                player1UnitPositionResult = GetUnitPositionResult(player1Units, player1FitInto);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Scale: Cannot get unit position result.");
                Debug.LogError(ex);
                throw;
            }

            try
            {
                player2PositionResult = GetUnitPositionResult(player2Units, player2FitInto);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Scale: Cannot get unit position result.");
                Debug.LogError(ex);
                throw;
            }

            float smallestScale = _unitScaler.FindSmallestScale(player1UnitPositionResult, player2PositionResult, player1FitInto, player2FitInto, _spaceBetweenUnits);
            _unitScaler.ScaleUnits(player1Units, smallestScale);
            _unitScaler.ScaleUnits(player2Units, smallestScale);
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

            float lastUnitPositionX = units[0].UnitData.Belonging == UnitBelonging.Player1 ? fitInto.center.x + fitInto.width / 2 : fitInto.center.x - fitInto.width / 2;

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

                totalOffsetX = units[i].UnitData.Belonging == UnitBelonging.Player1 ? -totalOffsetX : totalOffsetX;

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
