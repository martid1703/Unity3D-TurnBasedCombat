using System;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class InitiativeBarController : MonoBehaviour
    {
        private Transform _bar;
        private float _initiative = 1f;

        void Start()
        {
            _bar = transform.Find("Bar");
            _bar.localScale = new Vector3(_bar.localScale.x * _initiative / 100, _bar.localScale.y);
        }

        public void Initialize(float initiative)
        {
            _initiative = initiative;
        }
    }
}
