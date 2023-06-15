using TopDownShooter.Interactive.Player;
using UnityEngine;
using UnityEngine.UI;

namespace TopDownShooter
{
    public class UIController : MonoBehaviour
    {
        [SerializeReference] private GameManager _gameManager;

        [SerializeReference] private GameObject _mainMenuContainer;
        [SerializeReference] private GameObject _playerStatsContainer;
        [SerializeReference] private GameObject _miniMenuContainer;
        [SerializeReference] private Slider _playerHealthSlider;
        [SerializeReference] private Slider _playerStaminaSlider;

        private void Start()
        {
            SetMainMenuActive(true);
            SetPlayerStatsActive(false);
            SetMiniMenuActive(false);
        }

        public void OnPlayButton()
        {
            _gameManager.OpenNextLevel();
            SetMainMenuActive(false);
            SetPlayerStatsActive(true);
            SetMiniMenuActive(false);
        }

        public void OnQuitButton()
        {
            Application.Quit();
        }

        public void OnPlayerDeath()
        {
            SetMainMenuActive(false);
            SetPlayerStatsActive(false);
            SetMiniMenuActive(true);
        }

        public void SetMainMenuActive(bool active)
        {
            _mainMenuContainer.SetActive(active);
        }

        public void SetPlayerStatsActive(bool active)
        {
            _playerStatsContainer.SetActive(active);
        }

        public void SetMiniMenuActive(bool active)
        {
            _miniMenuContainer.SetActive(active);
        }

        private void Update()
        {
            var playerController = GameState.Current.PlayerCharacterController;
            if (playerController == null)
                return;

            var stats = playerController.GetComponent<PlayerLivingEntity>();
            _playerHealthSlider.value = stats.Health / stats.MaxHealth;
            _playerStaminaSlider.value = stats.Stamina / stats.MaxStamina;
        }
    }
}