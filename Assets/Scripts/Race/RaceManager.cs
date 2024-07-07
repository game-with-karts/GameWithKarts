using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;
using System;
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
    [SerializeField] private SettingsMenu settingsMenu;
    [Header("Per-Track settings")]
    [SerializeField] private Transform track;
    [SerializeField] private MinimapTransform minimapTransform;
    [SerializeField] private Sprite minimapImage;
    [SerializeField] private PreRaceSequence sequence;
    [SerializeField] private Volume globalVolume;
    [SerializeField] private AudioClip music;
    [SerializeField] private bool startOnAntigrav = false;
    

    private void Awake() {
        settingsMenu.inputs = new();
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
            pauseMenu.OnPause += car.Audio.Play;
            car.Path.OnRaceEnd += postRaceScreen.RaceEnded;
            car.Path.OnNextLap += postRaceScreen.NextLap;
            settingsMenu.inputs.Add(car.GetComponent<PlayerInput>());
            if (car.IsBot) {
                continue;
            }
            car.Path.OnRaceEnd += pauseMenu.RaceEnd;
            sequence.OnSequenceEnd += car.Camera.ActivateCamera;
            sequence.OnSequenceEnd += car.UI.ActivateCanvas;
            sequence.OnSequenceEnd += () => SoundManager.SetMusic(music);
            if (GameRulesManager.currentTrack.settings.timeAttackMode)
                pauseMenu.OnPause += car.Timer.ToggleTimer;
            car.UI.Minimap.SetMinimapImage(minimapImage);
            car.UI.Minimap.SetMinimapTransform(minimapTransform);
            car.UI.Minimap.AddCars(cars);

                
        }
        carPlacement.Init(cars);
        postRaceScreen.SetScreenVisibility(false);
        if (GameRulesManager.currentTrack.settings.timeAttackMode) {
            postRaceScreen.SetState<PostRaceTimeTrialState>();
        }
        else {
            postRaceScreen.SetState<PostRaceRegularRaceState>();
            carPlacement.OnFinalPlacement += postRaceScreen.SetFinalPlace;
        }
        countdownScreen.OnCountdownOver += StartRace;
        sequence.OnSequenceEnd += () => {
            pauseMenu.gameObject.SetActive(true);
            SoundManager.SetMusicLooping(true);
            countdownScreen.StartCountdown();
        };
    }

    private void Start() {
        pauseMenu.gameObject.SetActive(false);
        sequence.StartSequence();
    }

    public void ResetRace() {
        SoundManager.StopMusic();
        countdownScreen.ResetCountdown();
        pauseMenu.ResetRace();
        OnRaceReset?.Invoke();
        postRaceScreen.RestartRace();
        countdownScreen.StartCountdown();
    }

    private void StartRace() {
        OnRaceStart?.Invoke();
        SoundManager.PlayMusic();
    }

    private void OnDestroy() {
        OnRaceReset = null;
        OnRaceStart = null;
    }
}