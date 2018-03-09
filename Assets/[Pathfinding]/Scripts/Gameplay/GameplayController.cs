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

        private void Awake()
        {
            gridController.Populate();
            fillController.Fill( gridController );
            visualizationController.Initialize( gridController );
        }
    }
}
