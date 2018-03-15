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

        private int positionX;
        public int PositionX
        {
            get
            {
                return positionX;
            }
        }
        private int positionY;
        public int PositionY
        {
            get
            {
                return positionY;
            }
        }

        private float gCost;
        public float GCost
        {
            get { return gCost; }
        }

        private float hCost;

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

        public Tile( int targetTileIndex, int targetPositionX, int targetPOsitionY )
        {
            index = targetTileIndex;
            SetTilePostion( targetPositionX, targetPOsitionY );
        }

        public void SetParent( Tile targetTile )
        {
            parent = targetTile;
        }

        private void SetTilePostion( int targetPositionX, int targetPOsitionY )
        {
            positionX = targetPositionX;
            positionY = targetPOsitionY;
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

        public override bool Equals( object obj )
        {
            Tile otherTile = obj as Tile;
            if ( otherTile == null )
                return false;

            return Index == otherTile.Index;
        }
    }
}
