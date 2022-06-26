using System;
using UnityEngine;

namespace UnfrozenTestWork
{
    [Serializable]
    public class UnitReference
    {
        [SerializeField]
        public UnitType UnitType;

        [SerializeField]
        public UnitModel UnitPrefab;
    }
}
