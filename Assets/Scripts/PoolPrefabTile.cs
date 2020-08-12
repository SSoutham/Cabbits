using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// written by Iaro
namespace UnityEngine.Tilemaps
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Pool Prefab Tile", menuName = "Tiles/Extended/Pool Prefab Tile")]
    public class PoolPrefabTile : TileBase
    {
        public string poolType;
        private ObjectPool pool;
        private GameObject myGO = null;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            if (!pool) pool = GameObject.FindObjectOfType<PoolManager>().pools[poolType];

            myGO = pool.Get(position, Quaternion.identity);
        }

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject gameObject)
        {
            return base.StartUp(position, tilemap, gameObject);
        }

        public GameObject GetLastInstant { get; private set; }

        public void Return()
        {
            pool.Return(myGO);
        }
    }
}