using UnityEngine;

namespace TopDownShooter.Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float _smoothTime = 0.125f;
        [SerializeReference] private Transform _player;
        private Vector3 _positionSmoothVelocity;

        private void LateUpdate()
        {
            var currentPosition = transform.position;
            var targetPosition = _player.transform.position + new Vector3(0, 10, 0);
            transform.position = Vector3.SmoothDamp(currentPosition, targetPosition, ref _positionSmoothVelocity, _smoothTime);
        }
    }
}
