using UnityEngine;

namespace TopDownShooter.Interactive.Enemy.Traits.Vision
{
    public class TemporaryXrayVision : BasicVision
    {
        [SerializeField] private float _xrayVisionSeconds = 1f;

        private float _xrayVisionActiveSeconds = 0;

        protected override bool CheckIfPlayerIsVisible()
        {
            var isPlayerVisible = base.CheckIfPlayerIsVisible();

            if (isPlayerVisible)
            {
                _xrayVisionActiveSeconds = 0;
                return true;
            }
            else
            {
                _xrayVisionActiveSeconds += Time.fixedDeltaTime;
                if (_xrayVisionActiveSeconds < _xrayVisionSeconds)
                {
                    var player = GameState.Current.PlayerCharacterController;
                    LastSeenPlayerPosition = player.transform.position;
                    LastSeenPlayerVelocity = player.GetLastVelocity();
                    return true;
                }
            }

            return false;
        }
    }
}