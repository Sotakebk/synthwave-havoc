namespace TopDownShooter.Player
{
    public class MovingPhysicsPlayerEntity : MovingPhysicsEntity
    {
        private bool _redirectOnNextFrame = false;

        public void RedirectOnNextFrame()
        {
            _redirectOnNextFrame = true;
        }

        protected override void Update()
        {
            if (DebugSettings.DrawPlayerMovementDebugLines)
                DrawDebugLines();
        }

        protected override void HandleMovement()
        {
            base.HandleMovement();

            if (_redirectOnNextFrame)
            {
                _rigidbody.velocity = _facingDirection * _rigidbody.velocity.magnitude;
                _redirectOnNextFrame = false;
            }
        }
    }
}