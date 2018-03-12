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

        [SerializeField]
        private bool printBiggestPath;

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

            List<Tile> result = Pathfinder.GetPath( selectedTilePosition, clickPosition );
            int resultCount = result.Count;
            for ( int i = 0; i < resultCount; ++i )
            {
                Tile tile = result[i];
                tile.SetType( TileType.ROAD );
            }

            hasFirstNodeSelected = false;
        }
        

        [ContextMenu("Print Biggest Path")]
        public void PrintBiggestPath()
        {
            visualizationController.ToggleObjectCreationg( false );
            OnUserClickOnTilePosition(0, 0);
            OnUserClickOnTilePosition(gridController.GridSizeX - 1, gridController.GridSizeY - 1);
            visualizationController.ToggleObjectCreationg( true );

        }

        private void Update()
        {
            if ( printBiggestPath )
            {
                printBiggestPath = false;
                visualizationController.ToggleObjectCreationg( false );
                PrintBiggestPath();
                visualizationController.ToggleObjectCreationg( true );
            }
        }
    }
}
