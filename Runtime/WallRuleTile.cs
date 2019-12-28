using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace RuleTileExtras
{
    [CreateAssetMenu(menuName = "Rule Tile Extras/Wall Rule Tile")]
    public class WallRuleTile : RuleTile<WallRuleTile.Neighbor>
    {

        public class Neighbor
        {
            public const int OtherTile = 2;
        }

        public List<TileBase> excludes = new List<TileBase>();

        public override bool RuleMatch(int neighbor, TileBase other)
        {
            if (other is RuleOverrideTile)
                other = (other as RuleOverrideTile).m_InstanceTile;

            switch (neighbor)
            {
                case Neighbor.OtherTile:
                    return other && other != this && !excludes.Contains(other);
            }

            return base.RuleMatch(neighbor, other);
        }
    }
}
