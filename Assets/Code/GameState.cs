using System;
using TopDownShooter.Player;
using UnityEngine;

public class GameState : MonoBehaviour
{
    [SerializeReference] private PlayerCharacterController _playerCharacterController;

    public static GameState Instance { get; private set; }

    public PlayerCharacterController PlayerCharacterController => _playerCharacterController;

    private void Start()
    {
        if (Instance != null)
            throw new InvalidOperationException();

        Instance = this;
    }
}