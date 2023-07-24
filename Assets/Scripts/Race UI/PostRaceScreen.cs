using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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
    [Header("Buttons")]
    [SerializeField] private Button nextRaceBtn;
    [SerializeField] private Button BackToMenuBtn;

    private List<BaseCar> raceLeaderboard = new();

    void Awake() {
        lastDisplayedIndex = 0;
    }

    public void SetScreenVisibility(bool visible) {
        screen.SetActive(visible);
    }

    public void RaceEnded(BaseCar car) {
        raceLeaderboard.Add(car);
        Display();
        car.Path.OnRaceEnd -= RaceEnded;
        if (!car.playerControlled) return;
        SetScreenVisibility(true);
        nextRaceBtn.gameObject.SetActive(!GameRulesManager.isPlaylistEmpty);
    }

    public void SetFinalPlace(int place) {
        finalPlaceDisplay.text = FormatPlace(place);
    }

    public void RestartRace() {
        SetScreenVisibility(false);
        lastDisplayedIndex = 0;
    }

    private void Display() {
        BaseCar car = raceLeaderboard[lastDisplayedIndex];
        GameObject entry = Instantiate(leaderboardEntryPrefab, leaderboardDisplayParent);
        ((RectTransform)entry.transform).anchoredPosition = new(0, leaderboardEntryHeight * -lastDisplayedIndex);
        entry.GetComponent<PostRaceLeaderboardEntry>().Display(car.gameObject.name, lastDisplayedIndex + 1);
        lastDisplayedIndex++;
    }

    private string FormatPlace(int place) {
        string suffix;
        if ((place / 10) % 10 != 1) {
            suffix = (place % 10) switch {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th"
            };
        }
        else suffix = "th";
        return $"{place}{suffix}";
    }
}