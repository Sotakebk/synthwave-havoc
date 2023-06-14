using System;
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

    }
}