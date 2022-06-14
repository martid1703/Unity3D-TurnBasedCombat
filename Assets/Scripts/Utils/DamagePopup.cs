using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class DamagePopup : MonoBehaviour
    {
        [SerializeField]
        private float _criticalDamageFontSize = 50f;

        [SerializeField]
        private float _regularDamageFontSize = 20f;

        private TextMeshPro _textMeshPro;

        [SerializeField]
        private Color _regularDamageColor;

        [SerializeField]
        private Color _criticalDamageColor;

        private void Awake()
        {
            _textMeshPro = GetComponentInChildren<TextMeshPro>();
        }

        public void CriticalPopup(float damage, Vector3 position, float destroyTime = 2f)
        {
            _textMeshPro.fontSize = _criticalDamageFontSize;
            _textMeshPro.color = _criticalDamageColor;
            Popup(damage, position);
        }

        public void RegularPopup(float damage, Vector3 position, float destroyTime = 2f)
        {
            _textMeshPro.fontSize = _regularDamageFontSize;
            _textMeshPro.color = _regularDamageColor;
            Popup(damage, position);
        }

        private void Popup(float damage, Vector3 position, float destroyTime = 1f)
        {
            _textMeshPro.text = damage.ToString();
            StartCoroutine(Move(destroyTime, 1f));
            StartCoroutine(DestroyAfter(destroyTime));
        }

        private IEnumerator DestroyAfter(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            GameObject.Destroy(transform.gameObject);
        }

        private IEnumerator Move(float moveTime, float beginDisappearTime = 0.5f)
        {
            float moveSpeed = 10f;
            var moveTimeLocal = moveTime;
            var color = _textMeshPro.color;
            Vector3 moveUpVector = new Vector3(0.2f, 1f) * moveSpeed;
            Vector3 moveDownVector = new Vector3(0.2f, -1f) * moveSpeed;

            while (moveTimeLocal > 0)
            {
                if (moveTimeLocal >= moveTime / 2)
                {
                    transform.position += moveUpVector * Time.deltaTime;
                    transform.localScale += Vector3.one * Time.deltaTime;
                }

                if (moveTimeLocal < moveTime / 2)
                {
                    transform.position += moveDownVector * Time.deltaTime;
                    transform.localScale -= Vector3.one * Time.deltaTime;
                }

                if (beginDisappearTime < 0)
                {
                    color.a -= 1 * Time.deltaTime;
                    _textMeshPro.color = color;
                }

                moveTimeLocal -= Time.deltaTime;
                beginDisappearTime -= Time.deltaTime;
                yield return null;
            }
        }
    }
}
