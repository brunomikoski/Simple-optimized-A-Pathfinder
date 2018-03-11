using System.Collections.Generic;
using System.Linq;
using BrunoMikoski.Pahtfinding.Grid;
using UnityEngine;

namespace BrunoMikoski.Pahtfinding
{
    public static class Pathfinder
    {
        private static GridController gridController;

        private static List<PathTile> openList = new List<PathTile>();
        private static HashSet<PathTile> closedList = new HashSet<PathTile>();

        public static void Initialize( GridController targetGridController )
        {
            gridController = targetGridController;
        }

        public static List<Vector2Int> GetPath( Vector2Int from, Vector2Int to )
        {
            openList.Clear();
            closedList.Clear();

            PathTile initialPathTile = new PathTile( from );
            PathTile destinationPathTile = new PathTile( to );

            openList.Add( initialPathTile );

            while ( openList.Count > 0 )
            {
                PathTile currentPathTile = openList[0];
                for ( int i = 1; i < openList.Count; i++ )
                {
                    if ( openList[i].FCost < currentPathTile.FCost ||
                         openList[i].FCost == currentPathTile.FCost &&
                         openList[i].HCost < currentPathTile.HCost )
                    {
                        currentPathTile = openList[i];
                    }
                }

                openList.Remove( currentPathTile );
                closedList.Add( currentPathTile );

                if ( currentPathTile.TilePosition == destinationPathTile.TilePosition )
                    break;

                PathTile[] neighbours = GetPathTileNeighbors( currentPathTile );

                foreach ( PathTile neighbourPathTile in neighbours )
                {
                    if ( neighbourPathTile == null )
                        continue;

                    if ( IsTilePostionAtList( neighbourPathTile ) )
                        continue;

                    float movementCostToNeighbour = currentPathTile.GCost + GetDistance( currentPathTile, neighbourPathTile );
                    if ( movementCostToNeighbour < neighbourPathTile.GCost || !IsTilePositionAtOpenList( neighbourPathTile ) )
                    {
                        neighbourPathTile.SetGCost( movementCostToNeighbour );
                        neighbourPathTile.SetHCost( GetDistance( neighbourPathTile, destinationPathTile ) );
                        neighbourPathTile.SetParent( currentPathTile );

                        if ( !IsTilePositionAtOpenList( neighbourPathTile ) )
                            openList.Add( neighbourPathTile );
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

            finalPath.Reverse();
            Debug.Break();
            return finalPath;
        }

        private static bool IsTilePostionAtList( PathTile targetPathTile )
        {
            foreach ( PathTile pathTile in closedList )
            {
                if ( pathTile.TilePosition == targetPathTile.TilePosition )
                    return true;
            }

            return false;
        }

        private static bool IsTilePositionAtOpenList( PathTile targetPathTile )
        {
            foreach ( PathTile pathTile in openList )
            {
                if ( pathTile.TilePosition == targetPathTile.TilePosition )
                    return true;
            }

            return false;
        }

        private static float GetDistance( PathTile targetFromTile, PathTile targetToTile )
        {
            float dstX = Mathf.Abs( targetFromTile.TilePosition.x - targetToTile.TilePosition.x );
            float dstY = Mathf.Abs( targetFromTile.TilePosition.y - targetToTile.TilePosition.y );

            if ( dstX > dstY )
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }

        private static PathTile[] GetPathTileNeighbors( PathTile targetPathTile )
        {
            PathTile[] neighbors = new PathTile[4];
            neighbors[0] = GetNeighborAtDirection( targetPathTile, NeighborDirection.LEFT );
            neighbors[1] = GetNeighborAtDirection( targetPathTile, NeighborDirection.TOP );
            neighbors[2] = GetNeighborAtDirection( targetPathTile, NeighborDirection.RIGHT );
            neighbors[3] = GetNeighborAtDirection( targetPathTile, NeighborDirection.DOWN );

            return neighbors;
        }

        private static PathTile GetNeighborAtDirection( PathTile targetPathTile, NeighborDirection targetDirection )
        {
            Vector2Int neighborPosition = GetNeighbourPosition( targetPathTile, targetDirection );
            if ( !gridController.IsValidTilePosition( neighborPosition ) )
                return null;

            PathTile neighborPathTile = new PathTile( neighborPosition );
            return neighborPathTile;
        }

        private static Vector2Int GetNeighbourPosition( PathTile targetTile, NeighborDirection targetDirection )
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
