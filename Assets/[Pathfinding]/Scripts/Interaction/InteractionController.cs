using BrunoMikoski.Camera;
using BrunoMikoski.Pahtfinding.Generic;
using UnityEngine;

namespace BrunoMikoski.Pahtfinding
{
    public sealed class InteractionController : MonoBehaviour
    {
        private CameraController cameraController;
        private bool initialized;

        public void Initialize( CameraController targetCameraController )
        {
            cameraController = targetCameraController;
            initialized = true;
        }


        private void Update()
        {
            if(!initialized)
                return;

            if ( Input.GetMouseButtonDown( 0 ) )
            {
                RaycastHit raycastHit;
                Ray ray = cameraController.Camera.ScreenPointToRay(Input.mousePosition);

                if ( Physics.Raycast( ray, out raycastHit, Layers.GROUND_LAYER_MASK ) )
                {
                    Events.EventsDispatcher.Interaction.DispatchOnUserClickOnTilePositionEvent(
                        Mathf.RoundToInt( raycastHit.point.x ), Mathf.RoundToInt( raycastHit.point.z ) );
                }
            }
        }
        
        
        
        
    }
}
