using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnfrozenTestWork
{
    public class UnitSpawner
    {
        private readonly UnitModelProvider _unitModelProvider;
        private Transform _overviewSpace;
        private int _minInitiative = 10;
        private int _maxInitiative = 100;

        private int _minHealth = 100;
        private int _maxHealth = 200;

        private int _minDamage = 50;
        private int _maxDamage = 100;
        private float _moveSpeed = 5f;

        private readonly EventHandler _unitSelected;
        private readonly EventHandler _unitIsDead;
        private readonly EventHandler _unitOnMouseExit;

        private EventHandler<BattleSpeedEventArgs> _battleSpeedChange;
        private readonly Func<UnitModel, bool> _isUnitSelectable;
        private readonly Func<UnitModel, bool> _isUnitSelectableAsTarget;

        private Transform _playerUnitsContainer;
        private Transform _enemyUnitsContainer;

        public UnitSpawner(
            UnitModelProvider unitModelProvider,
            Transform overviewSpace,
            EventHandler unitSelected,
            EventHandler unitIsDead,
            Func<UnitModel, bool> isUnitSelectable,
            Func<UnitModel, bool> isUnitSelectableAsTarget,
            Transform playerUnitsContainer,
            Transform enemyUnitsContainer,
            EventHandler unitOnMouseExit)
        {
            _unitModelProvider = unitModelProvider;
            _overviewSpace = overviewSpace;
            _unitSelected = unitSelected;
            _unitIsDead = unitIsDead;
            _isUnitSelectable = isUnitSelectable;
            _playerUnitsContainer = playerUnitsContainer;
            _enemyUnitsContainer = enemyUnitsContainer;
            _isUnitSelectableAsTarget = isUnitSelectableAsTarget;
            _unitOnMouseExit = unitOnMouseExit;
        }

        public void OnBattleSpeedChange(object sender, BattleSpeedEventArgs args)
        {
            _battleSpeedChange?.Invoke(sender, args);
        }

        public UnitSpawnerResult Spawn(IEnumerable<UnitType> playerUnits, IEnumerable<UnitType> enemyUnits)
        {
            var startPosition = GetStartPosition();
            var spawnedPlayerUnits = SpawnUnits(UnitBelonging.Player, playerUnits, startPosition);
            var spawnedEnemyUnits = SpawnUnits(UnitBelonging.Enemy, enemyUnits, startPosition);

            return new UnitSpawnerResult(spawnedPlayerUnits, spawnedEnemyUnits, new UnitModel[0]);
        }

        public UnitModel AddUnit(UnitBelonging unitBelonging, UnitType unit, List<UnitModel> units)
        {
            var startPosition = GetStartPosition();
            var spawnedUnit = SpawnUnit(unitBelonging, unit, startPosition);
            units.Add(spawnedUnit);
            return spawnedUnit;
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
            UnitModel unitPrefab = _unitModelProvider.GetUnitPrefab(unit);

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
            spawnedUnit.UnitIsDead += _unitIsDead;
            spawnedUnit.UnitOnMouseExit += _unitOnMouseExit;
            spawnedUnit.IsUnitSelectable = _isUnitSelectable;
            spawnedUnit.IsUnitSelectableAsTarget = _isUnitSelectableAsTarget;
            _battleSpeedChange += spawnedUnit.OnBattleSpeedChange;
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
