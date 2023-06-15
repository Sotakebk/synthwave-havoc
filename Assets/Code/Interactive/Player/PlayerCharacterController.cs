using UnityEngine;

namespace TopDownShooter.Interactive.Player
{
    public class PlayerCharacterController : BaseCharacterController
    {
        [SerializeField] private float _dashVelocity;
        private bool _redirectOnNextFrame = false;
        private bool _applyDash = false;

        public void RedirectOnNextFrame()
        {
            _redirectOnNextFrame = true;
        }

        public void ApplyDash()
        {
            _applyDash = true;
        }

        protected override void Update()
        {
            if (DebugSettings.DrawPlayerMovementLines)
                DrawMovementLines();
        }

        protected override void HandleMovement()
        {
            if (_redirectOnNextFrame)
            {
                _targetVelocity = _facingDirection;
            }

            base.HandleMovement();

            if (_redirectOnNextFrame)
            {
                _rigidbody.velocity = _facingDirection * _rigidbody.velocity.magnitude;
                _redirectOnNextFrame = false;
            }
            if (_applyDash)
            {
                _rigidbody.velocity += _facingDirection * _dashVelocity * Time.fixedDeltaTime;
                _applyDash = false;
            }
        }
    }
}