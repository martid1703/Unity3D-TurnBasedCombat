using System;

namespace UnfrozenTestWork
{
    public struct UnitData
    {
        public UnitType Type { get; private set; }
        public int Initiative { get; private set; }
        public int Health { get; private set; }
        public int Damage { get; private set; }

        public UnitData(UnitType type, int initiative, int health, int damage)
        {
            Type = type;
            Initiative = initiative;
            Health = health;
            Damage = damage;
        }

        public UnitData TakeDamage(int damage)
        {
            Health -= damage;
            return new UnitData(Type, Initiative, Health, Damage);
        }
    }
}
