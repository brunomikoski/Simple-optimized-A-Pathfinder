using System.Collections.Generic;
using System.Linq;
using BrunoMikoski.Pahtfinding.Grid;
using UnityEngine;

namespace BrunoMikoski.Pahtfinding
{
    public static class Pathfinder
    {
        private static GridController gridController;

        private static List<Tile> openList = new List<Tile>();
        private static HashSet<Tile> closedList = new HashSet<Tile>();

        public static void Initialize( GridController targetGridController )
        {
            gridController = targetGridController;
        }

        public static List<Vector2Int> GetPath( Vector2Int from, Vector2Int to )
        {
            openList.Clear();
            closedList.Clear();

            int fromIndex = gridController.TilePosToIndex( from.x, from.y );
            int toIndex = gridController.TilePosToIndex( to.x, to.y );

            Tile initialTile = gridController.Tiles[fromIndex];
            Tile destinationTile = gridController.Tiles[toIndex];

            openList.Add( initialTile );

            while ( openList.Count > 0 )
            {
                Tile currentTile = openList[0];
                for ( int i = openList.Count - 1; i >= 1; i-- )
                {
                    if ( openList[i].FCost < currentTile.FCost ||
                         openList[i].FCost == currentTile.FCost &&
                         openList[i].HCost < currentTile.HCost )
                    {
                        currentTile = openList[i];
                    }
                }

                openList.Remove( currentTile );
                closedList.Add( currentTile );

                if ( currentTile == destinationTile )
                    break;

                Tile[] neighbours = GetPathTileNeighbors( currentTile );

                for ( int i = neighbours.Length - 1; i >= 0; i-- )
                {
                    Tile neighbourPathTile = neighbours[i];
                    if ( neighbourPathTile == null )
                        continue;

                    if ( closedList.Contains( neighbourPathTile ) )
                        continue;

                    float movementCostToNeighbour = currentTile.GCost + GetDistance( currentTile, neighbourPathTile );
                    bool isAtOpenList = openList.Contains( neighbourPathTile );
                    if ( movementCostToNeighbour < neighbourPathTile.GCost || !isAtOpenList )
                    {
                        neighbourPathTile.SetGCost( movementCostToNeighbour );
                        neighbourPathTile.SetHCost( GetDistance( neighbourPathTile, destinationTile ) );
                        neighbourPathTile.SetParent( currentTile );

                        if ( !isAtOpenList )
                            openList.Add( neighbourPathTile );
                    }
                }
            }

            Tile tile = closedList.Last();
            List<Vector2Int> finalPath = new List<Vector2Int>();
            while ( tile != initialTile )
            {
                finalPath.Add( tile.TilePosition );
                tile = tile.Parent;
            }

            finalPath.Reverse();
            return finalPath;
        }


        private static float GetDistance( Tile targetFromTile, Tile targetToTile )
        {
            float dstX = Mathf.Abs( targetFromTile.TilePosition.x - targetToTile.TilePosition.x );
            float dstY = Mathf.Abs( targetFromTile.TilePosition.y - targetToTile.TilePosition.y );

            if ( dstX > dstY )
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }

        private static Tile[] GetPathTileNeighbors( Tile targetTile )
        {
            Tile[] neighbors = new Tile[4];
            neighbors[0] = GetNeighborAtDirection( targetTile, NeighborDirection.LEFT );
            neighbors[1] = GetNeighborAtDirection( targetTile, NeighborDirection.TOP );
            neighbors[2] = GetNeighborAtDirection( targetTile, NeighborDirection.RIGHT );
            neighbors[3] = GetNeighborAtDirection( targetTile, NeighborDirection.DOWN );

            return neighbors;
        }

        private static Tile GetNeighborAtDirection( Tile targetTile, NeighborDirection targetDirection )
        {
            Vector2Int neighborPosition = GetNeighbourPosition( targetTile, targetDirection );
            if ( !gridController.IsValidTilePosition( neighborPosition ) )
                return null;
            
            int neighborIndex = gridController.TilePosToIndex( neighborPosition.x, neighborPosition.y );

            return gridController.Tiles[neighborIndex];
        }

        private static Vector2Int GetNeighbourPosition( Tile targetTile, NeighborDirection targetDirection )
        {
            Vector2Int neighbourPosition = targetTile.TilePosition;
            switch ( targetDirection )
            {
                case NeighborDirection.LEFT:
                    neighbourPosition.x -= 1;
                    break;
                case NeighborDirection.TOP:
                    neighbourPosition.y += 1;
                    break;
                case NeighborDirection.RIGHT:
                    neighbourPosition.x += 1;
                    break;
                case NeighborDirection.DOWN:
                    neighbourPosition.y -= 1;
                    break;
            }

            return neighbourPosition;
        }
    }
}
