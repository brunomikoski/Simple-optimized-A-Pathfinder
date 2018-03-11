using BrunoMikoski.Pahtfinding.Grid;
using UnityEngine;

namespace BrunoMikoski.Pahtfinding.Fill
{
    public sealed class FillController : MonoBehaviour
    {
        [SerializeField]
        private float mimumValueAsBlock = 0.5f;

        [SerializeField]
        private float mapScale = 1f;

        [SerializeField]
        private int minimumRandomIndexOffset = 1;

        [SerializeField]
        private int maximumRandomIndexOffset = 10;

        [SerializeField]
        private bool fillMap;

        private GridController gridController;

        private void Fill()
        {
            Fill( gridController );
        }
        
        public void Fill(GridController targetGrid)
        {
            gridController = targetGrid;

            int randomOffset = Random.Range(minimumRandomIndexOffset, maximumRandomIndexOffset);

            for (int i = 0; i < targetGrid.Tiles.Length; i++)
            {
                Tile tile = targetGrid.Tiles[i];
                float noise = Mathf.PerlinNoise(((float)(tile.TilePosition.x+randomOffset) / targetGrid.GridSizeX) * mapScale,
                                                ((float)(tile.TilePosition.y+randomOffset) / targetGrid.GridSizeY) * mapScale);

                if ( noise > mimumValueAsBlock )
                {
                    tile.SetType( TileType.BLOCK );
                }
            }
        }

        private void Update()
        {
            if ( fillMap )
            {
                gridController.Clear();
                Fill(  );
                fillMap = false;
            }
        }
    }
}
