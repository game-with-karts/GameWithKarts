using UnityEngine;

public class RaceManager : MonoBehaviour
{
    [Header("Initialisation")]
    [Tooltip("1st place is at index 0, 2nd place at index 1, etc.")]
    [SerializeField] private Transform[] startingPositions;
    [SerializeField] private CarSpawner carSpawner;
    [Header("Per-Track settings")]
    [SerializeField] private bool startOnAntigrav = false;
    

    // --- SPAWNING SETTINGS ---
    private PlayerSpawning playerSpawning;

    private void Awake() {
        playerSpawning = GameRulesManager.instance.settings.playerSpawning;
        carSpawner.SpawnRandom(startingPositions, playerSpawning, 1, startOnAntigrav);
    }
}