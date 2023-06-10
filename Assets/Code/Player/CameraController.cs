using UnityEngine;

namespace TopDownShooter.Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float _firstSmoothTime = 0.125f;
        [SerializeField] private float _secondSmoothTime = 0.125f;
        [SerializeField] private float _targetHeight = 10f;
        [SerializeField] private float _minHeight = 5f;
        [SerializeField] private float _maxHeight = 25f;
        [SerializeField] private float _heightSmoothTime = 0.125f;

        private Vector2 _currentPosition;
        private Vector2 _currentVelocity;
        private Vector2 _positionVelocity;
        private Vector2 _velocityVelocity;
        private float _currentHeight;
        private float _heightVelocity;

        private void ResetPosition()
        {
            var targetPosition = GameState.Instance.PlayerCharacterController.transform.position.GetXZ();
            _currentHeight = _targetHeight;
            _currentPosition = GetTargetPosition(targetPosition, _currentHeight);
            transform.position = _currentPosition;

            _currentVelocity = default;
            _positionVelocity = default;
            _velocityVelocity = default;
            _heightVelocity = default;
        }

        private void Start()
        {
            ResetPosition();
        }

        private void LateUpdate()
        {
            HandleZoomInput();
            HandleMovement();

            if (DebugSettings.DrawCameraLines)
                DrawDebugLines();
        }

        private void HandleZoomInput()
        {
            _targetHeight += Input.mouseScrollDelta.y;
            _targetHeight = Mathf.Clamp(_targetHeight, _minHeight, _maxHeight);
            _currentHeight = Mathf.SmoothDamp(_currentHeight, _targetHeight, ref _heightVelocity, _heightSmoothTime, float.MaxValue, Time.deltaTime);
        }

        private void HandleMovement()
        {
            var targetPosition = GameState.Instance.PlayerCharacterController.transform.position.GetXZ();
            var delta = targetPosition - _currentPosition;
            _currentVelocity = Vector2.SmoothDamp(_currentVelocity, delta, ref _velocityVelocity, _firstSmoothTime, float.MaxValue, Time.deltaTime);

            var realTarget = targetPosition + _currentVelocity;
            _currentPosition = Vector2.SmoothDamp(_currentPosition, realTarget, ref _positionVelocity, _secondSmoothTime, float.MaxValue, Time.deltaTime);

            transform.position = GetTargetPosition(_currentPosition, _currentHeight);
        }

        private Vector3 GetTargetPosition(Vector2 playerPosition, float height)
        {
            return new Vector3(playerPosition.x, height, playerPosition.y);
        }

        private void DrawDebugLines()
        {
            var root = transform.position;
            root = new Vector3(root.x, 0, root.z);
            Debug.DrawRay(root, _currentVelocity.ToVector3XZ(), Color.red);
            Debug.DrawRay(root, _positionVelocity.ToVector3XZ(), Color.yellow);
            Debug.DrawRay(root, _velocityVelocity.ToVector3XZ(), Color.magenta);
        }
    }
}