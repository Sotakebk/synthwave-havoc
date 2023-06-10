using System;
using UnityEngine;

namespace TopDownShooter.Enemy.Behaviours
{
    [Serializable]
    public abstract class BaseBehaviour : MonoBehaviour
    {
        protected EnemyAI _enemyAI;

        public abstract int Priority { get; }

        public virtual void Initialize(EnemyAI enemyAI)
        {
            _enemyAI = enemyAI;
        }

        public abstract bool CanEnter();

        public abstract void OnEnter();

        public abstract bool CanExit();

        public abstract void OnExit();

        public virtual void DrawDebugLines()
        {
        }

        public virtual void BehaviourUpdate()
        {
        }

        public virtual void ActiveBehaviourUpdate()
        {
        }
    }
}