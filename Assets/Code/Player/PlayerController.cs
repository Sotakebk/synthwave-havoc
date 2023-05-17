using UnityEngine;

namespace TopDownShooter.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeReference] private Camera _camera;
        private CharacterController _characterController;

        // movement
        [SerializeField] private float _speed = 12;
        [SerializeField] private float _accelerationCoefficient = 0.25f;
        private Vector3 _acceleration;
        private Vector3 _currentVelocity;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            FollowMouseCursor();
            Move();
        }

        private void FollowMouseCursor()
        {
            var mousePos = Input.mousePosition;
            var ray = _camera.ScreenPointToRay(mousePos);

            var plane = new Plane(Vector3.up, 0);

            if (!plane.Raycast(ray, out float rayDistance))
            {
                return;
            }
            var hitPoint = ray.GetPoint(rayDistance);
            var direction = hitPoint - transform.position;
            var rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(direction.x, direction.z), Vector3.up);
            transform.rotation = rotation;
            if (DebugSettings.DrawDebugLines)
            {
                Debug.DrawLine(transform.position, ray.GetPoint(rayDistance), Color.red);
            }
        }

        private void Move()
        {
            var targetVelocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * _speed;
            _currentVelocity = Vector3.SmoothDamp(_currentVelocity, targetVelocity, ref _acceleration, _accelerationCoefficient);
            var realVelocity = _currentVelocity + new Vector3(0, -9.81f, 0);
            _characterController.Move(realVelocity * Time.deltaTime);
            if (DebugSettings.DrawDebugLines)
            {
                Debug.DrawRay(transform.position, targetVelocity, Color.green);
                Debug.DrawRay(transform.position, _currentVelocity, Color.cyan);
                Debug.DrawRay(transform.position, _acceleration, Color.blue);
            }
        }
    }
}
