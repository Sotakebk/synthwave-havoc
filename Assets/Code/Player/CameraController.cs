using UnityEngine;

namespace TopDownShooter.Player
{
    public class CameraController : MonoBehaviour
    {
        #region set from the inspector

        [SerializeField] private float _firstSmoothTime = 0.125f;
        [SerializeField] private float _secondSmoothTime = 0.125f;
        [SerializeField] private float _targetHeight = 10f;
        [SerializeField] private float _minHeight = 5f;
        [SerializeField] private float _maxHeight = 25f;
        [SerializeField] private float _heightSmoothTime = 0.125f;

        [SerializeReference] private GameObject _player;

        #endregion set from the inspector

        private Vector3 _currentPosition;
        private Vector3 _currentVelocity;
        private Vector3 _positionVelocity;
        private Vector3 _velocityVelocity;
        private float _currentHeight;
        private float _heightVelocity;

        private void Start()
        {
            _currentHeight = _targetHeight;
            var target = _player.transform.position;
            transform.position = target + new Vector3(0, _currentHeight, 0);

        }

        private void LateUpdate()
        {
            HandleZoomInput();
            HandleMovement();

            if (DebugSettings.DrawCameraDebugLines)
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
            var target = _player.transform.position;
            var delta = target - _currentPosition;
            _currentVelocity = Vector3.SmoothDamp(_currentVelocity, delta, ref _velocityVelocity, _firstSmoothTime, float.MaxValue, Time.deltaTime);

            var realTarget = target + _currentVelocity;
            _currentPosition = Vector3.SmoothDamp(_currentPosition, realTarget, ref _positionVelocity, _secondSmoothTime, float.MaxValue, Time.deltaTime);

            transform.position = _currentPosition + new Vector3(0, _currentHeight, 0);
        }

        private void DrawDebugLines()
        {
            var root = transform.position;
            root = new Vector3(root.x, 0, root.z);
            Debug.DrawRay(root, _currentVelocity, Color.red);
            Debug.DrawRay(root, _positionVelocity, Color.yellow);
            Debug.DrawRay(root, _velocityVelocity, Color.magenta);
        }
    }
}