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
            int x = 0;
            int y = 0;

            int randomOffset = Random.Range(minimumRandomIndexOffset, maximumRandomIndexOffset);

            for (int i = 0; i < targetGrid.TileTypes.Length; i++)
            {
                targetGrid.IndexToTilePos(i, out x, out y);
                
                float noise = Mathf.PerlinNoise(((float)(x+randomOffset) / targetGrid.GridSizeX) * mapScale,
                                                ((float)(y+randomOffset) / targetGrid.GridSizeY) * mapScale);

                targetGrid.SetTileBlocked(x, y, noise > mimumValueAsBlock);
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
