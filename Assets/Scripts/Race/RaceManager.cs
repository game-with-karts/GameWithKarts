using UnityEngine;
using System;
using System.Linq;
public class RaceManager : MonoBehaviour
{
    private Action OnRaceReset;
    private Action OnRaceStart;
    private BaseCar[] cars;
    [Header("Initialisation")]
    [Tooltip("1st place is at index 0, 2nd place at index 1, etc.")]
    [SerializeField] private StartFinish startFinish;
    [SerializeField] private CarSpawner carSpawner;
    [SerializeField] private CarPlacement carPlacement;
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private PostRaceScreen postRaceScreen;
    [SerializeField] private CountdownScreen countdownScreen;
    [Header("Per-Track settings")]
    [SerializeField] private bool startOnAntigrav = false;
    

    private void Awake() {
        cars = carSpawner.SpawnRandom(startFinish.StartPositions, 
                                      GameRulesManager.instance.currentTrack.settings, 
                                      GameRulesManager.instance.players, 
                                      startOnAntigrav);
        foreach (var car in cars) {
            OnRaceReset += car.ResetCar;
            OnRaceReset += () => { car.Path.SetPath(startFinish.GetPathAtLap(1)); };
            OnRaceStart += car.StartRace;
            car.Path.OnRaceEnd += postRaceScreen.RaceEnded;
            if (!car.IsBot) {
                car.Path.OnRaceEnd += pauseMenu.RaceEnd;
            }
                
        }
        carPlacement.Init(cars);
        postRaceScreen.SetScreenVisibility(false);
        carPlacement.OnFinalPlacement += postRaceScreen.SetFinalPlace;
        countdownScreen.OnCountdownOver += StartRace;
        countdownScreen.StartCountdown();
    }

    public void ResetRace() {
        countdownScreen.ResetCountdown();
        pauseMenu.ResetRace();
        OnRaceReset?.Invoke();
        postRaceScreen.RestartRace();
        countdownScreen.StartCountdown();
    }

    private void StartRace() => OnRaceStart?.Invoke();
}