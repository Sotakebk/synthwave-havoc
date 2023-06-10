using System;
using TopDownShooter.Interactive.Player;
using UnityEngine;

namespace TopDownShooter
{
    [Serializable]
    public class GameState
    {
        [SerializeReference] private PlayerCharacterController _playerCharacterController;

        public PlayerCharacterController PlayerCharacterController => _playerCharacterController;
    }
}