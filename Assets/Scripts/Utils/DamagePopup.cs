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

        public IEnumerator CriticalPopup(float damage, Vector3 position, float destroyTime)
        {
            _textMeshPro.fontSize = _criticalDamageFontSize;
            _textMeshPro.color = _criticalDamageColor;
            yield return Popup(damage, position, destroyTime);
        }

        public IEnumerator RegularPopup(float damage, Vector3 position, float destroyTime)
        {
            _textMeshPro.fontSize = _regularDamageFontSize;
            _textMeshPro.color = _regularDamageColor;
            yield return Popup(damage, position, destroyTime);
        }

        private IEnumerator Popup(float damage, Vector3 position, float destroyTime)
        {
            _textMeshPro.text = damage.ToString();
            yield return StartCoroutine(Move(destroyTime));
            yield return StartCoroutine(DestroyAfter(destroyTime));
        }

        private IEnumerator DestroyAfter(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            GameObject.Destroy(transform.gameObject);
        }

        private IEnumerator Move(float moveTime)
        {
            float moveSpeed = 10f;
            var moveTimeLocal = moveTime;
            var color = _textMeshPro.color;
            Vector3 moveUpVector = new Vector3(0.2f, 1f) * moveSpeed;
            Vector3 moveDownVector = new Vector3(0.2f, -1f) * moveSpeed;

            float moveLimit = 2f / 5f;
            while (moveTimeLocal > 0)
            {
                if (moveTimeLocal >= moveTime * moveLimit)
                {
                    transform.position += moveUpVector * Time.deltaTime;
                    transform.localScale += Vector3.one * Time.deltaTime;
                }
                else
                {
                    transform.position += moveDownVector * Time.deltaTime;
                    transform.localScale -= Vector3.one * 4 * Time.deltaTime;
                    color.a -= 5 * Time.deltaTime;
                    _textMeshPro.color = color;
                }

                moveTimeLocal -= Time.deltaTime;
                yield return null;
            }
        }
    }
}
