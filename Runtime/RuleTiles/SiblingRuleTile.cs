using UnityEngine;
using UnityEngine.Tilemaps;

namespace RuleTileExtras
{
    [CreateAssetMenu(menuName = "Rule Tile Extras/Rule Tiles/Sibling Rule Tile")]
    public class SiblingRuleTile : RuleTile
    {

        public bool m_MatchEdge;
        public bool m_MatchSilbling;
        public int m_SiblingLayer;

        public override bool RuleMatch(int neighbor, TileBase other)
        {
            if (m_MatchEdge && other is EdgeTile)
            {
                switch (neighbor)
                {
                    case TilingRule.Neighbor.This: return true;
                    case TilingRule.Neighbor.NotThis: return false;
                }
            }

            if (other is RuleOverrideTile)
                other = (other as RuleOverrideTile).m_InstanceTile;

            if (m_MatchSilbling && other is SiblingRuleTile)
            {
                SiblingRuleTile otherSibTile = other as SiblingRuleTile;
                bool match = m_SiblingLayer == otherSibTile.m_SiblingLayer;

                switch (neighbor)
                {
                    case TilingRule.Neighbor.This: return match;
                    case TilingRule.Neighbor.NotThis: return !match;
                }
            }

            return base.RuleMatch(neighbor, other);
        }
    }
}
