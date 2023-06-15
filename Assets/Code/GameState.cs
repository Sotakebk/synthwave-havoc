using System;
using System.Linq;
using TopDownShooter.Interactive.Enemy;
using TopDownShooter.Interactive.Player;
using UnityEngine;

namespace TopDownShooter
{
    [Serializable]
    public class GameState : MonoBehaviour
    {
        public static GameState Current { get; private set; }
        public PlayerCharacterController PlayerCharacterController { get; private set; }
        public CameraController PlayerCamera { get; private set; }

        [SerializeReference] GameManager _gameManager;
        [SerializeReference] UIController _uiController;
        [SerializeReference] MusicController _musicController;

        private void Awake()
        {
            if (Current != null)
                throw new InvalidOperationException();

            Current = this;
        }

        public void Unbind()
        {
            PlayerCharacterController = null;
            PlayerCamera = null;
        }

        public void Bind()
        {
            PlayerCharacterController = FindObjectOfType<PlayerCharacterController>();
            PlayerCamera = FindObjectOfType<CameraController>();
        }

        public void NotifyEnemyDied()
        {
            var enemies = FindObjectsOfType<EnemyAI>();
            if(enemies.Length == 0)
            {
                _gameManager.OpenNextLevel();
                _musicController.ShouldBeFiltered = false;
            }
        }

        public void NotifyPlayerDied()
        {
            _uiController.OnPlayerDeath();
            _musicController.ShouldBeFiltered = true;
        }
    }
}