using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// written by Vanya
namespace UnityEngine.Tilemaps
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Prefab Tile", menuName = "Tiles/Extended/Prefab Tile")]
    public class PrefabTile : TileBase
    {
        public GameObject m_Prefab;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
                tileData.gameObject = m_Prefab;
        }

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject gameObject)
        {
            return base.StartUp(position, tilemap, gameObject);
        }

        public GameObject GetLastInstant { get; private set; }
    }
}