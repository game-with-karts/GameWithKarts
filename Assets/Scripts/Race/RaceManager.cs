using UnityEngine;
using System;
public class RaceManager : MonoBehaviour
{
    private Action OnRaceReset;
    [Header("Initialisation")]
    [Tooltip("1st place is at index 0, 2nd place at index 1, etc.")]
    [SerializeField] private Transform[] startingPositions;
    [SerializeField] private CarSpawner carSpawner;
    [SerializeField] private CarPlacement carPlacement;
    [SerializeField] private PauseMenu pauseMenu;
    [Header("Per-Track settings")]
    [SerializeField] private bool startOnAntigrav = false;
    

    private void Awake() {
        BaseCar[] cars = carSpawner.SpawnRandom(startingPositions, GameRulesManager.instance.settings, 1, startOnAntigrav);
        foreach (var car in cars) {
            OnRaceReset += car.ResetCar;
        }
        carPlacement.Init(cars);
    }

    public void ResetRace() {
        pauseMenu.ResetRace();
        OnRaceReset?.Invoke();
    }
}