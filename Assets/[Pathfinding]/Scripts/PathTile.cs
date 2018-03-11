using UnityEngine;

namespace BrunoMikoski.Pahtfinding
{
    public class PathTile
    {
        private PathTile parent;
        public PathTile Parent
        {
            get { return parent; }
        }

        private Vector2Int tilePosition;
        public Vector2Int TilePosition
        {
            get
            {
                return tilePosition;
            }
        }

        private float gCost;
        public float GCost
        {
            get { return gCost; }
        }

        private float hCost;
        public float HCost
        {
            get
            {
                return hCost;
            }
        }

        public float FCost
        {
            get
            {
                return gCost + hCost;
            }
        }
        private int[] neighboursPositionIndexes;

        public PathTile( Vector2Int targetPosition )
        {
            SetTilePostion( targetPosition );
        }

        public void SetParent( PathTile targetTile )
        {
            parent = targetTile;
        }

        private void SetTilePostion( Vector2Int targetTilePosition )
        {
            tilePosition = targetTilePosition;
        }

        public void SetGCost( float targetGCost )
        {
            gCost = targetGCost;
        }

        public void SetHCost( float targetHCost )
        {
            hCost = targetHCost;
        }
    }
}
