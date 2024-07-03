using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System;
public class CarLapTimer : CarComponent
{
    private readonly Stopwatch timer = new();
    private List<double> lapTimes;
    private double fastestTime;
    private bool eventsSubscribed = false;
    public List<double> LapTimes => lapTimes;
    public double TotalTimeMS => lapTimes.Sum();
    public double TotalTime => TotalTimeMS / 1000;
    public double ElapsedTime => timer.Elapsed.TotalSeconds;
    public double ElapsedTimeMS => timer.Elapsed.TotalMilliseconds;
    public override void Init() {
        timer.Stop();
        timer.Reset();
        fastestTime = double.MaxValue;
        lapTimes = new();
        if (!eventsSubscribed) {
            eventsSubscribed = true;
            car.Path.OnNextLap += SaveLap;
            car.Path.OnRaceEnd += StopTimer;
        }
    }

    public void ToggleTimer(bool paused) {
        if (paused || !car.Movement.IsControlable){
            timer.Stop();
            return;
        }
        timer.Start();
    }

    private void SaveLap(BaseCar _) {
        timer.Stop();
        double time = Math.Floor(ElapsedTimeMS) - TotalTimeMS;
        lapTimes.Add(time);
        if (time < fastestTime)
            fastestTime = time;
        timer.Start();
    }

    private void StopTimer(BaseCar _) {
        car.Path.OnNextLap -= SaveLap;
        car.Path.OnRaceEnd -= StopTimer;
        eventsSubscribed = false;
        timer.Stop();
    }

    public override void StartRace() {
        timer.Start();
    }

    public static string GetFormattedTime(double time, bool asMs = false) {
        if (asMs) {
            time /= 1000;
        }
        if (time == double.MaxValue) return "--:--.---";
        int m = (int)time / 60;
        int s = (int)time % 60;
        int ms = (int)((time - s - m * 60) * 1000);
        string m_str = string.Format("{0:00}", m);
        string s_str = string.Format("{0:00}", s);
        string ms_str = string.Format("{0:000}", ms);
        return $"{m_str}:{s_str}.{ms_str}";
    }
}