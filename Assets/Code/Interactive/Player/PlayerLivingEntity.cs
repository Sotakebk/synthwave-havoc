using UnityEngine;

namespace TopDownShooter.Interactive.Player
{
    public class PlayerLivingEntity : BaseLivingEntity
    {
        protected override void Die()
        {
            GameState.Current.NotifyPlayerDied();
            gameObject.SetActive(false);
        }
    }
}