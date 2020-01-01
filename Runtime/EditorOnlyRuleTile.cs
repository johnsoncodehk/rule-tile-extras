using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace RuleTileExtras
{
    [CreateAssetMenu(menuName = "Rule Tile Extras/Editor Only Rule Tile")]
    public class EditorOnlyRuleTile : BaseRuleTile
    {

        static HashSet<KeyValuePair<ITilemap, Vector3Int>> m_StartUpPositions = new HashSet<KeyValuePair<ITilemap, Vector3Int>>();

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject instantiatedGameObject)
        {
            if (m_StartUpPositions.Add(new KeyValuePair<ITilemap, Vector3Int>(tilemap, position)))
                tilemap.RefreshTile(position);

            return base.StartUp(position, tilemap, instantiatedGameObject);
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);

            if (Application.isPlaying)
                tileData.sprite = null;
        }
    }
}
