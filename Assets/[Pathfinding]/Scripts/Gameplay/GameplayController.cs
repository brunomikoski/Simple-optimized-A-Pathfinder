using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting.Metadata;
using BrunoMikoski.Camera;
using BrunoMikoski.Events;
using BrunoMikoski.Pahtfinding.Fill;
using BrunoMikoski.Pahtfinding.Grid;
using BrunoMikoski.Pahtfinding.Visualization;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace BrunoMikoski.Pahtfinding.Gameplay
{
    public sealed class GameplayController : MonoBehaviour
    {
        [SerializeField]
        private GridController gridController;
        [SerializeField]
        private FillController fillController;
        [SerializeField]
        private VisualizationController visualizationController;
        [SerializeField]
        private CameraController cameraController;
        [SerializeField]
        private InteractionController interactionController;

        private Vector2Int selectedTilePosition;
        private bool hasFirstNodeSelected = false;

        private void Awake()
        {
            gridController.GenerateTiles();
            fillController.Fill( gridController );
            visualizationController.Initialize( gridController );
            cameraController.Setup( gridController );
            interactionController.Initialize( cameraController );
            Pathfinder.Initialize( gridController );
            
            
            EventsDispatcher.Interaction.OnUserClickOnTilePositionEvent += OnUserClickOnTilePosition;
        }

        private void OnDestroy()
        {
            EventsDispatcher.Interaction.OnUserClickOnTilePositionEvent -= OnUserClickOnTilePosition;
        }

        private void OnUserClickOnTilePosition( int targetX, int targetY )
        {
            Vector2Int clickPosition = new Vector2Int( targetX, targetY );
            if ( !gridController.IsValidTilePosition( clickPosition ) )
                return;


            if ( !hasFirstNodeSelected )
            {
                selectedTilePosition = clickPosition;
                hasFirstNodeSelected = true;
                return;
            }
            

            if ( selectedTilePosition == clickPosition )
            {
                hasFirstNodeSelected = false;
                return;
            }

            List<Vector2Int> result = Pathfinder.GetPath( selectedTilePosition, clickPosition );
            foreach ( Vector2Int vector2Int in result )
            {
                gridController.SetTileType(vector2Int, TileType.ROAD);
            }

            hasFirstNodeSelected = false;
        }
        
        
        public void PrintBiggestPath()
        {
            OnUserClickOnTilePosition(0, 0);
            OnUserClickOnTilePosition(gridController.GridSizeX - 1, gridController.GridSizeY - 1);
        }
    }
}
