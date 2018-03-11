using System;
using BrunoMikoski.Pahtfinding;
using BrunoMikoski.Pahtfinding.Grid;

namespace BrunoMikoski.Events
{
    public static class EventsDispatcher
    {
        public static class Grid
        {
            public static Action<Tile> OnTileTypeChangedEvent;


            public static void DispatchOnTileTypeChangedEvent( Tile targetTile )
            {
                if ( OnTileTypeChangedEvent != null )
                    OnTileTypeChangedEvent( targetTile );
            }
        }

        public static class Interaction
        {

            public static Action<int, int> OnUserClickOnTilePositionEvent;
            public static void DispatchOnUserClickOnTilePositionEvent( int targetX, int targetZ )
            {
                if ( OnUserClickOnTilePositionEvent != null )
                    OnUserClickOnTilePositionEvent( targetX, targetZ );
            }
        }
    }
}
