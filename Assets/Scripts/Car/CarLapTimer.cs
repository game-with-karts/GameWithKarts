using UnityEngine;
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
namespace GWK.Kart {
    public class CarLapTimer : CarComponent {
        private readonly Stopwatch timer = new();
        private List<int> lapTimes;
        private int fastestTime;
        private bool eventsSubscribed = false;
        public List<int> LapTimes => lapTimes;
        public int TotalTime => lapTimes.Sum();
        public int ElapsedTime => (int)Math.Round(timer.Elapsed.TotalMilliseconds, MidpointRounding.AwayFromZero);
        public event Action<int> OnLapSaved;
        public override void Init(bool restarting) {
            timer.Stop();
            timer.Reset();
            fastestTime = int.MaxValue;
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
            int time = ElapsedTime - TotalTime;
            lapTimes.Add(time);
            if (time < fastestTime)
                fastestTime = time;
            timer.Start();
            OnLapSaved?.Invoke(time);
        }

        private void StopTimer(BaseCar _) {
            car.Path.OnNextLap -= SaveLap;
            car.Path.OnRaceEnd -= StopTimer;
            eventsSubscribed = false;
            timer.Stop();
            if (!car.playerControlled) {
                Extrapolate();
            }
        }

        public override void StartRace() {
            timer.Start();
        }

        private void Extrapolate() {
            if (lapTimes.Count == car.Path.numLaps) {
                return;
            }
            if (lapTimes.Count == 0) {
                int elapsed = ElapsedTime;
                float reciprocal = 1 / car.Path.CurrentPathTime;
                for (int i = 0; i < car.Path.numLaps; i++) {
                    lapTimes.Add(Mathf.RoundToInt(Mathf.LerpUnclamped(0, elapsed, reciprocal)));
                }
                return;
            }
            int lapsRemaining = car.Path.numLaps - car.Path.CurrentLap;
            int averageLapTime = (int)Math.Round(lapTimes.Average(), MidpointRounding.AwayFromZero);
            if (car.Path.CurrentPathTime == 0) {
                lapTimes.Add(averageLapTime);
            }
            else {
                lapTimes.Add(Mathf.RoundToInt(Mathf.LerpUnclamped(0, ElapsedTime - TotalTime, .95f / car.Path.CurrentPathTime)));
            }
            for (int i = 0; i < lapsRemaining; i++) {
                lapTimes.Add(averageLapTime);
            }
        }

        void OnDestroy() {
            OnLapSaved = null;
        }
    }
}