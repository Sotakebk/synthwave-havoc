using TopDownShooter.Helpers;
using UnityEngine;

namespace TopDownShooter.Interactive.Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float _firstSmoothTime = 0.125f;
        [SerializeField] private float _secondSmoothTime = 0.125f;
        [SerializeField] private float _targetHeight = 10f;
        [SerializeField] private float _minHeight = 5f;
        [SerializeField] private float _maxHeight = 25f;
        [SerializeField] private float _heightSmoothTime = 0.125f;
        [SerializeField] private float _shakeTargetPositionRecoveryTime = 0.5f;
        [SerializeField] private float _shakePositionFollowTime = 0.1f;
        [SerializeField] private float _shakeImpact = 0.1f;

        private Vector2 _currentPosition;
        private Vector2 _currentVelocity;
        private Vector2 _positionVelocity;
        private Vector2 _velocityVelocity;
        private float _currentHeight;
        private float _heightVelocity;

        private Vector2 _targetShakeOffset;
        private Vector2 _shakeOffsetRecoveryVelocity;
        private Vector2 _currentShakeOffset;
        private Vector2 _currentShakeOffsetVelocity;

        public void AddShake(Vector2 impulse)
        {
            _targetShakeOffset += impulse;
        }

        private void ResetPosition()
        {
            var targetPosition = GameManager.CurrentState.PlayerCharacterController.transform.position.GetXZ();
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
            HandleShake();
            UpdateCameraPosition();

            if (DebugSettings.DrawCameraLines)
                DrawDebugLines();
        }

        private void HandleShake()
        {
            _targetShakeOffset = Vector2.SmoothDamp(_targetShakeOffset, Vector2.zero, ref _shakeOffsetRecoveryVelocity, _shakeTargetPositionRecoveryTime, float.MaxValue, Time.deltaTime);
            _currentShakeOffset = Vector2.SmoothDamp(_currentShakeOffset, _targetShakeOffset, ref _currentShakeOffsetVelocity, _shakePositionFollowTime, float.MaxValue, Time.deltaTime);
        }

        private void HandleZoomInput()
        {
            _targetHeight += Input.mouseScrollDelta.y;
            _targetHeight = Mathf.Clamp(_targetHeight, _minHeight, _maxHeight);
            _currentHeight = Mathf.SmoothDamp(_currentHeight, _targetHeight, ref _heightVelocity, _heightSmoothTime, float.MaxValue, Time.deltaTime);
        }

        private void HandleMovement()
        {
            var targetPosition = GameManager.CurrentState.PlayerCharacterController.transform.position.GetXZ();
            var delta = targetPosition - _currentPosition;
            _currentVelocity = Vector2.SmoothDamp(_currentVelocity, delta, ref _velocityVelocity, _firstSmoothTime, float.MaxValue, Time.deltaTime);

            var realTarget = targetPosition + _currentVelocity;
            _currentPosition = Vector2.SmoothDamp(_currentPosition, realTarget, ref _positionVelocity, _secondSmoothTime, float.MaxValue, Time.deltaTime);
        }

        private void UpdateCameraPosition()
        {
            transform.position = GetTargetPosition(_currentPosition + _currentShakeOffset * _shakeImpact, _currentHeight);
        }

        private Vector3 GetTargetPosition(Vector2 targetPosition, float height)
        {
            return new Vector3(targetPosition.x, height, targetPosition.y);
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