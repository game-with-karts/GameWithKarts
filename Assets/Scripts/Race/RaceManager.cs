using UnityEngine;
using System;
public class RaceManager : MonoBehaviour
{
    private Action OnRaceReset;
    private BaseCar[] cars;
    [Header("Initialisation")]
    [Tooltip("1st place is at index 0, 2nd place at index 1, etc.")]
    [SerializeField] private StartFinish startFinish;
    [SerializeField] private CarSpawner carSpawner;
    [SerializeField] private CarPlacement carPlacement;
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private PostRaceScreen postRaceScreen;
    [Header("Per-Track settings")]
    [SerializeField] private bool startOnAntigrav = false;
    

    private void Awake() {
        cars = carSpawner.SpawnRandom(startFinish.StartPositions, 
                                      GameRulesManager.instance.currentTrack.settings, 
                                      1, startOnAntigrav);
        foreach (var car in cars) {
            OnRaceReset += car.ResetCar;
            if (!car.IsBot)
                car.Path.OnRaceEnd += pauseMenu.RaceEnd;
        }
        carPlacement.Init(cars);
        postRaceScreen.SetScreenVisibility(false);
        carPlacement.OnFinalPlacement += postRaceScreen.RaceEnded;
    }

    public void ResetRace() {
        pauseMenu.ResetRace();
        OnRaceReset?.Invoke();
        postRaceScreen.RestartRace();
    }
}