using System.Collections.Generic;
using System.Linq;
using BrunoMikoski.Pahtfinding.Grid;
using Priority_Queue;
using UnityEngine;
using UnityEngine.Profiling;

namespace BrunoMikoski.Pahtfinding
{
    public static class Pathfinder
    {
        private static GridController gridController;

        private static FastPriorityQueue<Tile> openListPriorityQueue;
        private static Dictionary<int, Tile> tileIndexToTileObjectOpen = new Dictionary<int, Tile>();
        private static HashSet<Tile> closedList = new HashSet<Tile>();
        private static Tile[] neighbors = new Tile[4];

        public static void Initialize( GridController targetGridController )
        {
            gridController = targetGridController;
            openListPriorityQueue = new FastPriorityQueue<Tile>( gridController.GridSizeX * gridController.GridSizeY );
        }

        public static List<Vector2Int> GetPath( Vector2Int from, Vector2Int to )
        {
            closedList.Clear();

            int fromIndex = gridController.TilePosToIndex( from.x, from.y );
            int toIndex = gridController.TilePosToIndex( to.x, to.y );

            Tile initialTile = gridController.Tiles[fromIndex];
            Tile destinationTile = gridController.Tiles[toIndex];

            openListPriorityQueue.Enqueue( initialTile, 0 );
            tileIndexToTileObjectOpen.Add( initialTile.Index, initialTile );

            while ( openListPriorityQueue.Count > 0 )
            {
                Tile currentTile = openListPriorityQueue.Dequeue();
                tileIndexToTileObjectOpen.Remove( currentTile.Index );

                closedList.Add( currentTile );

                if ( currentTile == destinationTile )
                    break;

                UpdateNeighbors( currentTile );

                foreach ( Tile neighbourPathTile in neighbors )
                {
                    if ( neighbourPathTile == null )
                        continue;

                    if ( closedList.Contains( neighbourPathTile ) )
                        continue;

                    float movementCostToNeighbour = currentTile.GCost + GetDistance( currentTile, neighbourPathTile );
                    bool isAtOpenList = tileIndexToTileObjectOpen.ContainsKey( neighbourPathTile.Index );
                    if ( movementCostToNeighbour < neighbourPathTile.GCost || !isAtOpenList )
                    {
                        neighbourPathTile.SetGCost( movementCostToNeighbour );
                        neighbourPathTile.SetHCost( GetDistance( neighbourPathTile, destinationTile ) );
                        neighbourPathTile.SetParent( currentTile );

                        if ( !isAtOpenList )
                        {
                            openListPriorityQueue.Enqueue( neighbourPathTile,
                                                           neighbourPathTile.FCost + neighbourPathTile.HCost );
                            tileIndexToTileObjectOpen.Add( neighbourPathTile.Index, neighbourPathTile );
                        }
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
            
            openListPriorityQueue.Clear();
            tileIndexToTileObjectOpen.Clear();
            return finalPath;
        }


        private static float GetDistance( Tile targetFromTile, Tile targetToTile )
        {
            return (targetFromTile.TilePosition.x - targetToTile.TilePosition.x) *
                   (targetFromTile.TilePosition.x - targetToTile.TilePosition.x) +
                   (targetFromTile.TilePosition.y - targetToTile.TilePosition.y) *
                   (targetFromTile.TilePosition.y - targetToTile.TilePosition.y);
        }

        private static Tile[] UpdateNeighbors( Tile targetTile )
        {
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
