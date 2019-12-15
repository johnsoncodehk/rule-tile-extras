using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace RuleTileExtras
{
    [CreateAssetMenu(menuName = "Rule Tile Extras/Edge Tile")]
    public class EdgeTile : Tile
    {

        public static Dictionary<ITilemap, HashSet<Vector3Int>> m_StartUpPositions = new Dictionary<ITilemap, HashSet<Vector3Int>>();

        public override bool StartUp(Vector3Int location, ITilemap tilemap, GameObject instantiatedGameObject)
        {
            if (!m_StartUpPositions.ContainsKey(tilemap))
                m_StartUpPositions[tilemap] = new HashSet<Vector3Int>();

            if (m_StartUpPositions[tilemap].Add(location))
                tilemap.RefreshTile(location);

            return base.StartUp(location, tilemap, instantiatedGameObject);
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);

            if (Application.isPlaying)
                tileData.sprite = null;
        }
    }
}
