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

        private float destinationCost;
        public float DestinationCost
        {
            get { return destinationCost; }
        }

        private float fromParentCost;
        public float FromParentCost
        {
            get { return fromParentCost; }
        }

        private float totalCost;
        public float TotalCost
        {
            get { return totalCost; }
        }

        private Vector2Int tilePosition;
        public Vector2Int TilePosition
        {
            get
            {
                return tilePosition;
            }
        }

        private int[] neighboursPositionIndexes;

        public PathTile( Vector2Int targetPosition )
        {
            SetTilePostion( targetPosition );
        }

        public void SetToDestinationCost( float targetValue )
        {
            destinationCost = targetValue;
        }

        public void SetFromParentCost( float targetValue )
        {
            fromParentCost = targetValue;
        }

        public void SetTotalCost( float targetValue )
        {
            totalCost = targetValue;
        }

        public void SetParent( PathTile targetTile )
        {
            parent = targetTile;
        }

        public void SetTotalCost()
        {
            totalCost = FromParentCost + DestinationCost;
        }

        public void SetTilePostion( Vector2Int targetTilePosition )
        {
            tilePosition = targetTilePosition;
        }

        public void Clear()
        {
            SetTilePostion( Vector2Int.zero );
            SetParent( null );
            SetFromParentCost( 0 );
            SetToDestinationCost( 0 );
            SetTotalCost( 0 );
        }
    }
}
