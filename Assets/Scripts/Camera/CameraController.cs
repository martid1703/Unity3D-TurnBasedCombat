using System.Collections;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class CameraController : MonoBehaviour
    {
        private Camera _camera;
        private float _overviewSize;

        private void Start()
        {
            _camera = GetComponent<Camera>();
            _overviewSize = _camera.orthographicSize;
        }

        public IEnumerator FitOverview(Transform target, float speed = 5f)
        {
            yield return ScaleAndFit(_overviewSize, target, speed);
        }

        public IEnumerator FitBattle(Transform target, float speed = 5f)
        {
            var rect = target.GetComponent<RectTransform>().rect;
            yield return ScaleAndFit(rect.height / 2f, target, speed);
        }

        private IEnumerator ScaleAndFit(float targetOrthographicSize, Transform target, float speed = 10f)
        {
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