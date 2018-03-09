using BrunoMikoski.Events;
using BrunoMikoski.Pahtfinding.Grid;
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

        public void Initialize(GridController targetGrid)
        {
            grid = targetGrid;
            EventsDispatcher.Grid.OnTileTypeChangedEvent += OnTileTypeChanged;
            
            InitializeVisuals();
            CreateTerrain();
        }

        private void OnDestroy()
        {
            EventsDispatcher.Grid.OnTileTypeChangedEvent -= OnTileTypeChanged;
        }

        private void OnTileTypeChanged( int targetX, int targetY, TileType targetTileType )
        {
            GameObject blockTile = SimplePool.Spawn(typesPrefabs[(int) targetTileType], transform);
            blockTile.transform.localPosition = new Vector3(targetX, 0, targetY);
        }

        private void CreateTerrain()
        {
            ground = Instantiate(groundPrefab, Vector3.zero, Quaternion.identity);
            ground.transform.SetParent(transform, true);

            Vector3 finalSize = new Vector3(grid.GridSizeX, 1, grid.GridSizeY);
            ground.transform.localScale = finalSize;

            Vector3 finalLocalPosition =
                new Vector3(grid.GridSizeX * 0.5f - 0.5f, 0, grid.GridSizeY * 0.5f - 0.5f);
            ground.transform.localPosition = finalLocalPosition;
            
            
            int x;
            int y;
            for ( int i = 0; i < grid.TileTypes.Length; i++ )
            {
                TileType gridTileType = grid.TileTypes[i];
                if ( gridTileType != TileType.BLOCK )
                    continue;

                grid.IndexToTilePos( i, out x, out y );

                GameObject blockTile = SimplePool.Spawn( typesPrefabs[(int) TileType.BLOCK], transform );
                blockTile.transform.localPosition = new Vector3( x, 0, y );
            }
        }
     
        private void InitializeVisuals()
        {
            int totalTiles = grid.TileTypes.Length;

            int maxBlocksAmount = (int) (totalTiles * 0.3f);
            int maxTypesAmount = (int) (totalTiles * 0.3f);
            for (int i = 0; i < typesPrefabs.Length; i++)
            {
                GameObject typesPrefab = typesPrefabs[i];
                if (typesPrefab == null)
                    continue;

                SimplePool.AddObjectsToPool(typesPrefab, maxTypesAmount);
            }
            SimplePool.AddObjectsToPool(typesPrefabs[(int) TileType.BLOCK], maxBlocksAmount);
        }
    }
}
