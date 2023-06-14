using UnityEngine;

namespace TopDownShooter.Interactive.Enemy.Traits.Shooting
{
    public class ShootIfInLineOfSight : BaseShooting
    {
        [SerializeReference] private EnemyAI _enemyAI;
        [SerializeField] private float _maxAngle;
        [SerializeField] private float _maxDistance;

        protected virtual void FixedUpdate()
        {
            if (!_enemyAI.Vision.IsPlayerVisible)
                return;

            var sourcePosition = _weapon.OutputSource;
            var playerPosition = _enemyAI.Vision.LastSeenPlayerPosition;
            var directionToPlayer = (playerPosition - sourcePosition.position).normalized;
            var outputDirection = _weapon.OutputSource.forward;

            if (Vector3.Angle(directionToPlayer, outputDirection) > _maxAngle)
                return;

            if (Vector3.Distance(_enemyAI.transform.position, playerPosition) > _maxDistance)
                return;

            _weapon.TryShoot();
        }
    }
}