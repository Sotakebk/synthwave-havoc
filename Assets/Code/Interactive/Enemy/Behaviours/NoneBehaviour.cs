namespace TopDownShooter.Interactive.Enemy.Behaviours
{
    public class NoneBehaviour : BaseBehaviour
    {
        public override int Priority => int.MinValue + 1;

        public override bool CanEnter()
        {
            return true;
        }

        public override bool CanExit()
        {
            return true;
        }

        public override void OnEnter()
        {
            _enemyAI.Shooting?.SetShootingEnable(false);
        }

        public override void OnExit()
        {
        }
    }
}