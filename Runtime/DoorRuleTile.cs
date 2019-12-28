using UnityEngine;
using UnityEngine.Tilemaps;

namespace RuleTileExtras
{
    [CreateAssetMenu(menuName = "Rule Tile Extras/Door Rule Tile")]
    public partial class DoorRuleTile : RuleTile
    {

        public int rulesSplitIndex = 0;
        public string gridInformationKey = "Is Opened";

        protected override bool RuleMatches(TilingRule rule, Vector3Int position, ITilemap tilemap, ref Matrix4x4 transform)
        {
            GridInformation gridInfo = tilemap.GetComponent<GridInformation>();
            bool isOpen = gridInfo ? IsOpen(position, gridInfo) : false;
            bool isOpenRule = m_TilingRules.IndexOf(rule) >= rulesSplitIndex;

            if (isOpen != isOpenRule)
                return false;

            return base.RuleMatches(rule, position, tilemap, ref transform);
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
