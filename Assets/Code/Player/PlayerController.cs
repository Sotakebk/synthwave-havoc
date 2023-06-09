using UnityEngine;

namespace TopDownShooter.Player
{
    [RequireComponent(typeof(MovingPhysicsPlayerEntity))]
    public class PlayerController : MonoBehaviour
    {
        #region set from the inspector

        [SerializeReference] private Camera _camera;
        [SerializeReference] private Transform _model;

        #endregion set from the inspector

        private MovingPhysicsPlayerEntity _physicsPlayerEntity;
        private Vector3 _mouseHitPosition;

        private void Awake()
        {
            _physicsPlayerEntity = GetComponent<MovingPhysicsPlayerEntity>();
        }

        private void Update()
        {
            HandleMovementInput();
            HandleAim();

            if (DebugSettings.DrawPlayerMovementDebugLines)
                DrawDebugLines();
        }

        private void HandleAim()
        {
            var mousePos = Input.mousePosition;
            var ray = _camera.ScreenPointToRay(mousePos);

            var plane = new Plane(Vector3.up, 0);

            if (!plane.Raycast(ray, out float rayDistance))
                return;

            _mouseHitPosition = ray.GetPoint(rayDistance);
            var _facingDirection = (_mouseHitPosition - transform.position).normalized;

            _physicsPlayerEntity.SetFacingDirection(_facingDirection);
        }

        private void HandleMovementInput()
        {
            var inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            _physicsPlayerEntity.SetTargetVelocity(inputDirection);
            
            if (Input.GetKeyDown(KeyCode.E))
                _physicsPlayerEntity.RedirectOnNextFrame();
        }

        private void DrawDebugLines()
        {
            Debug.DrawLine(transform.position, _mouseHitPosition, Color.red);
        }
    }
}