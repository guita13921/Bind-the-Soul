using System.Collections.Generic;
using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class TileScroller : MonoBehaviour
    {
        // A list of sprite tiles that will be scrolling in sequence from bottom to top
        public List<Sprite> spriteTiles = new List<Sprite>();
        public GameObject tilePrefab;
        public int replicate = 5;
        public List<Sprite> decals = new List<Sprite>();
        public GameObject decalPrefab;
        public int maxDecals = 3;
        public int decalChance = 30;
        public float yOffset = 5f;
        public Vector2 decalRandom = new Vector2(3f, 3f);
        public float zInc = 1f;

        private List<GameObject> _tiles = new List<GameObject>();
        private Vector3 _offset = new Vector3(0f, 0f, 0f);

        void Awake()
        {
            int lastDecal = -1;

            // Create SpriteRenderer components for each tile and position them
            for (int k = 0; k < replicate; ++k)
            {
                for (int i = 0; i < spriteTiles.Count; ++i)
                {
                    GameObject obj = Instantiate(tilePrefab);
                    obj.GetComponent<SpriteRenderer>().sprite = spriteTiles[i];
                    obj.transform.SetParent(transform);
                    obj.transform.localPosition = _offset;
                    obj.layer = gameObject.layer;
                    _tiles.Add(obj);

                    _offset += new Vector3(0f, yOffset, zInc);

                    // Add random decals to each tile
                    if (decalPrefab != null && decals.Count > 0)
                    {
                        for (int j = 0; j < maxDecals; ++j)
                        {
                            if (Random.Range(0, 100) < decalChance)
                            {
                                int nextDecal = Random.Range(0, decals.Count - 1);
                                while (nextDecal == lastDecal)
                                {
                                    nextDecal = Random.Range(0, decals.Count - 1);
                                } // while

                                lastDecal = nextDecal;
                                GameObject decal = Instantiate(decalPrefab);
                                decal.GetComponent<SpriteRenderer>().sprite = decals[nextDecal];
                                decal.transform.SetParent(obj.transform);
                                decal.transform.localPosition = new Vector2(Random.Range(-decalRandom.x, decalRandom.x), Random.Range(-decalRandom.y, decalRandom.y));
                                decal.layer = gameObject.layer;
                            } // if
                        } // for j
                    } // if
                } // for i
            } // for j
        } // Awake()
    } // class TileScroller
} // namespace Magique.SoulLink