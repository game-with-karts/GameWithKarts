using UnityEngine;
using UnityEngine.Assertions;
using TMPro;
using System;
using System.Linq;
using System.Collections.Generic;
using static System.Linq.Enumerable;
using GWK.UI;
using GWK.Kart;
using GWK.Util;
using UnityEngine.InputSystem;

public class PostRaceScreen : MonoBehaviour
{
    int activeScreen = 0;
    [SerializeField] private GameObject parentScreen;
    [Header("Screens")]
    [SerializeField] private GameObject[] screens;
    [Header("Position and Lap Times")]
    [SerializeField] private RectTransform lapTimeParent; 
    [SerializeField] private GameObject lapTimePrefab; 
    [Header("Leaderboard")]
    [SerializeField] private RectTransform leaderboardDisplayParent;
    [SerializeField] private GameObject leaderboardEntryPrefab;
    [Header("Final Position")]
    [SerializeField] private TMP_Text finalPlaceDisplay;
    [SerializeField] private TMP_Text finalTimeDisplay;
    [Header("Buttons")]
    [SerializeField] private Window buttonsWindow;
    [SerializeField] private Button nextRaceBtn;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button menuBtn;

    private List<BaseCar> raceLeaderboard = new();
    private PlayerInputActions inputs;

    void Awake() {
        inputs = new();
    }

    Action<InputAction.CallbackContext> switchScreens;

    void OnEnable() {
        switchScreens = _ => SwitchScreens();
        inputs.UI.Confirm.started += switchScreens;
    }

    void OnDisable() {
        inputs.UI.Confirm.started -= switchScreens;
        inputs.UI.Confirm.Disable();
        inputs.UI.Disable();
    }

    private void SwitchScreens() {
        if (activeScreen + 1 == screens.Length) {
            return;
        }
        activeScreen += 1;
        screens[activeScreen - 1].SetActive(false);
        screens[activeScreen].SetActive(true);
        SoundManager.OnConfirmUI();
    }

    public void Show(BaseCar player, List<BaseCar> allCars) {
        raceLeaderboard = allCars.OrderBy(c => c.Timer.TotalTime).ToList();
        foreach(int i in Range(0, numPlayers)) {
            (leaderboardEntries[i].transform as RectTransform).anchoredPosition = new(0, -60 * i);
            leaderboardEntries[i].Display(raceLeaderboard[i].gameObject.name, i + 1, raceLeaderboard[i].Timer.TotalTime);
        }

        finalPlaceDisplay.text = StringsUtil.FormatPlace(player.Path.currentPlacement);
        finalTimeDisplay.text = StringsUtil.GetFormattedTime(player.Timer.TotalTime);

        if (useAggregates) {
            int best = player.Timer.LapTimes.Min();
            int last = player.Timer.LapTimes.Last();
            int avg  = (int)Math.Round(player.Timer.LapTimes.Average(), MidpointRounding.AwayFromZero);

            timeEntries[0].Display("Best", best);
            timeEntries[1].Display("Last", last);
            timeEntries[2].Display("Avg.", avg);
        }
        else {
            Assert.IsTrue(timeEntries.Count == player.Timer.LapTimes.Count);
            foreach (int i in Range(0, timeEntries.Count)) {
                timeEntries[i].Display($"Lap {i + 1}", player.Timer.LapTimes[i]);
            }
        }

        bool hasNextTrack = GameRulesManager.playlist.Length > 0;
        nextRaceBtn.gameObject.SetActive(hasNextTrack);

        if (hasNextTrack) {
            (nextRaceBtn.transform as RectTransform).anchoredPosition = new(0, 50);
            (restartBtn.transform as RectTransform).anchoredPosition = new(0, 0);
            (menuBtn.transform as RectTransform).anchoredPosition = new(0, -50);

            nextRaceBtn.SetSelectUpAndDown(menuBtn, restartBtn);
            restartBtn.SetSelectUpAndDown(nextRaceBtn, menuBtn);
            menuBtn.SetSelectUpAndDown(restartBtn, nextRaceBtn);

            buttonsWindow.SetFirstFocused(nextRaceBtn);
        }
        else {
            (restartBtn.transform as RectTransform).anchoredPosition = new(0, 25);
            (menuBtn.transform as RectTransform).anchoredPosition = new(0, -25);

            restartBtn.SetSelectUpAndDown(menuBtn, menuBtn);
            menuBtn.SetSelectUpAndDown(restartBtn, restartBtn);

            buttonsWindow.SetFirstFocused(restartBtn);
        }
        SetScreenVisibility(true);
    }

    public void SetScreenVisibility(bool visible) {
        parentScreen.SetActive(visible);
        if (!visible) {
            inputs?.UI.Confirm.Disable();
            foreach (GameObject g in screens) {
                g.SetActive(false);
            }
            return;
        }
        inputs.UI.Confirm.Enable();
        activeScreen = 0;
        screens[0].SetActive(true);
        finalPlaceDisplay.gameObject.SetActive(!GameRulesManager.currentTrack.settings.timeAttackMode);
        finalTimeDisplay.gameObject.SetActive(GameRulesManager.currentTrack.settings.timeAttackMode);
    }

    public void RestartRace() {
        SetScreenVisibility(false);
    }

    private bool useAggregates;
    private List<PostRaceTimeEntry> timeEntries = new();
    private List<PostRaceLeaderboardEntry> leaderboardEntries = new();
    private int numPlayers;

    public void Init(int numLaps, int numPlayers, bool aggregates = false) {
        if (numLaps > 7) {
            Init(3, numPlayers, true);
            return;
        }
        useAggregates = aggregates;
        this.numPlayers = numPlayers;
        lapTimeParent.sizeDelta = new(lapTimeParent.sizeDelta.x, 80 * numLaps);
        GameObject temp;

        if (aggregates) {
            // best time
            temp = Instantiate(lapTimePrefab, lapTimeParent);
            (temp.transform as RectTransform).anchoredPosition = new(0, 0);
            timeEntries.Add(temp.GetComponent<PostRaceTimeEntry>());

            // last time
            temp = Instantiate(lapTimePrefab, lapTimeParent);
            (temp.transform as RectTransform).anchoredPosition = new(0, -80);
            timeEntries.Add(temp.GetComponent<PostRaceTimeEntry>());

            // avg time
            temp = Instantiate(lapTimePrefab, lapTimeParent);
            (temp.transform as RectTransform).anchoredPosition = new(0, -160);
            timeEntries.Add(temp.GetComponent<PostRaceTimeEntry>());
        }
        else {
            foreach (int i in Range(0, numLaps)) {
                temp = Instantiate(lapTimePrefab, lapTimeParent);
                (temp.transform as RectTransform).anchoredPosition = new(0, i * -80);
                timeEntries.Add(temp.GetComponent<PostRaceTimeEntry>());
                timeEntries.Last().Display($"Lap {i + 1}", 0);
            }
        }

        foreach (int i in Range(1, numPlayers)) {
            temp = Instantiate(leaderboardEntryPrefab, leaderboardDisplayParent);
            leaderboardEntries.Add(temp.GetComponent<PostRaceLeaderboardEntry>());
        }
    }
}