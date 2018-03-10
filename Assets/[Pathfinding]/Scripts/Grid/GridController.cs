using UnityEngine;

namespace BrunoMikoski.Pahtfinding.Grid
{
    public enum TileType : byte
    {
        EMPTY,
        ROAD,
        BLOCK
    }

    public enum NeighborDirection
    {
        LEFT = 0,
        TOP = 1,
        RIGHT = 2,
        DOWN = 3
    }

    public sealed class GridController : MonoBehaviour
    {
        [SerializeField]
        private int gridSizeX;
        public int GridSizeX
        {
            get { return gridSizeX; }
        }

        [SerializeField]
        private int gridSizeY;
        public int GridSizeY
        {
            get { return gridSizeY; }
        }

        private TileType[] tileTypes;
        public TileType[] TileTypes
        {
            get { return tileTypes; }
        }


        public int TilePosToIndex( int x, int y )
        {
            return x + y * gridSizeY;
        }

        public void IndexToTilePos( int index, out int x, out int y )
        {
            x = index % gridSizeX;
            y = Mathf.FloorToInt( index / (float) gridSizeX );
        }

        public void SetTileType( int index, TileType type )
        {
            if ( tileTypes[index] == type )
                return;

            tileTypes[index] = type;

            int x;
            int y;
            IndexToTilePos( index, out x, out y );

            Events.EventsDispatcher.Grid.DispatchOnTileTypeChangedEvent( index, x, y, type );
        }

        public void SetTileType( int x, int y, TileType type )
        {
            SetTileType( TilePosToIndex( x, y ), type );
        }

        public void SetTileType( Vector2Int targetPosition, TileType type )
        {
            SetTileType( TilePosToIndex( targetPosition.x, targetPosition.y ), type );
        }

        public TileType GetTileType( int index )
        {
            return tileTypes[index];
        }

        public TileType GetTileType( int x, int y )
        {
            return GetTileType( TilePosToIndex( x, y ) );
        }

        public void SetTileBlocked( int index, bool blocked )
        {
            SetTileType( index, blocked ? TileType.BLOCK : TileType.EMPTY );
        }

        public void SetTileBlocked( int x, int y, bool blocked )
        {
            SetTileBlocked( TilePosToIndex( x, y ), blocked );
        }

        public bool IsTileBlocked( int index )
        {
            return tileTypes[index] == TileType.BLOCK;
        }

        public bool IsTileBlocked( int x, int y )
        {
            return IsTileBlocked( TilePosToIndex( x, y ) );
        }

        public void GenerateTiles()
        {
            tileTypes = new TileType[gridSizeX * gridSizeY];
        }


        public bool IsValidTilePosition( int targetPositionX, int targetPositionY )
        {
            if ( targetPositionX < 0 || targetPositionX > gridSizeX - 1 )
                return false;

            if ( targetPositionY < 0 || targetPositionY > gridSizeY - 1 )
                return false;

            int tilePosToIndex = TilePosToIndex( targetPositionX, targetPositionY );

            if ( tileTypes[tilePosToIndex] == TileType.BLOCK)
                return false;

            return true;
        }

        public bool IsValidTilePosition( Vector2Int targetPosition )
        {
            return IsValidTilePosition( targetPosition.x, targetPosition.y );
        }

        public void Clear()
        {
            for ( int i = tileTypes.Length - 1; i >= 0; i-- )
                SetTileType( i, TileType.EMPTY );
        }
    }
}
