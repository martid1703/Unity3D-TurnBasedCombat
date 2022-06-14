using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace UnfrozenTestWork
{
    public class UnitSpawner
    {
        private Transform[] _playerUnitPrefabs;
        private Transform[] _enemyUnitPrefabs;
        private Transform _overviewSpace;
        private Transform _unitsContainer;

        private int _minInitiative = 50;
        private int _maxInitiative = 100;// 100% is max value

        private int _minHealth = 100;
        private int _maxHealth = 200;

        private int _minDamage = 50;
        private int _maxDamage = 100;

        private EventHandler _unitSelected;
        private Func<Unit, bool> _isUnitSelectable;

        public UnitSpawner(
            Transform[] playerUnitPrefabs,
            Transform[] enemyUnitPrefabs,
            Transform overviewSpace,
            EventHandler unitSelected,
            Transform unitsContainer,
            Func<Unit, bool> isUnitSelectable
        )
        {
            _playerUnitPrefabs = playerUnitPrefabs;
            _enemyUnitPrefabs = enemyUnitPrefabs;
            _overviewSpace = overviewSpace;
            _unitSelected = unitSelected;
            _unitsContainer = unitsContainer;
            _isUnitSelectable = isUnitSelectable;
        }

        public UnitSpawnerResult Spawn()
        {
            var unitSize = _playerUnitPrefabs[0].GetComponentInChildren<BoxCollider2D>().size;
            float offset = unitSize.x * 1.5f;
            var rnd = new Random();

            var startPosition = GetStartPosition();

            var playerUnits = SpawnPlayerUnits(startPosition, rnd);
            var enemyUnits = SpawnEnemyUnits(startPosition, rnd);

            Unit[] allUnits = new Unit[playerUnits.Length + enemyUnits.Length];
            Array.Copy(playerUnits, allUnits, playerUnits.Length);
            Array.Copy(enemyUnits, 0, allUnits, playerUnits.Length, enemyUnits.Length);

            ScaleUnits(allUnits, offset);

            PositionUnits(startPosition, playerUnits, enemyUnits);

            return new UnitSpawnerResult(playerUnits, enemyUnits, new Unit[0]);
        }

        private void PositionUnits(
            Vector3 startPosition,
            Unit[] playerUnits,
            Unit[] enemyUnits)
        {
            var newUnitSize = playerUnits[0].GetComponentInChildren<BoxCollider2D>().bounds.size;
            var newOffsetX = newUnitSize.x * 1.5f;
            var newOffsetY = newUnitSize.y / 2;

            Vector3[] playerUnitPositions = GetPositions(
                playerUnits,
                startPosition,
                -newOffsetX,
                newOffsetY
            );
            Vector3[] enemyUnitPositions = GetPositions(
                enemyUnits,
                startPosition,
                newOffsetX,
                newOffsetY
            );

            PositionUnits(playerUnits, playerUnitPositions);
            PositionUnits(enemyUnits, enemyUnitPositions);
        }

        private Vector3 GetStartPosition()
        {
            return new Vector3(_overviewSpace.position.x, _overviewSpace.position.y);
        }

        private void ScaleUnits(Unit[] units, float offset)
        {
            var rect = _overviewSpace.GetComponent<RectTransform>().rect;
            var fitInto = new Rect(rect.center.x, rect.center.y, rect.size.x, rect.size.y);
            var unitsRect = GetUnitsRect(units, fitInto, offset);

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

        private static Rect GetUnitsRect(Unit[] units, Rect fitInto, float offset)
        {
            var boxCollider = units[0].GetComponentInChildren<BoxCollider2D>();
            float unitRectHeight = boxCollider.bounds.size.y;
            var unitRectWidth = offset * units.Length + boxCollider.bounds.size.x;
            Rect unitsRect = new Rect(
                fitInto.position.x,
                fitInto.position.y,
                unitRectWidth,
                unitRectHeight
            );
            return unitsRect;
        }

        private static Vector3[] GetPositions(
            Unit[] units,
            Vector3 startPosition,
            float offsetX,
            float offsetY
        )
        {
            var boxCollider = units[0].GetComponentInChildren<BoxCollider2D>();
            float unitRectHeight = boxCollider.bounds.size.y;
            var unitsPositions = new Vector3[units.Length];
            for (int i = 0; i < units.Length; i++)
            {
                Vector3 position = new Vector3(startPosition.x + offsetX, -unitRectHeight / 2);
                unitsPositions[i] = position;
                startPosition = position;
            }
            return unitsPositions;
        }

        private void PositionUnits(Unit[] units, Vector3[] positions)
        {
            for (int i = 0; i < units.Length; i++)
            {
                units[i].transform.position = positions[i];
            }
        }

        private Unit[] SpawnPlayerUnits(Vector3 startPosition, Random rnd)
        {
            var units = new List<Unit>(_playerUnitPrefabs.Length + _enemyUnitPrefabs.Length);

            for (int i = 0; i < _playerUnitPrefabs.Length; i++)
            {
                units.Add(SpawnUnit(UnitType.Player, startPosition, _playerUnitPrefabs[i], rnd));
            }

            return units.ToArray();
        }

        private Unit[] SpawnEnemyUnits(Vector3 startPosition, Random rnd)
        {
            var units = new List<Unit>(_playerUnitPrefabs.Length + _enemyUnitPrefabs.Length);

            for (int i = 0; i < _enemyUnitPrefabs.Length; i++)
            {
                units.Add(SpawnUnit(UnitType.Enemy, startPosition, _enemyUnitPrefabs[i], rnd));
            }

            return units.ToArray();
        }

        private Unit SpawnUnit(
            UnitType unitType,
            Vector3 position,
            Transform unitPrefab,
            Random rnd
        )
        {
            var spawnedUnit = GameObject.Instantiate(
                unitPrefab,
                position,
                Quaternion.identity,
                _unitsContainer
            );
            var unit = spawnedUnit.gameObject.GetComponentInChildren<Unit>();
            UnitData unitData = GenerateUnitData(unitType, rnd);
            unit.Initialize(unitData);
            unit.UnitSelected += _unitSelected;
            unit.IsUnitSelectable = _isUnitSelectable;
            return unit;
        }

        private UnitData GenerateUnitData(UnitType unitType, Random rnd)
        {
            int initiative = rnd.Next(_minInitiative, _maxInitiative);
            int health = rnd.Next(_minHealth, _maxHealth);
            int damage = rnd.Next(_minDamage, _maxDamage);

            var unitData = new UnitData(unitType, initiative, health, damage);
            return unitData;
        }
    }
}
