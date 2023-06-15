using UnityEngine;

namespace TopDownShooter
{
    public class ImpersistentObject : MonoBehaviour
    {
        public void DestroyOnLevelChange()
        {
            Destroy(gameObject);
        }
    }
}