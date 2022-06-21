using System;
using UnityEngine;

namespace UnfrozenTestWork
{
    [Serializable]
    public class UnitData
    {
        [SerializeField] private UnitType type;
        [SerializeField] private int initiative;
        [SerializeField] private int health;
        [SerializeField] private int damage;
        [SerializeField] private float moveSpeed;

        internal UnitType Type { get => type; private set => type = value; }
        internal int Initiative { get => initiative; private set => initiative = value; }
        internal int Health { get => health; private set => health = value; }
        internal int Damage { get => damage; private set => damage = value; }
        internal float MoveSpeed { get => moveSpeed; private set => moveSpeed = value; }


        public UnitData(UnitType type, int initiative, int health, int damage, float moveSpeed)
        {
            Type = type;
            Initiative = initiative;
            Health = health;
            Damage = damage;
            MoveSpeed = moveSpeed;
        }

        // public UnitData TakeDamage(int damage)
        // {
        //     Health -= damage;
        //     return new UnitData(Type, Initiative, Health, Damage, MoveSpeed);
        // }

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
