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
        private float _moveSpeed = 5f;

        private readonly EventHandler _unitSelected;
        private readonly Func<Unit, bool> _isUnitSelectable;

        public UnitSpawner(
            Transform[] playerUnitPrefabs,
            Transform[] enemyUnitPrefabs,
            Transform overviewSpace,
            EventHandler unitSelected,
            Transform unitsContainer,
            Func<Unit, bool> isUnitSelectable)
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
            var rnd = new Random();

            var startPosition = GetStartPosition();
            var playerUnits = SpawnPlayerUnits(startPosition, rnd);
            var enemyUnits = SpawnEnemyUnits(startPosition, rnd);

            return new UnitSpawnerResult(playerUnits, enemyUnits, new Unit[0]);
        }

        private Vector3 GetStartPosition()
        {
            return new Vector3(_overviewSpace.position.x, _overviewSpace.position.y);
        }

        private Unit[] SpawnPlayerUnits(Vector3 startPosition, Random rnd)
        {
            var units = new List<Unit>(_playerUnitPrefabs.Length + _enemyUnitPrefabs.Length);

            for (int i = 0; i < _playerUnitPrefabs.Length; i++)
            {
                var spawnedUnit = SpawnUnit(UnitType.Player, startPosition, _playerUnitPrefabs[i], rnd);
                units.Add(spawnedUnit);
            }

            return units.ToArray();
        }

        private Unit[] SpawnEnemyUnits(Vector3 startPosition, Random rnd)
        {
            var units = new List<Unit>(_playerUnitPrefabs.Length + _enemyUnitPrefabs.Length);

            for (int i = 0; i < _enemyUnitPrefabs.Length; i++)
            {
                Unit spawnedUnit = SpawnUnit(UnitType.Enemy, startPosition, _enemyUnitPrefabs[i], rnd);
                units.Add(spawnedUnit);
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
            BattleManager.Instance.BattleSpeedChange += unit.OnBattleSpeedChange;
            return unit;
        }

        private UnitData GenerateUnitData(UnitType unitType, Random rnd)
        {
            int initiative = rnd.Next(_minInitiative, _maxInitiative);
            int health = rnd.Next(_minHealth, _maxHealth);
            int damage = rnd.Next(_minDamage, _maxDamage);

            var unitData = new UnitData(unitType, initiative, health, damage, _moveSpeed);
            return unitData;
        }
    }
}
