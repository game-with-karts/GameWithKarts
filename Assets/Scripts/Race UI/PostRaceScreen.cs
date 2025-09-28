using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using GWK.Kart;
using GWK.Util;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class PostRaceScreen : MonoBehaviour {
    private List<BaseCar> raceLeaderboard = new();
    private PlayerInputActions inputs;

    private RaceSettings raceSettings => GameRulesManager.instance.currentTrack.settings;
    private PlaylistSettings playlistSettings => GameRulesManager.instance.playlist.settings;

    [SerializeField] private UIDocument document;
    [SerializeField] private GameObject hintsCanvas;
    private VisualElement root => document.rootVisualElement;
    [Space]
    [SerializeField] private VisualTreeAsset finishScreen;
    [Space]
    [SerializeField] private VisualTreeAsset summaryScreen;
    [SerializeField] private VisualTreeAsset timeSummaryElement;
    [Space]
    [SerializeField] private VisualTreeAsset placementScreen;
    [SerializeField] private VisualTreeAsset placementElement;
    [SerializeField] private Color colorFirst;
    [SerializeField] private Color colorSecond;
    [SerializeField] private Color colorThird;
    [SerializeField] private Color colorDefault;
    [Space]
    [SerializeField] private GameObject buttonsNormal;
    [SerializeField] private GameObject buttonsPlaylist;
    [SerializeField] private GameObject buttonsCup;
    [SerializeField] private UnityEvent OnRestart;
    [SerializeField] private UnityEvent OnBackToMenu;
    [SerializeField] private UnityEvent OnNext;

    public BaseCar Player {get; set;}

    private Action nextScreen;
    private IEnumerator finishScreenCoroutine;

    void Awake() {
        inputs = new();
        inputs.UI.Confirm.Enable();
        inputs.UI.Confirm.started += SwitchScreens;
    }

    void OnDisable() {
        inputs.UI.Confirm.started -= SwitchScreens;
        inputs.UI.Confirm.Disable();
    }

    private void SwitchScreens(InputAction.CallbackContext _) {
        if (nextScreen is null) {
            return;
        }
        SoundManager.OnConfirmUI();
        nextScreen.Invoke();
    }

    public void Hide() {
        hintsCanvas.SetActive(false);
        if (finishScreenCoroutine != null) {
            StopCoroutine(finishScreenCoroutine);
        }
        finishScreenCoroutine = null;
        nextScreen = null;
        root.Clear();
        buttonsNormal.SetActive(false);
        buttonsPlaylist.SetActive(false);
        buttonsCup.SetActive(false);
    }

    public void Show() {
        hintsCanvas.SetActive(true);
        if (finishScreenCoroutine != null) {
            StopCoroutine(finishScreenCoroutine);
        }
        /*
        finishScreenCoroutine = FinishScreen();
        StartCoroutine(finishScreenCoroutine);
        */
        SummaryScreen();
    }

    private void InitialiseRoot(VisualTreeAsset tree) {
        root.Clear();
        tree.CloneTree(root);
    }

    private IEnumerator FinishScreen() {
        InitialiseRoot(finishScreen);
        yield return new WaitForSeconds(2f);
        finishScreenCoroutine = null;
        SummaryScreen();
    }

    private IEnumerator AddClass(VisualElement element, string className) {
        yield return new WaitForEndOfFrame();
        element.AddToClassList(className);
    }

    private void SummaryScreen() {
        InitialiseRoot(summaryScreen);
        Label l = root.Q<Label>("score");
        if (raceSettings.timeAttackMode) {
            l.text = StringsUtil.GetFormattedTime(Player.Timer.TotalTime);
        }
        else {
            l.text = StringsUtil.FormatPlace(Player.Path.currentPlacement);
        }

        VisualElement times = root.Q<VisualElement>("time-table");
        if (raceSettings.numberOfLaps > 7) {
            int fastest = Player.Timer.LapTimes.OrderBy(x => x).First();
            int average = Player.Timer.TotalTime / Player.Timer.LapTimes.Count;
            int last = Player.Timer.LapTimes.Last();

            VisualElement fastestElem = timeSummaryElement.CloneTree();
            fastestElem.Q<Label>("title").text = "Best";
            fastestElem.Q<Label>("value").text = StringsUtil.GetFormattedTime(fastest);
            times.Add(fastestElem);

            VisualElement avgElem = timeSummaryElement.CloneTree();
            avgElem.Q<Label>("title").text = "Avg";
            avgElem.Q<Label>("value").text = StringsUtil.GetFormattedTime(average);
            times.Add(avgElem);

            VisualElement lastElem = timeSummaryElement.CloneTree();
            lastElem.Q<Label>("title").text = "Last";
            lastElem.Q<Label>("value").text = StringsUtil.GetFormattedTime(last);
            times.Add(lastElem);
        }
        else {
            for (int i = 0; i < raceSettings.numberOfLaps; i++) {
                VisualElement timeElem = timeSummaryElement.CloneTree();
                timeElem.Q<Label>("title").text = $"Lap {i + 1}";
                timeElem.Q<Label>("value").text = StringsUtil.GetFormattedTime(Player.Timer.LapTimes[i]);
                times.Add(timeElem);
            }
        }
        StartCoroutine(AddClass(times, "main-container-after"));

        if (raceSettings.timeAttackMode) {
            nextScreen = ButtonsScreenNormal;
            return;
        }
        nextScreen = PlacementScreen;
    }

    List<VisualElement> placementEntries;
    private void PlacementScreen() {
        InitialiseRoot(placementScreen);

        placementEntries = new();

        VisualElement container = root.Q<VisualElement>("container");
        IEnumerable<BaseCar> cars = RaceManager.instance.CarsInPlacementOrder;
        int i = 1;
        foreach (BaseCar car in cars) {
            VisualElement elem = placementElement.CloneTree();
            Color col = i switch {
                1 => colorFirst,
                2 => colorSecond,
                3 => colorThird,
                _ => colorDefault,
            };
            elem.Q<VisualElement>("base").style.backgroundColor = new StyleColor(col);
            elem.Q<Label>("place").text = StringsUtil.FormatPlace(i);
            elem.Q<Label>("name").text = car.gameObject.name;
            elem.Q<Label>("time").text = StringsUtil.GetFormattedTime(car.Timer.TotalTime);
            container.Add(elem);
            placementEntries.Add(elem);
            i++;
        }
        StartCoroutine(AnimateStandings());
        if (playlistSettings.cupScoring) {
            nextScreen = CupPointsScreen;
            return;
        }
        if (GameRulesManager.instance.playlist.LastTrack) {
            nextScreen = ButtonsScreenNormal;
            return;
        }
        nextScreen = ButtonsScreenPlaylist;
    }

    public void CupPointsScreen() {
        

        if (GameRulesManager.instance.playlist.LastTrack) {
            nextScreen = ButtonsScreenNormal;
            return;
        }
        nextScreen = ButtonsScreenCup;
    }

    private IEnumerator AnimateStandings() {
        Debug.Log(placementEntries.Count);
        foreach (VisualElement ve in placementEntries) {
            yield return new WaitForSeconds(.08f);
            ve.Q<VisualElement>("base").AddToClassList("main-container-after");
        }
    }

    private void ButtonsScreenNormal() {
        root.Clear();
        buttonsNormal.SetActive(true);
        nextScreen = null;
    }

    private void ButtonsScreenPlaylist() {
        root.Clear();
        buttonsPlaylist.SetActive(true);
        nextScreen = null;
    }

    private void ButtonsScreenCup() {
        root.Clear();
        buttonsCup.SetActive(true);
        nextScreen = null;
    }

    public void Restart() => OnRestart.Invoke();
    public void BackToMenu() => OnBackToMenu.Invoke();
    public void NextTrack() => OnNext.Invoke();
}
