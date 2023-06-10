using System;
using UnityEngine;

namespace TopDownShooter.Enemy.Traits.MovementEffects
{
    [Serializable]
    public struct MovementEffectWeightPair
    {
        [SerializeReference] private BaseMovementEffect _effect;
        [SerializeField] private float _weight;

        public BaseMovementEffect Effect => _effect;
        public float Weight => _weight;
    }
}