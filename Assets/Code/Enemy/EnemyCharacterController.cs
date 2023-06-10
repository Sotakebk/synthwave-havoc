namespace TopDownShooter.Enemy
{
    public class EnemyCharacterController : BaseCharacterController
    {
        protected override void Update()
        {
            if (DebugSettings.DrawEnemyMovementLines)
            {
                DrawMovementLines();
            }
        }
    }
}