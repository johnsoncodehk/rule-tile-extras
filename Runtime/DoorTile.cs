using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RuleTileExtras
{
    [CreateAssetMenu(menuName = "Rule Tile Extras/Door Tile")]
    public partial class DoorTile : TileBase
    {

        [System.Serializable]
        public struct OutputTileData
        {
            public Sprite m_Sprite;
            public GameObject m_GameObject;
            public Tile.ColliderType m_ColliderType;
        }

        public OutputTileData[] outputs = new OutputTileData[2];

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            bool doorOpen = IsOpen(position, tilemap.GetComponent<Tilemap>());
            OutputTileData data = outputs[doorOpen ? 1 : 0];

            tileData.sprite = data.m_Sprite;
            tileData.gameObject = data.m_GameObject;
            tileData.colliderType = data.m_ColliderType;
        }

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {
            var dataKey = GetDataKey(position, tilemap);

            if (!m_Data.ContainsKey(dataKey))
                SetOpen(dataKey, false);

            return true;
        }
    }

    public partial class DoorTile
    {

        public struct DataKey
        {
            public Vector3Int position;
            public Tilemap tilemap;
        }

        public static Dictionary<DataKey, bool> m_Data = new Dictionary<DataKey, bool>();

        public static void SetOpen(Vector3Int position, Tilemap tilemap, bool open)
        {
            var dataKey = GetDataKey(position, tilemap);
            SetOpen(dataKey, open);
        }

        public static void SetOpen(DataKey dataKey, bool open)
        {
            m_Data[dataKey] = open;
            dataKey.tilemap.RefreshTile(dataKey.position);
        }

        public static bool IsOpen(Vector3Int position, Tilemap tilemap)
        {
            var dataKey = GetDataKey(position, tilemap);

            if (m_Data.ContainsKey(dataKey))
                return m_Data[dataKey];

            return false;
        }

        public static DataKey GetDataKey(Vector3Int position, ITilemap tilemap)
        {
            return GetDataKey(position, tilemap.GetComponent<Tilemap>());
        }

        public static DataKey GetDataKey(Vector3Int position, Tilemap tilemap)
        {
            var dataKey = new DataKey();
            dataKey.position = position;
            dataKey.tilemap = tilemap;
            return dataKey;
        }
    }
}
