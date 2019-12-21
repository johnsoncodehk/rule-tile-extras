using UnityEngine;
using UnityEngine.Tilemaps;

namespace RuleTileExtras
{
    [CreateAssetMenu(menuName = "Rule Tile Extras/Rule Tiles/Sibling Rule Tile")]
    public class SiblingRuleTile : RuleTile
    {

        public TileBase[] m_Siblings = new TileBase[0];
        public bool m_MatchSiblingLayer;
        public int m_SiblingLayer;

        public override bool RuleMatch(int neighbor, TileBase other)
        {
            bool isMatchCondition = neighbor == RuleTile.TilingRule.Neighbor.This;

            foreach (TileBase sibling in m_Siblings)
                if (other == sibling)
                    return isMatchCondition;

            if (other is RuleOverrideTile)
                other = (other as RuleOverrideTile).m_InstanceTile;

            if (m_MatchSiblingLayer && other is SiblingRuleTile)
            {
                bool isMatch = m_SiblingLayer == (other as SiblingRuleTile).m_SiblingLayer;
                return isMatch == isMatchCondition;
            }

            return base.RuleMatch(neighbor, other);
        }
    }
}
