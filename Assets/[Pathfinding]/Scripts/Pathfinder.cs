using System.Collections.Generic;
using System.Linq;
using BrunoMikoski.Pahtfinding.Grid;
using UnityEngine;

namespace BrunoMikoski.Pahtfinding
{
    public static class Pathfinder
    {
        private static GridController gridController;

        private static List<PathTile> openList;
        private static List<PathTile> closedList;

        public static void Initialize( GridController targetGridController )
        {
            gridController = targetGridController;
        }

        public static List<Vector2Int> GetPath( Vector2Int from, Vector2Int to )
        {
            openList = new List<PathTile>();
            closedList = new List<PathTile>();

            PathTile initialPathTile = new PathTile( from );
            initialPathTile.SetParent( initialPathTile );
            PathTile finalPathTile = new PathTile( to );
            AddToOpenList( initialPathTile );

            bool keepSearching = true;
            bool pathExists = true;

            while ( keepSearching && pathExists )
            {
                if ( openList.Count == 0 )
                {
                    pathExists = false;
                    break;
                }

                PathTile currentTile = GetBestPathTile();
                AddToCloseList( currentTile );

                if ( currentTile.TilePosition == finalPathTile.TilePosition )
                    keepSearching = false;
                else
                {
                    PathTile[] neighbourList = GetPathTileNeighbors( currentTile, finalPathTile );
                    foreach ( PathTile neighbourPathTile in neighbourList )
                    {
                        if ( neighbourPathTile == null )
                            continue;

                        if ( closedList.Contains( neighbourPathTile ) )
                            continue;

                        PathTile pathTileFromOpenList = GetPathTileFromOpenList( neighbourPathTile, false );
                        if ( pathTileFromOpenList == null )
                            AddToOpenList( neighbourPathTile );
                        else
                        {
                            if ( neighbourPathTile.FromParentCost < pathTileFromOpenList.FromParentCost )
                            {
                                pathTileFromOpenList.SetFromParentCost( neighbourPathTile.DestinationCost );
                                pathTileFromOpenList.SetTotalCost();
                                pathTileFromOpenList.SetParent( currentTile );
                            }
                        }
                    }
                }
            }

            PathTile pathTile = closedList.Last();
            List<Vector2Int> finalPath = new List<Vector2Int>();
            while ( pathTile.TilePosition != initialPathTile.TilePosition )
            {
                finalPath.Add( pathTile.TilePosition );
                pathTile = pathTile.Parent;
            }

            return finalPath;
        }

        private static PathTile GetPathTileFromOpenList( PathTile targetPathTile , bool remove = true)
        {
            foreach ( PathTile tile in openList )
            {
                if ( tile.TilePosition == targetPathTile.TilePosition )
                {
                    if(remove)
                        openList.Remove( tile );
                    return tile;
                }
            }

            return null;
        }

        private static PathTile[] GetPathTileNeighbors( PathTile currentTile, PathTile targetFinalPathTile )
        {
            PathTile[] neighbors = new PathTile[4];
            neighbors[0] = GetNeighborFromPathTile( currentTile, targetFinalPathTile, NeighborDirection.LEFT );
            neighbors[1] = GetNeighborFromPathTile( currentTile, targetFinalPathTile, NeighborDirection.TOP );
            neighbors[2] = GetNeighborFromPathTile( currentTile, targetFinalPathTile, NeighborDirection.RIGHT );
            neighbors[3] = GetNeighborFromPathTile( currentTile, targetFinalPathTile, NeighborDirection.DOWN );

            return neighbors;
        }

        private static PathTile GetNeighborFromPathTile( PathTile currentTile, PathTile targetFinalPathTile,
            NeighborDirection targetDirection )
        {
            Vector2Int neighborPosition;

            GetNeighbourInformation( currentTile, targetDirection, out neighborPosition );
            if ( !gridController.IsValidTilePosition( neighborPosition ) )
                return null;

            PathTile neighborPathTile = new PathTile( neighborPosition );

            neighborPathTile.SetFromParentCost( currentTile.FromParentCost +
                                                CalculateDistance( currentTile,
                                                                   neighborPathTile ) );
            neighborPathTile.SetToDestinationCost( CalculateDistance( neighborPathTile,
                                                                      targetFinalPathTile ) );
            neighborPathTile.SetTotalCost();
            neighborPathTile.SetParent( currentTile );
            return neighborPathTile;
        }

        private static void GetNeighbourInformation( PathTile targetTile, NeighborDirection targetDirection,
            out Vector2Int neighbourPosition )
        {
            neighbourPosition = new Vector2Int();
            switch ( targetDirection )
            {
                case NeighborDirection.LEFT:
                    neighbourPosition.x = targetTile.TilePosition.x - 1;
                    neighbourPosition.y = targetTile.TilePosition.y;
                    break;
                case NeighborDirection.TOP:
                    neighbourPosition.x = targetTile.TilePosition.x;
                    neighbourPosition.y = targetTile.TilePosition.y + 1;
                    break;
                case NeighborDirection.RIGHT:
                    neighbourPosition.x = targetTile.TilePosition.x + 1;
                    neighbourPosition.y = targetTile.TilePosition.y;

                    break;
                case NeighborDirection.DOWN:
                    neighbourPosition.x = targetTile.TilePosition.x;
                    neighbourPosition.y = targetTile.TilePosition.y - 1;
                    break;
            }
        }

        private static void AddToCloseList( PathTile targetPathTile )
        {
            closedList.Add( targetPathTile );
        }

        private static PathTile GetBestPathTile()
        {
            float minimumDistance = float.MaxValue;
            PathTile bestPathTile = null;
            foreach ( PathTile pathTile in openList )
            {
                if ( pathTile.TotalCost >= minimumDistance )
                    continue;

                minimumDistance = pathTile.TotalCost;
                bestPathTile = pathTile;
            }

            openList.Remove( bestPathTile );
            return bestPathTile;
        }

        private static void AddToOpenList( PathTile targetPathTile )
        {
            openList.Add( targetPathTile );
        }

        private static float CalculateDistance( PathTile fromTile, PathTile toTile )
        {
            return (fromTile.TilePosition.x - toTile.TilePosition.x) *
                   (fromTile.TilePosition.x - toTile.TilePosition.x) +
                   (fromTile.TilePosition.y - toTile.TilePosition.y) *
                   (fromTile.TilePosition.y - toTile.TilePosition.y);
        }
    }
}
