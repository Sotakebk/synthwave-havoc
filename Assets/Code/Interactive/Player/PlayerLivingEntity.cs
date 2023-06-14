using UnityEngine;

namespace TopDownShooter.Interactive.Player
{
    public class PlayerLivingEntity : BaseLivingEntity
    {
        protected override void Die()
        {
            Debug.Log("Player died!");
        }
    }
}