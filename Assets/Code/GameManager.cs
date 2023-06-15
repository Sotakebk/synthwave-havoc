using TopDownShooter.World.Construction;
using TopDownShooter.World.Data;
using UnityEngine;

namespace TopDownShooter
{
    [RequireComponent(typeof(LevelContainer))]
    [RequireComponent(typeof(WorldBuilder))]
    [RequireComponent(typeof(GameState))]
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int _currentLevel;
        [SerializeField] private WorldData[] _levels;

        private WorldBuilder _worldBuilder;

        private void Awake()
        {
            _worldBuilder = GetComponent<WorldBuilder>();
        }

        private void Start()
        {
            _currentLevel = -1;
            _levels = GetComponent<LevelContainer>().GetLevels();
        }

        public void OpenNextLevel()
        {
            _currentLevel = ((_currentLevel + 1) % _levels.Length);
            GameState.Current.Unbind();
            _worldBuilder.ReloadLevel(_levels[_currentLevel]);
            GameState.Current.Bind();
        }
    }
}