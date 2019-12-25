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
            public Sprite sprite;
            public GameObject gameObject;
            public Tile.ColliderType colliderType;
        }

        public OutputTileData[] outputs = new OutputTileData[2];
        public string gridInformationKey = "Is Opened";

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            GridInformation gridInfo = tilemap.GetComponent<GridInformation>();
            bool isOpen = gridInfo ? IsOpen(position, gridInfo) : false;
            OutputTileData data = outputs[isOpen ? 1 : 0];

            tileData.sprite = data.sprite;
            tileData.gameObject = data.gameObject;
            tileData.colliderType = data.colliderType;
        }

        public bool IsOpen(Vector3Int position, GridInformation gridInfo)
        {
            return gridInfo.GetPositionProperty(position, gridInformationKey, 0) == 1;
        }

        public void SetOpen(Vector3Int position, Tilemap tilemap, GridInformation gridInfo, bool open)
        {
            gridInfo.SetPositionProperty(position, gridInformationKey, open ? 1 : 0);
            tilemap.RefreshTile(position);
        }
    }
}
