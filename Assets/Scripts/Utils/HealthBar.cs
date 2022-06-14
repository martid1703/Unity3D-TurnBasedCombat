using System;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField]
        Gradient _gradient;

        private Transform _bar;
        private float _health;
        private float _maxHealth;
        private SpriteRenderer _visibleHealth;

        void Start()
        {
            _bar = transform.Find("Bar");
            _visibleHealth = _bar.Find("BarSprite").GetComponent<SpriteRenderer>();
            SetColor(_gradient.Evaluate(1f));
        }

        public void Initialize(float maxHealth)
        {
            _maxHealth = _health = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            _health = Mathf.Clamp(_health - damage, 0, _health);
            if (_health < 0)
            {
                _health = 0;
            }

            float healthNormalized = (float)Math.Round(_health / _maxHealth, 2);
            _bar.localScale = new Vector3(healthNormalized, _bar.localScale.y);
            var color = _gradient.Evaluate(healthNormalized);
            SetColor(color);
        }

        private void SetColor(Color color)
        {
           _visibleHealth.color = color;
        }
    }
}
