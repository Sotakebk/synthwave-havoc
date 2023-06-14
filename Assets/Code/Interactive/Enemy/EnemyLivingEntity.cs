namespace TopDownShooter.Interactive.Enemy
{
    public class EnemyLivingEntity : BaseLivingEntity
    {
        protected override void Die()
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}