using System;
using System.Collections;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField]
        private Gradient _gradient;

        private float _reduceSpeed = 5f;

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

        public void SetReduceHPSpeed(float reduceSpeed)
        {
            _reduceSpeed = reduceSpeed;
        }

        public IEnumerator TakeDamage(float damage)
        {
            _health = Mathf.Clamp(_health - damage, 0, _health);
            if (_health < 0)
            {
                _health = 0;
            }

            float healthNormalized = _health / _maxHealth;
            double diff;
            do
            {
                diff = Math.Round(_bar.localScale.x - healthNormalized, 5);
                float heathChange = Mathf.Lerp(_bar.localScale.x, healthNormalized, _reduceSpeed * Time.deltaTime);
                _bar.localScale = new Vector3(heathChange, _bar.localScale.y);
                var color = _gradient.Evaluate(healthNormalized);
                SetColor(color);
                yield return null;

            } while (diff > 0);
        }

        private void SetColor(Color color)
        {
            _visibleHealth.color = color;
        }
    }
}
