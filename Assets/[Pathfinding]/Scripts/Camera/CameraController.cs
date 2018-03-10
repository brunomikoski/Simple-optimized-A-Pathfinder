using BrunoMikoski.Pahtfinding.Grid;
using UnityEngine;

namespace BrunoMikoski.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private new UnityEngine.Camera camera;

        [SerializeField]
        private float minimumHeight = 10;

        [SerializeField]
        private float maximumHeight = 30;

        [SerializeField]
        private float smoothCameraFactor = 5;

        [SerializeField]
        private float zoomStepFactor = 5;

        [SerializeField]
        private float borderMovementArea = 0.05f;

        [SerializeField]
        private float edgeMovement = 5;
        
        private Vector3 targetCameraPosition;
        private bool initialized;

        public void Setup( GridController targetGridController )
        {
            transform.position = new Vector3( targetGridController.GridSizeX * 0.5f, 20,
                                              targetGridController.GridSizeY * 0.5f );
            targetCameraPosition = transform.position;

            initialized = true;
        }

        private void LateUpdate()
        {
            if ( !initialized )
                return;

            CheckForEdgeMovement();
            CheckForZoom();
            MoveCamera();
        }

        private void CheckForEdgeMovement()
        {
            Vector3 viewPortPosition = camera.ScreenToViewportPoint( Input.mousePosition );

            if ( viewPortPosition.x > 0 && viewPortPosition.x < borderMovementArea )
            {
                targetCameraPosition.x -= edgeMovement;
            }
            else if ( viewPortPosition.x < 1.0f && viewPortPosition.x > 1.0f - borderMovementArea )
            {
                targetCameraPosition.x += edgeMovement;
            }

            if ( viewPortPosition.y > 0 && viewPortPosition.y < borderMovementArea )
            {
                targetCameraPosition.z -= edgeMovement;
            }
            else if ( viewPortPosition.y < 1.0f && viewPortPosition.y > 1.0f - borderMovementArea )
            {
                targetCameraPosition.z += edgeMovement;
            }
        }

        private void MoveCamera()
        {
            transform.position = Vector3.Lerp( transform.position, targetCameraPosition,
                                               Time.deltaTime * smoothCameraFactor );
        }

        private void CheckForZoom()
        {
            Vector3 currentPosition = targetCameraPosition;
            currentPosition.y -= (Input.mouseScrollDelta.y * zoomStepFactor);
            currentPosition.y = Mathf.Clamp( currentPosition.y, minimumHeight, maximumHeight );

            targetCameraPosition = currentPosition;
        }
    }
}
