using System.Collections;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class CameraController : MonoBehaviour
    {
        private Camera _camera;

        private void Start()
        {
            _camera = GetComponent<Camera>();
        }

        public Transform GetTransform()
        {
            return _camera.transform;
        }

        public void Fit(Transform go)
        {
            if (_camera.orthographic)
            {
                FitOrthographicCamera(go.transform);
            }
        }

        private IEnumerator FitOrthographicCamera(Transform target, float speed = 5f)
        {
            var scale = ObjectScaler.Instance.GetScaleCoefficient(transform, target);

            while (Mathf.Abs(scale - 1f) > 0.05f || transform.position != target.position)
            {
                float xTarget = target.position.x;
                float yTarget = target.position.y;
                float xNew = Mathf.Lerp(transform.position.x, xTarget, Time.deltaTime * speed);
                float yNew = Mathf.Lerp(transform.position.y, yTarget, Time.deltaTime * speed);
                transform.position = new Vector3(xNew, yNew, transform.position.z);

                var rect = target.GetComponent<RectTransform>().rect;
                float orthographicSize = rect.height / 2;
                _camera.orthographicSize = orthographicSize;
                _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, orthographicSize, Time.deltaTime * speed);

                scale = ObjectScaler.Instance.GetScaleCoefficient(transform, target);
                yield return null;
            }
        }
    }
}