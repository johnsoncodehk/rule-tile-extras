using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RuleTileExtras
{
    [CreateAssetMenu(menuName = "Rule Tile Extras/Door Tile")]
    public partial class DoorTile : TileBase
    {

        public Sprite m_OnOpenSprite, m_OnCloseSprite;
        public GameObject m_OnOpenGameObject, m_OnCloseGameObject;
        public Tile.ColliderType m_OnOpenColliderType, m_OnCloseColliderType;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            bool doorOpen = IsOpen(position, tilemap.GetComponent<Tilemap>());

            if (doorOpen)
            {
                tileData.sprite = m_OnOpenSprite;
                tileData.gameObject = m_OnOpenGameObject;
                tileData.colliderType = m_OnOpenColliderType;
            }
            else
            {
                tileData.sprite = m_OnCloseSprite;
                tileData.gameObject = m_OnCloseGameObject;
                tileData.colliderType = m_OnCloseColliderType;
            }
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

        public static Dictionary<KeyValuePair<Vector3Int, Tilemap>, bool> m_Data = new Dictionary<KeyValuePair<Vector3Int, Tilemap>, bool>();

        public static void SetOpen(Vector3Int position, Tilemap tilemap, bool open)
        {
            var dataKey = GetDataKey(position, tilemap);
            SetOpen(dataKey, open);
        }

        public static void SetOpen(KeyValuePair<Vector3Int, Tilemap> dataKey, bool open)
        {
            m_Data[dataKey] = open;
            dataKey.Value.RefreshTile(dataKey.Key);
        }

        public static bool IsOpen(Vector3Int position, Tilemap tilemap)
        {
            var dataKey = GetDataKey(position, tilemap);

            if (m_Data.ContainsKey(dataKey))
                return m_Data[dataKey];

            return false;
        }

        public static KeyValuePair<Vector3Int, Tilemap> GetDataKey(Vector3Int position, ITilemap tilemap)
        {
            return GetDataKey(position, tilemap.GetComponent<Tilemap>());
        }

        public static KeyValuePair<Vector3Int, Tilemap> GetDataKey(Vector3Int position, Tilemap tilemap)
        {
            return new KeyValuePair<Vector3Int, Tilemap>(position, tilemap);
        }
    }
}
