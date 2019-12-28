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
            bool isMatch = other == this;

            if (!isMatch)
            {
                isMatch = siblings.Contains(other);
            }

            if (!isMatch && matchSiblingLayer)
            {
                if (other is RuleOverrideTile)
                    other = (other as RuleOverrideTile).m_InstanceTile;

                if (other is SiblingRuleTile)
                    isMatch = siblingLayer == (other as SiblingRuleTile).siblingLayer;
            }

            return isMatch == isMatchCondition;
        }
    }
}
