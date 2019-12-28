using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace RuleTileExtras
{
    [CreateAssetMenu(menuName = "Rule Tile Extras/Sibling Rule Tile")]
    public class SiblingRuleTile : RuleTile
    {

        public List<TileBase> siblings = new List<TileBase>();
        public bool matchSiblingLayer;
        public int siblingLayer;

        public override bool RuleMatch(int neighbor, TileBase other)
        {
            bool isMatchCondition = neighbor == RuleTile.TilingRule.Neighbor.This;

            if (siblings.Contains(other))
                return isMatchCondition;

            if (other is RuleOverrideTile)
                other = (other as RuleOverrideTile).m_InstanceTile;

            if (matchSiblingLayer && other is SiblingRuleTile)
            {
                bool isMatch = siblingLayer == (other as SiblingRuleTile).siblingLayer;
                return isMatch == isMatchCondition;
            }

            return base.RuleMatch(neighbor, other);
        }
    }
}
