using System.Collections;
using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class Mover : MonoBehaviour
    {
        [SerializeField]
        private float _velocityX = 0f;

        [SerializeField]
        private float _velocityY = -1f;

        [SerializeField]
        private Vector2 _randomXVelocity = new Vector2();

        [SerializeField]
        private float _randXChangeTime = 1.5f;

        [SerializeField]
        private Vector2 _randomYVelocity = new Vector2();

        [SerializeField]
        private float _randYChangeTime = 1.5f;

        [SerializeField]
        private float _rotationAngle = 0f;

        private float _rotDir = 1;

        private void Awake()
        {
            _rotDir = Random.Range(0, 100) > 50 ? 1 : -1;
        } // Awake()

        public void Activate()
        {
            OnSpawned();
        } // Activate()

        void OnSpawned()
        {
            StopCoroutine(RandomXMove());

            if (_randomXVelocity != Vector2.zero)
            {
                StartCoroutine(RandomXMove());
            }

            if (_randomYVelocity != Vector2.zero)
            {
                StartCoroutine(RandomYMove());
            }
        } // OnSpawned()

        IEnumerator RandomXMove()
        {
            bool done = false;
            while (!done)
            {
                if (Random.Range(0, 100) < 50)
                {
                    _velocityX = Random.Range(-_randomXVelocity.x, -_randomXVelocity.y);
                }
                else
                {
                    _velocityX = Random.Range(_randomXVelocity.x, _randomXVelocity.y);
                }

                yield return new WaitForSeconds(_randXChangeTime);

                if (_randXChangeTime == 0f) done = true;
            } // while
        } // RandomXMove()

        IEnumerator RandomYMove()
        {
            bool done = false;
            while (!done)
            {
                _velocityY = Random.Range(_randomYVelocity.x, _randomYVelocity.y);

                yield return new WaitForSeconds(_randYChangeTime);

                if (_randYChangeTime == 0f) done = true;
            } // while
        } // RandomYMove()

        void Update()
        {
            var deltaX = _velocityX * Time.deltaTime;
            var deltaY = _velocityY * Time.deltaTime;

            transform.position = new Vector3(transform.position.x + deltaX, transform.position.y + deltaY, 0f);

            if (_rotationAngle > 0f)
            {
                transform.Rotate(new Vector3(0f, 0f, 1f), _rotationAngle * Time.deltaTime * _rotDir);
            } // if
        } // Update()
    } // class Mover
} // namespace Magique.SoulLink