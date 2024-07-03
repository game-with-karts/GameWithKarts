using UnityEngine;

public class PostRaceTimeTrialState : PostRaceState
{
    public PostRaceTimeTrialState() : base() {
    }

    public override void NextLap(BaseCar car) {
        leaderboard.Add(car);
        Display();
        base.NextLap(car);
    }

    public override void RaceEnded(BaseCar car) {
        base.RaceEnded(car);
    }

    protected override void Display() {
        BaseCar car = leaderboard[lastDisplayedIndex];
        GameObject entry = GameObject.Instantiate(leaderboardEntryPrefab, leaderboardDisplayParent);
        ((RectTransform)entry.transform).anchoredPosition = new(0, leaderboardEntryHeight * -lastDisplayedIndex);
        entry.GetComponent<PostRaceLeaderboardEntry>().Display(CarLapTimer.GetFormattedTime(car.Timer.LapTimes[lastDisplayedIndex], true), lastDisplayedIndex + 1);
        lastDisplayedIndex++;
    }

    public override void RestartRace()
    {
        base.RestartRace();
        int n = leaderboard.Count;
        leaderboard = new();
        for (int i = 0; i < n; i++) {
            GameObject.Destroy(leaderboardDisplayParent.GetChild(0).gameObject);
        }
    }
}