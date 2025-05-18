using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magique.SoulLink
{
    public class RandomLevel : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> _prefabs = new List<Transform>();

        [SerializeField]
        private int _spawnQty = 30;

        [SerializeField]
        private float _radius = 10f;

        [SerializeField]
        private float _minScale = 1f;

        [SerializeField]
        private float _maxScale = 3f;

        void Start()
        {
            GenerateLevel();
        } // Start()

        void GenerateLevel()
        {
            for (int i = 0; i < _spawnQty; ++i)
            {
                // randomize x/z width and height
                float xWidth = Random.Range(_minScale, _maxScale);
                float zWidth = Random.Range(_minScale, _maxScale);
                float height = Random.Range(_minScale, _maxScale);

                // instantiate in scene
                var j = Random.Range(0, _prefabs.Count);

                var instance = Instantiate(_prefabs[j]);
                instance.transform.localScale = new Vector3(xWidth, height, zWidth);
                instance.transform.position = GetRandomPos();
            } // for i
        } // GenerateLevel()

        Vector3 GetRandomPos()
        {
            // randomize position from this point
            float x = Random.Range(5f, _radius);
            float z = Random.Range(5f, _radius);

            if (Random.Range(0, 100) < 50) x *= -1f;
            if (Random.Range(0, 100) < 50) z *= -1f;

            return new Vector3(x + transform.position.x, transform.position.y, z + transform.position.z);
        } // GetRandomPos()

    } // class RandomLevel
} // Magique.SoulLink