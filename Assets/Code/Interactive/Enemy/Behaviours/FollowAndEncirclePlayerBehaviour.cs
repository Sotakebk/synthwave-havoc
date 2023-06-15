using TopDownShooter.Helpers;
using UnityEngine;

namespace TopDownShooter.Interactive.Enemy.Behaviours
{
    public class FollowAndEncirclePlayerBehaviour : FollowPlayerBehaviour
    {
        [SerializeField] protected float _targetEncirclingDistance = 4f;
        [SerializeField] protected float _minEncirclingDistance = 0.5f;
        [SerializeField] protected float _minEncirclingDistanceVelocity = 0f;
        [SerializeField] protected float _maxEncirclingDistance = 1f;
        [SerializeField] protected float _maxEncirclingDistanceVelocity = 1f;
        [SerializeField] protected float _avoidanceVelocity = 7f;
        [SerializeField] protected float _straightening = 0.25f;

        protected override void FollowPath()
        {
            var path = _pathing.Path;

            if (path.corners.Length != 2)
            {
                base.FollowPath();
                return;
            }

            // so we have only two points, we can use the encircling behaviour now
            var position = transform.position;
            var velocity = _enemyAI.CharacterController.GetLastVelocity();

            var playerPosition = _vision.LastSeenPlayerPosition;
            var playerVelocity = _vision.LastSeenPlayerVelocity;

            var target = playerPosition + playerVelocity * _extrapolatePlayerVelocity;

            var difference = target - position;
            var directionToPlayer = difference.normalized;
            target += -directionToPlayer * _targetEncirclingDistance;

            difference = target - position;
            var distance = difference.magnitude;
            var direction = difference.normalized;

            var playerSideSpeed = Vector3.Cross(direction, playerVelocity).y;
            var avoidanceVelocity = Vector3.Cross(direction, Vector3.up) * playerSideSpeed;

            var straighteningVelocity = PathHelper.GetConstraintVelocityToFollowPath(position, target, velocity) * _straightening;

            var mappedDistance = MathHelper.ClampAndLinearMap(distance, _minEncirclingDistance, _minEncirclingDistanceVelocity, _maxEncirclingDistance, _maxEncirclingDistanceVelocity);
            var forwardVelocity = direction * mappedDistance;

            var (effects, _) = _enemyAI.SumMovementWeights();

            _sideForce = avoidanceVelocity;
            _faceDirection = directionToPlayer;
            _input = (avoidanceVelocity + straighteningVelocity + forwardVelocity + effects).ClampToUnitSphere();
            _movementTarget = target;

            _enemyAI.SetFacingDirection(directionToPlayer);
            _enemyAI.SetTargetVelocity(_input);
        }

        public override void DrawDebugLines()
        {
            base.DrawDebugLines();
            Debug.DrawRay(transform.position, _input, Color.blue);
            Debug.DrawRay(transform.position, _sideForce, Color.green);
            Debug.DrawRay(transform.position, _faceDirection, Color.magenta);
            Debug.DrawLine(transform.position, _movementTarget, Color.black);
        }
    }
}