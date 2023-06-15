using UnityEngine;

namespace TopDownShooter.Interactive.Player
{
    [RequireComponent(typeof(PlayerCharacterController))]
    public class PlayerController : MonoBehaviour
    {
        private Camera _camera;
        [SerializeReference] private BaseWeapon _weapon;
        [SerializeReference] private PlayerLivingEntity _livingEntity;
        [SerializeReference] private float _staminaPerRedirect = 4f;
        [SerializeReference] private float _staminaPerDashSecond = 2f;

        private PlayerCharacterController _physicsPlayerEntity;
        private Vector3 _mouseHitPosition;
        private bool _continueDash = false;

        private void Start()
        {
            _physicsPlayerEntity = GetComponent<PlayerCharacterController>();
            _camera = GameState.Current.PlayerCamera.GetComponent<Camera>();
        }

        private void Update()
        {
            HandleMovementInput();
            HandleAim();
            HandleShooting();

            if (DebugSettings.DrawPlayerMovementLines)
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

            if (Input.GetKeyDown(KeyCode.E) && _livingEntity.Stamina > _staminaPerRedirect)
            {
                _physicsPlayerEntity.RedirectOnNextFrame();
                _livingEntity.ModifyStamina(-_staminaPerRedirect);
                _continueDash = true;
            }
            if (Input.GetKey(KeyCode.E) && _livingEntity.Stamina > _staminaPerDashSecond * Time.deltaTime * 2 && _continueDash)
            {
                _physicsPlayerEntity.ApplyDash();
                _physicsPlayerEntity.RedirectOnNextFrame();
                _livingEntity.ModifyStamina(-_staminaPerDashSecond * Time.deltaTime);
                _continueDash = true;
            }
            else
            {
                _continueDash = false;
            }
        }

        private void HandleShooting() {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                 _ = _weapon.TryShoot();
            }
        }

        private void DrawDebugLines()
        {
            Debug.DrawLine(transform.position, _mouseHitPosition, Color.red);
        }
    }
}