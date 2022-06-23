using System.Collections;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class CameraController : MonoBehaviour
    {
        private Camera _camera;
        private float _correction = 1.0f;

        private void Start()
        {
            _camera = GetComponent<Camera>();
        }

        public IEnumerator FitOverview(RectTransform target, float speed = 5f)
        {
            yield return ScaleAndFit(target, speed);
        }

        public IEnumerator FitBattle(RectTransform target, float speed = 5f)
        {
            yield return ScaleAndFit(target, speed);
        }

        private IEnumerator ScaleAndFit(RectTransform target, float speed = 10f)
        {
            var targetOrthographicSize = target.rect.width / _camera.aspect;

            while (Mathf.Abs(_camera.orthographicSize - targetOrthographicSize) > 0.01f && transform.position != target.position)
            {
                float xTarget = target.position.x;
                float yTarget = target.position.y;
                float xNew = Mathf.Lerp(transform.position.x, xTarget, Time.deltaTime * speed);
                float yNew = Mathf.Lerp(transform.position.y, yTarget, Time.deltaTime * speed);
                transform.position = new Vector3(xNew, yNew, transform.position.z);

                _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, targetOrthographicSize, Time.deltaTime * speed);

                yield return null;
            }
        }
    }
}