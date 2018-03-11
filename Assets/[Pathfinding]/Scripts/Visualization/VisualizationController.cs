using System.Collections;
using BrunoMikoski.Events;
using BrunoMikoski.Pahtfinding.Grid;
using DG.Tweening;
using Pooling;
using UnityEngine;

namespace BrunoMikoski.Pahtfinding.Visualization
{
    public sealed class VisualizationController : MonoBehaviour
    {
        [SerializeField]
        private GameObject groundPrefab;

        [SerializeField]
        private GameObject[] typesPrefabs;

        private GameObject ground;
        private GridController grid;

        private GameObject[] tilesGameObjects;

        private int count;
        private Coroutine clearCountRoutine;
        private bool createObjects;

        public void Initialize( GridController targetGrid )
        {
            tilesGameObjects = new GameObject[targetGrid.Tiles.Length];
            grid = targetGrid;
            EventsDispatcher.Grid.OnTileTypeChangedEvent += OnTileTypeChanged;

            InitializeVisuals();
            CreateTerrain();
        }

        private void OnDestroy()
        {
            EventsDispatcher.Grid.OnTileTypeChangedEvent -= OnTileTypeChanged;
        }

        private void OnTileTypeChanged( Tile targetTile )
        {
            if ( !createObjects )
                return;

            if ( targetTile.TileType == TileType.EMPTY )
            {
                if ( tilesGameObjects[targetTile.Index] == null )
                    return;

                SimplePool.Despawn( tilesGameObjects[targetTile.Index] );
                tilesGameObjects[targetTile.Index] = null;
            }
            else
            {
                GameObject tileVisual = SimplePool.Spawn( typesPrefabs[(int) targetTile.TileType], transform );
                tileVisual.transform.localPosition =
                    new Vector3( targetTile.TilePosition.x, 0, targetTile.TilePosition.y );
                tilesGameObjects[targetTile.Index] = tileVisual;

                if ( targetTile.TileType == TileType.ROAD )
                {
                    tileVisual.transform.localPosition =
                        new Vector3( tileVisual.transform.localPosition.x, -5, tileVisual.transform.localPosition.z );
                    tileVisual.transform.DOLocalMoveY( 0, 0.3f )
                              .SetEase( Ease.OutBack )
                              .SetDelay( count * 0.05f );
                    count++;

                    if ( clearCountRoutine != null )
                    {
                        StopCoroutine( clearCountRoutine );
                        clearCountRoutine = null;
                    }

                    clearCountRoutine = StartCoroutine( ClearCountEnumerator() );
                }
            }
        }

        private IEnumerator ClearCountEnumerator()
        {
            yield return new WaitForSeconds( 1 );
            count = 0;
        }

        private void CreateTerrain()
        {
            ground = Instantiate( groundPrefab, Vector3.zero, Quaternion.identity );
            ground.transform.SetParent( transform, true );

            Vector3 finalSize = new Vector3( grid.GridSizeX, 1, grid.GridSizeY );
            ground.transform.localScale = finalSize;

            Vector3 finalLocalPosition =
                new Vector3( grid.GridSizeX * 0.5f - 0.5f, 0, grid.GridSizeY * 0.5f - 0.5f );
            ground.transform.localPosition = finalLocalPosition;

            for ( int i = 0; i < grid.Tiles.Length; i++ )
            {
                Tile gridTile = grid.Tiles[i];
                if ( gridTile.TileType != TileType.BLOCK )
                    continue;

                GameObject blockTile = SimplePool.Spawn( typesPrefabs[(int) TileType.BLOCK], transform );
                blockTile.transform.localPosition = new Vector3( gridTile.TilePosition.x, 0, gridTile.TilePosition.y );

                tilesGameObjects[i] = blockTile;
            }
        }

        private void InitializeVisuals()
        {
            int totalTiles = grid.Tiles.Length;

            int maxBlocksAmount = (int) (totalTiles * 0.3f);
            int maxTypesAmount = (int) (totalTiles * 0.3f);
            for ( int i = 0; i < typesPrefabs.Length; i++ )
            {
                GameObject typesPrefab = typesPrefabs[i];
                if ( typesPrefab == null )
                    continue;

                SimplePool.AddObjectsToPool( typesPrefab, maxTypesAmount );
            }

            SimplePool.AddObjectsToPool( typesPrefabs[(int) TileType.BLOCK], maxBlocksAmount );
        }

        public void ToggleVisualCreation( bool isOn )
        {
            createObjects = isOn;
        }
    }
}
