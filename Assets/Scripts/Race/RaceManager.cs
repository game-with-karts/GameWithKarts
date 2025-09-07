using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Linq;
using System.Collections.Generic;
using GWK.Kart;

public class RaceManager : MonoBehaviour
{
    private Action OnRaceReset;
    private Action OnRaceStart;
    private BaseCar[] cars;
    [SerializeField] private bool testMode;
    [Header("Initialisation")]
    [Tooltip("1st place is at index 0, 2nd place at index 1, etc.")]
    [SerializeField] private StartFinish startFinish;
    [SerializeField] private CarSpawner carSpawner;
    [SerializeField] private CarPlacement carPlacement;
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private PostRaceScreen postRaceScreen;
    [SerializeField] private CountdownScreen countdownScreen;
    [SerializeField] private SettingsMenu settingsMenu;
    [SerializeField] private GameObject itemBoxParent;
    [Header("Per-Track settings")]
    [SerializeField] private Transform track;
    [SerializeField] private MinimapTransform minimapTransform;
    [SerializeField] private Sprite minimapImage;
    [SerializeField] private PreRaceSequence sequence;
    [SerializeField] private Volume globalVolume;
    [SerializeField] private AudioClip music;
    [SerializeField] private bool startOnAntigrav = false;
    private int numPlayers;

    public static RaceManager instance { get; private set; }
    public static List<ISelfDestructable> allItems = new();
    public static bool RaceStarted {get; private set;}

    public IEnumerable<BaseCar> CarsInPlacementOrder => carPlacement.CarsInOrder;

    private void Awake() {
        RaceStarted = false;
        if (instance is not null) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        if (testMode) {
            cars = new BaseCar[0];
            return;
        }

        RaceSettings settings = GameRulesManager.instance.currentTrack.settings;

        track.localScale = settings.mirrorMode ? new Vector3(-1, 1, 1) : Vector3.one;
        globalVolume.enabled = PlayerPrefs.GetInt(SettingsMenu.EnablePostProcessingKey) == 1;

        cars = carSpawner.SpawnRandom(startFinish.StartPositions, 
                                      settings, 
                                      GameRulesManager.instance.players, 
                                      startOnAntigrav);

        numPlayers = cars.Count(c => c.playerControlled);

        postRaceScreen.Init(settings.numberOfLaps, cars.Length);

        bool useItems = settings.useItems && settings.itemsEnabled.Any(ie => ie.Value);
        itemBoxParent.SetActive(useItems);

        foreach (var car in cars) {
            OnRaceReset += () => car.ResetCar(false);
            OnRaceReset += () => car.Path.SetPath(startFinish.GetPathAtLap(1));
            OnRaceStart += car.StartRace;
            pauseMenu.OnPause += car.Audio.Play;
            car.Path.OnRaceEnd += OnCarFinished;

            if (car.IsBot) {
                continue;
            }

            car.Path.OnRaceEnd += pauseMenu.RaceEnd;

            sequence.OnSequenceEnd += car.Camera.ActivateCamera;
            sequence.OnSequenceEnd += car.UI.ActivateCanvas;
            sequence.OnSequenceEnd += () => SoundManager.SetMusic(music);

            pauseMenu.OnPause += car.Timer.ToggleTimer;

            car.UI.Minimap.SetMinimapImage(minimapImage);
            car.UI.Minimap.SetMinimapTransform(minimapTransform);
            car.UI.Minimap.AddCars(cars);
        }
        carPlacement.Init(cars);

        postRaceScreen.SetScreenVisibility(false);
        countdownScreen.OnCountdownOver += StartRace;

        sequence.OnSequenceEnd += () => {
            pauseMenu.gameObject.SetActive(true);
            SoundManager.SetMusicLooping(true);
            countdownScreen.StartCountdown();
        };
    }

    public IEnumerable<BaseCar> GetTargetables() {
        return cars;
    }

    /// <summary>
    /// ONLY FOR TESTING - DO NOT USE OUTSIDE OF TESTING!!!!!
    /// </summary>
    /// <param name="car"></param>
    public void AddCarManually(BaseCar car) {
        cars = cars.Append(car).ToArray();
    }

    private void OnCarFinished(BaseCar car) {
        car.Path.OnRaceEnd -= OnCarFinished;
        if (!car.playerControlled) {
            return;
        }
        numPlayers -= 1;
        if (numPlayers == 0) {
            foreach (BaseCar c in cars.Where(c => !c.Finished)) {
                c.EndRace();
            }
            postRaceScreen.Show(car, cars.ToList());
        }
    }

    private void Start() {
        if (testMode) {
            return;
        }
        pauseMenu.gameObject.SetActive(false);
        sequence.StartSequence();
    }

    public void ResetRace() {
        RaceStarted = false;
        SoundManager.StopMusic();
        countdownScreen.ResetCountdown();
        pauseMenu.ResetRace();

        if (allItems.Count > 0) {
            allItems.ForEach(i => i.SelfDestruct());
            allItems = new();
        }

        foreach (var c in cars.Where(c => c.Finished)) {
            c.Path.OnRaceEnd += OnCarFinished;
        }
        OnRaceReset?.Invoke();
        postRaceScreen.RestartRace();
        numPlayers = cars.Count(c => c.playerControlled);
        countdownScreen.StartCountdown();
    }

    private void StartRace() {
        OnRaceStart?.Invoke();
        SoundManager.PlayMusic();
        RaceStarted = true;
    }

    private void OnDestroy() {
        OnRaceReset = null;
        OnRaceStart = null;
        instance = null;
    }
}
