using UnityEngine;

namespace TopDownShooter.Enemy.Traits.MovementEffects
{
    public class AvoidCollisionWithStaticObjects : BaseMovementEffect
    {
        [SerializeField] private LayerMask _staticLayerMask;
        [SerializeField] private float _startRadius = 0.3f;
        [SerializeField] private float _endRadius = 2f;
        [SerializeField] private float _startForce = 1f;
        [SerializeField] private float _endForce = 0f;
        [SerializeField] private int rayCount = 8;

        private void Update()
        {
            if (DebugSettings.DrawAvoidCollisionWithStaticObjectLines)
            {
                Debug.DrawRay(transform.position, GetAvoidanceVector() * rayCount, Color.red);
            }
        }

        public override Vector3 GetMovementEffect()
        {
            return GetAvoidanceVector();
        }

        public Vector3 GetAvoidanceVector()
        {
            var sum = Vector3.zero;

            for (int i = 0; i < rayCount; i++)
            {
                var angle = i * rayCount / (2f * Mathf.PI);

                var direction = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));

                var raySource = new Vector3(transform.position.x, Constants.InteractionHeight, transform.position.z);
                var ray = new Ray(raySource, direction);
                var hit = Physics.Raycast(ray, out var hitInfo, _endRadius, _staticLayerMask.value, QueryTriggerInteraction.Ignore);

                if (!hit)
                    continue;

                var distance = hitInfo.distance;

                var force = direction * -MathHelper.ClampAndLinearMap(distance, _startRadius, _startForce, _endRadius, _endForce);
                sum += force;
            }

            return sum * (1f / rayCount);
        }
    }
}