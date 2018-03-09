using System;
using BrunoMikoski.Pahtfinding.Grid;

namespace BrunoMikoski.Events
{
    public static class EventsDispatcher
    {
        public static class Grid
        {
            public static Action<int, int, TileType> OnTileTypeChangedEvent;

            public static void DispatchOnTileTypeChangedEvent( int x, int y, TileType targetType )
            {
                if ( OnTileTypeChangedEvent != null )
                    OnTileTypeChangedEvent( x, y, targetType );
            }
        }
    }
}
