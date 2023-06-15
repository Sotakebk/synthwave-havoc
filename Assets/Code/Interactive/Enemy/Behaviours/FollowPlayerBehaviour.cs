using System.Linq;
using TopDownShooter.Helpers;
using TopDownShooter.Interactive.Enemy.Traits.PathFollowing;
using TopDownShooter.Interactive.Enemy.Traits.Vision;
using UnityEngine;

namespace TopDownShooter.Interactive.Enemy.Behaviours
{
    public class FollowPlayerBehaviour : BaseBehaviour
    {
        public override int Priority => _behaviourPriority;
        [SerializeField] protected int _behaviourPriority = 1;

        [SerializeField] protected float _secondsToFocusOnPlayer = 0.5f;
        [SerializeField] protected float _secondsToForgetPlayer = 5;
        [SerializeField] protected float _minRepathDistance = 0.1f;

        [SerializeField] protected float _minMovementDistanceVelocity = 0.5f;
        [SerializeField] protected float _minMovementDistance = 0f;
        [SerializeField] protected float _maxMovementDistanceVelocity = 1f;
        [SerializeField] protected float _maxMovementDistance = 1f;

        [SerializeField] protected float _extrapolatePlayerVelocity = 0.5f;

        [SerializeField] protected float _speed = 10f;

        protected int _updatesSincePathRecalculated;
        protected BasePathing _pathing;
        protected BaseVision _vision;

        protected Vector3 _input;
        protected Vector3 _sideForce;
        protected Vector3 _faceDirection;
        protected Vector3 _movementTarget;

        public override void Initialize(EnemyAI enemyAI)
        {
            base.Initialize(enemyAI);
            _pathing = enemyAI.Pathing;
            _vision = enemyAI.Vision;
        }

        public override bool CanEnter()
        {
            return _vision.SecondsSincePlayerVisible > _secondsToFocusOnPlayer;
        }

        public override bool CanExit()
        {
            return _vision.SecondsSincePlayerNotVisible > _secondsToForgetPlayer;
        }

        public override void OnEnter()
        {
            _updatesSincePathRecalculated = -1;
            _enemyAI.CharacterController.Speed = _speed;
            _enemyAI.Shooting?.SetShootingEnable(true);
        }

        public override void OnExit()
        {
            _updatesSincePathRecalculated = 0;
            _enemyAI.Shooting?.SetShootingEnable(false);
        }

        public override void ActiveBehaviourUpdate()
        {
            RegeneratePathIfNecessary();
            FollowPath();

            _updatesSincePathRecalculated++;
        }

        protected virtual void RegeneratePathIfNecessary()
        {
            var path = _pathing.Path;
            var playerPosition = _vision.LastSeenPlayerPosition;

            // did we just enter this state?
            var shouldRegeneratePath = _updatesSincePathRecalculated < 0;
            // or is there no valid path
            shouldRegeneratePath |= path.corners.Length < 2;

            if (path.corners.Length >= 2)
            {
                // did we just move far enough?
                var distToFirst = Vector3.Distance(path.corners.First(), transform.position);
                // did the player move far enough?
                var distToLast = Vector3.Distance(playerPosition, transform.position);

                shouldRegeneratePath |= distToFirst > _minRepathDistance;
                shouldRegeneratePath |= distToLast > _minRepathDistance;
            }

            if (shouldRegeneratePath)
            {

                _enemyAI.Pathing.CalculatePathTo(playerPosition);
                _updatesSincePathRecalculated = 0;
            }
        }

        protected virtual void FollowPath()
        {
            var path = _pathing.Path;
            var position = transform.position;
            var velocity = _enemyAI.CharacterController.GetLastVelocity();

            Vector3 target;
            if (path.corners.Length > 2)
                target = PathHelper.GetNextPointToFollowWithSomeDistanceEnforced(position, path.corners, 0.5f);
            else
                target = _vision.LastSeenPlayerPosition + _vision.LastSeenPlayerVelocity * _extrapolatePlayerVelocity;

            var difference = target - position;
            var distance = difference.magnitude;
            var direction = difference.normalized;

            var straighteningVelocity = PathHelper.GetConstraintVelocityToFollowPath(position, target, velocity);

            var mappedDistance = MathHelper.ClampAndLinearMap(distance, _minMovementDistance, _minMovementDistanceVelocity, _maxMovementDistance, _maxMovementDistanceVelocity);
            var forwardVelocity = direction * mappedDistance;

            var (effects, _) = _enemyAI.SumMovementWeights();

            _sideForce = straighteningVelocity;
            _faceDirection = direction;
            _input = (straighteningVelocity + forwardVelocity + effects).ClampToUnitSphere();
            _movementTarget = target;

            _enemyAI.SetFacingDirection(_faceDirection);
            _enemyAI.SetTargetVelocity(_input);
        }

        public override void DrawDebugLines()
        {
            base.DrawDebugLines();
            Debug.DrawRay(transform.position, _input, Color.blue);
            Debug.DrawRay(transform.position, _sideForce, Color.green);
            Debug.DrawRay(transform.position, _faceDirection, Color.magenta);
            Debug.DrawLine(transform.position, _movementTarget, Color.grey);
        }
    }
}