using System;
using UnityEngine;

namespace UnfrozenTestWork
{
    [Serializable]
    public class UnitData
    {
        [SerializeField]
        private int health;

        [SerializeField]
        private int damage;

        [SerializeField]
        private float moveSpeed;

        [SerializeField]
        private int initiative;

        [SerializeField]
        private UnitType type;

        [SerializeField]
        private UnitBelonging unitBelonging;

        internal UnitBelonging Belonging { get => unitBelonging; private set => unitBelonging = value; }
        internal int Initiative { get => initiative; private set => initiative = value; }
        internal int Health { get => health; private set => health = value; }
        internal int Damage { get => damage; private set => damage = value; }
        internal float MoveSpeed { get => moveSpeed; private set => moveSpeed = value; }
        internal UnitType Type { get => type; set => type = value; }

        public UnitData(UnitBelonging belonging, UnitType unitType, int initiative, int health, int damage, float moveSpeed)
        {
            Belonging = belonging;
            Type = unitType;
            Initiative = initiative;
            Health = health;
            Damage = damage;
            MoveSpeed = moveSpeed;
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }

        public void ChangeMoveSpeed(float speed)
        {
            MoveSpeed = speed;
        }
    }
}
