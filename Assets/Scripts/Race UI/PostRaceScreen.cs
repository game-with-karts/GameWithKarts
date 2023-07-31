using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class PostRaceScreen : MonoBehaviour
{
    static int lastDisplayedIndex;
    [SerializeField] private GameObject screen;
    [Header("Leaderboard")]
    [SerializeField] private RectTransform leaderboardDisplayParent;
    [SerializeField] private GameObject leaderboardEntryPrefab;
    [SerializeField] private float leaderboardEntryHeight;
    [Header("Final Position")]
    [SerializeField] private TMP_Text finalPlaceDisplay;
    [SerializeField] private TMP_Text finalTimeDisplay;
    [Header("Buttons")]
    [SerializeField] private Button nextRaceBtn;
    [SerializeField] private Button BackToMenuBtn;

    private List<BaseCar> raceLeaderboard = new();

    private PostRaceState state;

    void Awake() {
        lastDisplayedIndex = 0;
    }

    public void SetScreenVisibility(bool visible) {
        screen.SetActive(visible);
        finalPlaceDisplay.gameObject.SetActive(!GameRulesManager.currentTrack.settings.timeAttackMode);
        finalTimeDisplay.gameObject.SetActive(GameRulesManager.currentTrack.settings.timeAttackMode);
    }

    public void SetState<T>() where T : PostRaceState, new() {
        state = new T();
        state.SetLeaderboard(this.raceLeaderboard);
        state.SetUIElements(leaderboardDisplayParent, leaderboardEntryPrefab, leaderboardEntryHeight);
    }

    public void RaceEnded(BaseCar car) {
        car.Path.OnRaceEnd -= RaceEnded;
        car.Path.OnNextLap -= NextLap;
        state.RaceEnded(car);
        if (!car.playerControlled) return;
        finalTimeDisplay.text = CarLapTimer.GetFormattedTime(car.Timer.TotalTime);
        SetScreenVisibility(true);
        nextRaceBtn.gameObject.SetActive(!GameRulesManager.isPlaylistEmpty);
    }

    public void NextLap(BaseCar car) {
        state.NextLap(car);
    }

    public void SetFinalPlace(int place) {
        finalPlaceDisplay.text = CarUI.FormatPlace(place);
    }

    public void RestartRace() {
        SetScreenVisibility(false);
        lastDisplayedIndex = 0;
        state.RestartRace();
    }

}