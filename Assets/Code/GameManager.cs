using UnityEngine;

namespace TopDownShooter
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameState _gameState;

        public static GameState CurrentState { get; private set; }

        private void Awake()
        {
            CurrentState = _gameState;
        }
    }
}