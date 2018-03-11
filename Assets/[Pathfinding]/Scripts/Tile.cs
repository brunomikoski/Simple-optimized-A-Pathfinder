using BrunoMikoski.Pahtfinding.Grid;
using Priority_Queue;
using UnityEngine;

namespace BrunoMikoski.Pahtfinding
{
    public sealed class Tile : FastPriorityQueueNode
    {
        private Tile parent;
        public Tile Parent
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

        private TileType tileType;
        public TileType TileType
        {
            get
            {
                return tileType;
            }
        }

        private int index;
        public int Index
        {
            get
            {
                return index;
            }
        }

        public Tile( int targetTileIndex, Vector2Int targetPosition )
        {
            index = targetTileIndex;
            SetTilePostion( targetPosition );
        }

        public void SetParent( Tile targetTile )
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

        public void SetType( TileType targetType )
        {
            if(tileType == targetType)
                return;
            
            tileType = targetType;
            Events.EventsDispatcher.Grid.DispatchOnTileTypeChangedEvent( this);
        }
    }
}
