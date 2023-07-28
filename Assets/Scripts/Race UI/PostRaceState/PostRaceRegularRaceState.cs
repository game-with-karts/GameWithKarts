using UnityEngine;
using System.Collections.Generic;

public class PostRaceRegularRaceState : PostRaceState
{
    public PostRaceRegularRaceState() : base() {
    }

    public override void NextLap(BaseCar car) {
        base.NextLap(car);
    }

    public override void RaceEnded(BaseCar car) {
        leaderboard.Add(car);
        Display();
        base.RaceEnded(car);
    }

    
}