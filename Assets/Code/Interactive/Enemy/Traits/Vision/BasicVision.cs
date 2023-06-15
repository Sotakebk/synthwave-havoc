using TopDownShooter.Helpers;
using UnityEngine;

namespace TopDownShooter.Interactive.Enemy.Traits.Vision
{
    public class BasicVision : BaseVision
    {
        [SerializeField] protected float _maxVisionRange = 100f;
        [SerializeField] protected LayerMask _ObstacleAndPlayerMask;

        public override float SecondsSincePlayerVisible { get; protected set; }
        public override float SecondsSincePlayerNotVisible { get; protected set; }
        public override bool IsPlayerVisible { get; protected set; }
        public override bool WasPlayerVisible { get; protected set; }

        /// <summary>
        /// Run from FixedUpdate every frame!
        /// Calls OnPlayerVisible() and OnPlayerNotVisible()
        /// </summary>
        public override void UpdateVision()
        {
            WasPlayerVisible = IsPlayerVisible;
            IsPlayerVisible = CheckIfPlayerIsVisible();

            if (IsPlayerVisible && !WasPlayerVisible)
            {
                SecondsSincePlayerVisible = 0;
                SecondsSincePlayerNotVisible = 0;
            }
            else if (!IsPlayerVisible && WasPlayerVisible)
            {
                SecondsSincePlayerVisible = 0;
                SecondsSincePlayerNotVisible = 0;
            }

            if (IsPlayerVisible)
            {
                SecondsSincePlayerVisible += Time.fixedDeltaTime;
            }
            else
            {
                SecondsSincePlayerNotVisible += Time.fixedDeltaTime;
            }
        }

        protected virtual bool CheckIfPlayerIsVisible()
        {
            var player = GameState.Current.PlayerCharacterController;
            var raySource = new Vector3(transform.position.x, Constants.InteractionHeight, transform.position.z);
            var ray = new Ray(raySource, (player.transform.position - transform.position).normalized);
            var hit = Physics.Raycast(ray, out var hitInfo, _maxVisionRange, _ObstacleAndPlayerMask.value, QueryTriggerInteraction.Ignore);

            if (!hit)
                return false;

            if (hitInfo.collider.gameObject != player.gameObject)
                return false;

            LastSeenPlayerPosition = player.transform.position;
            LastSeenPlayerVelocity = player.GetLastVelocity();
            return true;
        }

        protected virtual void Update()
        {
            if (DebugSettings.DrawEnemyVisionLines)
                DrawVisionLines();
        }

        protected virtual void DrawVisionLines()
        {
            Debug.DrawLine(transform.position, LastSeenPlayerPosition, Color.yellow);
        }
    }
}