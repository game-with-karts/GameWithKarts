using UnityEngine;

public class RaceManager : MonoBehaviour
{
    [Header("Initialisation")]
    [Tooltip("1st place is at index 0, 2nd place at index 1, etc.")]
    [SerializeField] private Transform[] startingPositions;
    [SerializeField] private CarSpawner carSpawner;
    [SerializeField] private CarPlacement carPlacement;
    [Header("Per-Track settings")]
    [SerializeField] private bool startOnAntigrav = false;
    

    private void Awake() {
        BaseCar[] cars = carSpawner.SpawnRandom(startingPositions, GameRulesManager.instance.settings, 1, startOnAntigrav);
        carPlacement.Init(cars);
    }
}