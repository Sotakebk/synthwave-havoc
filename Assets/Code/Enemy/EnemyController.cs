using System.Linq;
using TopDownShooter.Player;
using UnityEngine;
using UnityEngine.AI;

namespace TopDownShooter.Enemy
{
    [RequireComponent(typeof(MovingPhysicsEntity))]
    public class EnemyController : MonoBehaviour
    {
        #region set from the inspector

        [SerializeField] private float _interactionHeight = 0.3f;

        // AI

        [SerializeReference] private GameObject _player;
        [SerializeReference] private EnemyGroupAvoidanceController _avoidanceController;
        [SerializeField] private float _maxDistanceFromPathEnd = 0.1f;
        [SerializeField] private float _maxDistanceFromPathStart = 0.1f;
        [SerializeField] private float _maxVisionRange = 100f;
        [SerializeField] private float _xrayVisionSeconds = 1f;
        [SerializeField] private float _secondsToForgetPlayer = 10f;
        [SerializeField] private float _secondsToNoticePlayer = 0.2f;
        [SerializeField] private float _secondsToChangeIdleTarget = 5f;
        [SerializeField] private float _idleTargetDistance = 1f;
        [SerializeField] private LayerMask _ObstacleAndPlayerMask;

        [SerializeField] private float _minMovementMargin = 0.2f;
        [SerializeField] private float _extrapolatedPlayerVelocity = 0.2f;

        [SerializeField] private float _followingPlayerSpeed = 10f;
        [SerializeField] private float _idleSpeed = 1f;

        #endregion set from the inspector

        private MovingPhysicsEntity _physicsEntity;
        private NavMeshPath _path;
        private MovingPhysicsPlayerEntity _physicsPlayerEntity;

        private bool _isFollowingPlayer = false;
        private bool _justToggledIsFollowingPlayer = false;
        private int _updatesSincePathRecalculated;

        private Vector3 _lastSeenPlayerPosition;
        private float _secondsSincePlayerLastVisible;
        private float _secondsSincePlayerVisible;

        private Vector3 _idleMovementTarget;
        private float _secondsSinceLastIdleMovementTarget;

        private Vector3 _input;
        private Vector3 _sideForce;
        private Vector3 _faceDirection;
        private Vector3 _smoothFaceDirection;

        private void Awake()
        {
            _path = new NavMeshPath();
            _physicsEntity = GetComponent<MovingPhysicsEntity>();
            _physicsPlayerEntity = _player.GetComponent<MovingPhysicsPlayerEntity>();
        }

        private void Update()
        {
            _smoothFaceDirection = Vector3.Slerp(_smoothFaceDirection, _faceDirection,Mathf.Clamp01(5f * Time.deltaTime));

            _physicsEntity.SetFacingDirection(_smoothFaceDirection);

            if (DebugSettings.DrawEnemyPathingDebugLines)
                DrawDebugLines();
        }

        private void FixedUpdate()
        {
            var isPlayerVisible = CheckIfPlayerIsVisible();

            if (_isFollowingPlayer)
            {
                // follow the path, recalculate if necessary
                if (UpdateShouldStopFollowingPlayer(isPlayerVisible))
                {
                    _isFollowingPlayer = false;
                    _justToggledIsFollowingPlayer = true;
                    IdleBehaviour();
                }
                else
                {
                    FollowPlayerBehaviour();
                }
            }
            else
            {
                // start following the player?
                if (UpdateShouldStartFollowingPlayer(isPlayerVisible))
                {
                    _isFollowingPlayer = true;
                    _justToggledIsFollowingPlayer = true;
                    FollowPlayerBehaviour();
                }
                else
                {
                    IdleBehaviour();
                }
            }
            _justToggledIsFollowingPlayer = false;
        }

        private void FollowPlayerBehaviour()
        {
            _physicsEntity.Speed = _followingPlayerSpeed;
            RecalculatePathToPlayer();
            FollowPath();
        }

        private void IdleBehaviour()
        {
            _physicsEntity.Speed = _idleSpeed;
            RecalculateIdlePath();
            FollowPath();
        }

        private void RecalculatePathToPlayer()
        {
            // if we just started following
            bool shouldRecalculatePath = !_justToggledIsFollowingPlayer;
            // or path couldn't be completed, and enough frames passed
            shouldRecalculatePath |= _path.status != NavMeshPathStatus.PathComplete && _updatesSincePathRecalculated > 30;

            // or, if the path so far was valid, maybe ve moved far enough?
            if (_path.corners.Length >= 2)
            {
                // did the player move far enough?
                shouldRecalculatePath |= _path.status == NavMeshPathStatus.PathComplete && Vector3.Distance(_path.corners.Last(), _player.transform.position) > _maxDistanceFromPathEnd;

                // did we just move far enough?
                var distToFirst = Vector3.Distance(_path.corners[0], transform.position);
                var distToSecond = Vector3.Distance(_path.corners[1], transform.position);
                var movedFromFirstPoint = distToFirst > _maxDistanceFromPathStart;
                var movedToSecondPoint = distToSecond < distToFirst;

                shouldRecalculatePath |= movedFromFirstPoint || movedToSecondPoint;
            }

            if (shouldRecalculatePath)
            {
                NavMesh.CalculatePath(transform.position, _lastSeenPlayerPosition, 1, _path);
                _updatesSincePathRecalculated = 0;
            }
            else
            {
                _updatesSincePathRecalculated++;
            }
        }

        private void RecalculateIdlePath()
        {
            // did we just enter this state?
            var shouldRecalculatePath = _justToggledIsFollowingPlayer || _secondsSinceLastIdleMovementTarget > _secondsToChangeIdleTarget;
            
            // or is there no valid path
            shouldRecalculatePath |= _path.corners.Length == 0;
            
            // if no, then compare distance with the last point
            if(!shouldRecalculatePath)
                shouldRecalculatePath |= Vector3.Distance(transform.position, _path.corners.Last()) < _minMovementMargin;

            if (_path.corners.Length >= 2)
            {
                // did we just move far enough?
                var distToSecond = Vector3.Distance(_path.corners[1], transform.position);

                shouldRecalculatePath |= distToSecond < _minMovementMargin;
            }

            if (shouldRecalculatePath)
            {
                var offset = Random.insideUnitCircle * _idleTargetDistance;
                _idleMovementTarget = transform.position + new Vector3(offset.x, 0, offset.y);
                NavMesh.CalculatePath(transform.position, _idleMovementTarget, 1, _path);

                _secondsSinceLastIdleMovementTarget = 0;
            }
            else
            {
                _secondsSinceLastIdleMovementTarget += Time.fixedDeltaTime;
            }
        }

        private void FollowPath()
        {
            var targetPosition = transform.position;

            if (_path.corners.Length >= 2)
            {
                targetPosition = _path.corners[1];
                
                // looks like the player is the path end
                if (_isFollowingPlayer && _path.corners.Length == 2)
                {
                    // we can use the player velocity here for future planning
                    targetPosition = _player.transform.position + _physicsPlayerEntity.GetLastVelocity() * _extrapolatedPlayerVelocity;
                }
            }

            var currentPosition = transform.position;
            var currentVelocity = _physicsEntity.GetLastVelocity();
            var difference = targetPosition - currentPosition;
            var distance = difference.magnitude;
            var direction = difference.normalized;

            var left = Vector3.Cross(direction, Vector3.up);
            var sideForce = left * Vector3.Cross(direction, currentVelocity).y;

            var a = 1 / (distance - _minMovementMargin);
            var distanceVariableInput = Mathf.Clamp01(a*(distance * 0.5f - _minMovementMargin));


            if (distanceVariableInput < _minMovementMargin)
                distanceVariableInput = 0;

            _sideForce = sideForce;
            var groupAvoidance = _avoidanceController?.GetAvoidanceVector() ?? Vector3.zero;

            _input = (sideForce + direction * distanceVariableInput + groupAvoidance).normalized;

            _faceDirection = direction;
            _physicsEntity.SetTargetVelocity(_input);
        }

        private bool UpdateShouldStartFollowingPlayer(bool currentlyVisible)
        {
            if (currentlyVisible)
                _secondsSincePlayerVisible += Time.fixedDeltaTime;
            else
                _secondsSincePlayerVisible = 0;

            if (_secondsSincePlayerVisible > _secondsToNoticePlayer)
                return true;

            return false;
        }

        private bool UpdateShouldStopFollowingPlayer(bool currentlyVisible)
        {
            if (!currentlyVisible)
                _secondsSincePlayerLastVisible += Time.fixedDeltaTime;
            else
                _secondsSincePlayerLastVisible = 0;

            if (_secondsSincePlayerLastVisible > _secondsToForgetPlayer)
                return true;

            return false;
        }

        private bool CheckIfPlayerIsVisible()
        {
            var raySource = new Vector3(transform.position.x, _interactionHeight, transform.position.z);
            var ray = new Ray(raySource, (_player.transform.position - transform.position).normalized);
            var hit = Physics.Raycast(ray, out var hitInfo, _maxVisionRange, _ObstacleAndPlayerMask.value, QueryTriggerInteraction.Ignore);

            // if we see the player, or if we have a few moments of xray still active...
            if ((hit && hitInfo.collider.gameObject == _player) || _secondsSincePlayerLastVisible < _xrayVisionSeconds)
                _lastSeenPlayerPosition = _player.transform.position;

            return hit && hitInfo.collider.gameObject == _player;
        }

        private void DrawDebugLines()
        {
            var corners = _path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
            {
                Debug.DrawLine(corners[i], corners[i + 1], Color.red);
            }
            if (_isFollowingPlayer)
                Debug.DrawLine(transform.position, _lastSeenPlayerPosition, Color.yellow);

            Debug.DrawRay(transform.position, _input, Color.green);
            Debug.DrawRay(transform.position, _sideForce, Color.cyan);
        }
    }
}