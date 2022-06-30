using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnfrozenTestWork
{
    public class UnitSpawner
    {
        private Transform _overviewSpace;
        private int _minInitiative = 50;
        private int _maxInitiative = 100;// 100% is max value

        private int _minHealth = 100;
        private int _maxHealth = 200;

        private int _minDamage = 50;
        private int _maxDamage = 100;
        private float _moveSpeed = 5f;

        private readonly EventHandler _unitSelected;
        private readonly Func<UnitModel, bool> _isUnitSelectable;
        private readonly Func<UnitModel, bool> _isUnitSelectableAsTarget;

        private Transform _playerUnitsContainer;
        private Transform _enemyUnitsContainer;

        public UnitSpawner(
            Transform overviewSpace,
            EventHandler unitSelected,
            Func<UnitModel, bool> isUnitSelectable,
            Func<UnitModel, bool> isUnitSelectableAsTarget,
            Transform playerUnitsContainer,
            Transform enemyUnitsContainer)
        {
            _overviewSpace = overviewSpace;
            _unitSelected = unitSelected;
            _isUnitSelectable = isUnitSelectable;
            _playerUnitsContainer = playerUnitsContainer;
            _enemyUnitsContainer = enemyUnitsContainer;
            _isUnitSelectableAsTarget = isUnitSelectableAsTarget;
        }

        public UnitSpawnerResult Spawn(IEnumerable<UnitType> playerUnits, IEnumerable<UnitType> enemyUnits)
        {
            var startPosition = GetStartPosition();
            var spawnedPlayerUnits = SpawnUnits(UnitBelonging.Player, playerUnits, startPosition);
            var spawnedEnemyUnits = SpawnUnits(UnitBelonging.Enemy, enemyUnits, startPosition);

            return new UnitSpawnerResult(spawnedPlayerUnits, spawnedEnemyUnits, new UnitModel[0]);
        }

        public void AddUnit(UnitBelonging unitBelonging, UnitType unit, List<UnitModel> units)
        {
            var startPosition = GetStartPosition();
            var spawnedUnit = SpawnUnit(unitBelonging, unit, startPosition);
            units.Add(spawnedUnit);
        }

        private Vector3 GetStartPosition()
        {
            return new Vector3(_overviewSpace.position.x, _overviewSpace.position.y);
        }

        private UnitModel[] SpawnUnits(UnitBelonging unitBelonging, IEnumerable<UnitType> units, Vector3 startPosition)
        {
            var spawnedUnits = new List<UnitModel>();
            foreach (var unit in units)
            {
                var spawnedUnit = SpawnUnit(unitBelonging, unit, startPosition);
                spawnedUnits.Add(spawnedUnit);
            }

            return spawnedUnits.ToArray();
        }

        private UnitModel SpawnUnit(UnitBelonging unitBelonging, UnitType unit, Vector3 position)
        {
            UnitModel unitPrefab = UnitModelProvider.Instance.GetUnitPrefab(unit);

            if (unitPrefab == null)
            {
                throw new Exception($"Cannot find prefab for unit type - {unit}.");
            }

            var unitParent = unitBelonging == UnitBelonging.Player ? _playerUnitsContainer : _enemyUnitsContainer;

            var spawnedUnit = GameObject.Instantiate(
                unitPrefab,
                position,
                Quaternion.identity,
                unitParent);

            UnitData unitData = GenerateUnitData(unitBelonging, unit);
            spawnedUnit.Initialize(unitData);
            spawnedUnit.UnitSelected += _unitSelected;
            spawnedUnit.IsUnitSelectable = _isUnitSelectable;
            spawnedUnit.IsUnitSelectableAsTarget = _isUnitSelectableAsTarget;
            BattleManager.Instance.BattleSpeedChange += spawnedUnit.OnBattleSpeedChange;
            return spawnedUnit;
        }

        private UnitData GenerateUnitData(UnitBelonging unitBelonging, UnitType unitType)
        {
            int initiative = Random.Range(_minInitiative, _maxInitiative);
            int health = Random.Range(_minHealth, _maxHealth);
            int damage = Random.Range(_minDamage, _maxDamage);

            var unitData = new UnitData(unitBelonging, unitType, initiative, health, damage, _moveSpeed);
            return unitData;
        }
    }
}
