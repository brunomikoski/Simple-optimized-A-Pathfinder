using System;
using System.Collections.Generic;
using BrunoMikoski.Camera;
using BrunoMikoski.Events;
using BrunoMikoski.Pahtfinding.Fill;
using BrunoMikoski.Pahtfinding.Grid;
using BrunoMikoski.Pahtfinding.Visualization;
using UnityEngine;

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


            if ( selectedTilePosition == default(Vector2Int) )
            {
                selectedTilePosition = clickPosition;
                return;
            }

            if ( selectedTilePosition == clickPosition )
            {
                selectedTilePosition = default(Vector2Int);
                return;
            }

            List<Vector2Int> result = Pathfinder.GetPath( selectedTilePosition, clickPosition );
            result.Reverse();
            
            foreach ( Vector2Int vector2Int in result )
            {
                gridController.SetTileType(vector2Int, TileType.ROAD);
            }

            selectedTilePosition = default(Vector2Int);
        }
    }
}
