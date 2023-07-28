using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
public abstract class PostRaceState
{
    protected List<BaseCar> leaderboard;
    public event Action OnRaceEnded;
    public event Action OnNextLap;

    protected int lastDisplayedIndex;
    protected RectTransform leaderboardDisplayParent;
    protected GameObject leaderboardEntryPrefab;
    protected float leaderboardEntryHeight;

    public PostRaceState(){
    }

    public void SetLeaderboard(List<BaseCar> leaderboard) => this.leaderboard = leaderboard;
    public void SetUIElements(RectTransform parent, GameObject prefab, float height) {
        leaderboardDisplayParent = parent;
        leaderboardEntryPrefab = prefab;
        leaderboardEntryHeight = height;
    }

    public virtual void RaceEnded(BaseCar car) {
        OnRaceEnded?.Invoke();
    }
    public virtual void NextLap(BaseCar car) {
        OnNextLap?.Invoke();
    }

    public virtual void RestartRace() {
        lastDisplayedIndex = 0;
    }

    protected virtual void Display() {
        BaseCar car = leaderboard[lastDisplayedIndex];
        GameObject entry = GameObject.Instantiate(leaderboardEntryPrefab, leaderboardDisplayParent);
        ((RectTransform)entry.transform).anchoredPosition = new(0, leaderboardEntryHeight * -lastDisplayedIndex);
        entry.GetComponent<PostRaceLeaderboardEntry>().Display(car.gameObject.name, lastDisplayedIndex + 1);
        lastDisplayedIndex++;
    }
}