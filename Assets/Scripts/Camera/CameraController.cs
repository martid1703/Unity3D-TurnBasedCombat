﻿using System.Collections;
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

        public IEnumerator FitOverview(Rect target, float speed = 5f)
        {
            yield return ScaleAndFit(target, speed);
        }

        public IEnumerator FitBattle(Rect target, float speed = 5f)
        {
            yield return ScaleAndFit(target, speed);
        }

        private IEnumerator ScaleAndFit(Rect target, float speed = 10f)
        {
            float tolerance = 0.1f;
            float heightCorrection = 1.1f;
            var height = target.width / _camera.aspect;
            var targetOrthographicSize = height / 2 * heightCorrection;

            while (Mathf.Abs(_camera.orthographicSize - targetOrthographicSize) > tolerance || (Vector2)transform.position != target.center)
            {
                float xNew = Mathf.MoveTowards(transform.position.x, target.center.x, Time.deltaTime * speed);
                float yNew = Mathf.MoveTowards(transform.position.y, target.center.y, Time.deltaTime * speed);
                transform.position = new Vector3(xNew, yNew, transform.position.z);

                _camera.orthographicSize = Mathf.MoveTowards(_camera.orthographicSize, targetOrthographicSize, Time.deltaTime * speed);

                yield return null;
            }
        }
    }
}