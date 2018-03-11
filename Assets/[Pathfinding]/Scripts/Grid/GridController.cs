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

        private Tile[] tiles;
        public Tile[] Tiles
        {
            get
            {
                return tiles;
            }
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
            tiles[index].SetType( type );
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
            return tiles[index].TileType;
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
            return tiles[index].TileType == TileType.BLOCK;
        }

        public bool IsTileBlocked( int x, int y )
        {
            return IsTileBlocked( TilePosToIndex( x, y ) );
        }

        public void GenerateTiles()
        {
            tiles = new Tile[gridSizeX * gridSizeY];
            for ( int i = tiles.Length - 1; i >= 0; i-- )
            {
                int positionX;
                int positionY;
                IndexToTilePos( i, out positionX, out positionY );
                tiles[i] = new Tile( i, positionX, positionY );
            }
        }


        public bool IsValidTilePosition( int targetPositionX, int targetPositionY )
        {
            if ( targetPositionX < 0 || targetPositionX > gridSizeX - 1 )
                return false;

            if ( targetPositionY < 0 || targetPositionY > gridSizeY - 1 )
                return false;

            int tilePosToIndex = TilePosToIndex( targetPositionX, targetPositionY );

            if ( tiles[tilePosToIndex].TileType == TileType.BLOCK)
                return false;

            return true;
        }

        public bool IsValidTilePosition( Vector2Int targetPosition )
        {
            return IsValidTilePosition( targetPosition.x, targetPosition.y );
        }

        public void Clear()
        {
            for ( int i = tiles.Length - 1; i >= 0; i-- )
                SetTileType( i, TileType.EMPTY );
        }
    }
}
