using UnityEngine;
using System;
using UnityEngine.Rendering;
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
    [SerializeField] private Transform track;
    [SerializeField] private PreRaceSequence sequence;
    [SerializeField] private Volume globalVolume;
    [SerializeField] private bool startOnAntigrav = false;
    

    private void Awake() {
        track.localScale = GameRulesManager.currentTrack.settings.mirrorMode ? new Vector3(-1, 1, 1) : Vector3.one;
        globalVolume.enabled = PlayerPrefs.GetInt(SettingsMenu.EnablePostProcessingKey) == 1;
        cars = carSpawner.SpawnRandom(startFinish.StartPositions, 
                                      GameRulesManager.currentTrack.settings, 
                                      GameRulesManager.players, 
                                      startOnAntigrav);
        foreach (var car in cars) {
            OnRaceReset += () => car.ResetCar(false);
            OnRaceReset += () => car.Path.SetPath(startFinish.GetPathAtLap(1));
            OnRaceStart += car.StartRace;
            car.Path.OnRaceEnd += postRaceScreen.RaceEnded;
            if (!car.IsBot) {
                car.Path.OnRaceEnd += pauseMenu.RaceEnd;
                sequence.OnSequenceEnd += car.Camera.ActivateCamera;
                sequence.OnSequenceEnd += car.UI.ActivateCanvas;
            }
                
        }
        carPlacement.Init(cars);
        postRaceScreen.SetScreenVisibility(false);
        carPlacement.OnFinalPlacement += postRaceScreen.SetFinalPlace;
        countdownScreen.OnCountdownOver += StartRace;
        sequence.OnSequenceEnd += countdownScreen.StartCountdown;
        sequence.OnSequenceEnd += () => pauseMenu.gameObject.SetActive(true);
    }

    private void Start() {
        pauseMenu.gameObject.SetActive(false);
        sequence.StartSequence();
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