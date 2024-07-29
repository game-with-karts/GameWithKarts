using System;
using System.Linq;
using System.Collections.Generic;

public class Timer {
    
    List<TimedEvent> timedEvents = new();

    float time = 0;
    public float Time => time;
    public bool running { 
        get; 
        private set; 
    }

    public Timer() {
        running = false;
    }

    public void Start() {
        running = true;
    }

    public void Tick(float delta) {
        if (!running) {
            return;
        }
        float newTime = time + delta;
        timedEvents.Where(e => IsBetween(e.timestamp, time, newTime)).Select(e => e.Invoke());
        time = newTime;
    }

    public void Stop() {
        running = false;
    }

    public void Reset() {
        time = 0;
    }

    public void ClearEvents() {
        timedEvents = new();
    }

    public TimedEvent AddEvent(float timestamp, Action action) {
        TimedEvent timedEvent = new TimedEvent(action, timestamp);
        timedEvents.Add(timedEvent);
        return timedEvent;
    }

    public void RemoveEvent(TimedEvent timedEvent) {
        timedEvents.Remove(timedEvent);
    }

    public void RemoveEventsAt(float timestamp) {
        timedEvents.RemoveAll(e => e.timestamp == timestamp);
    }

    private bool IsBetween(float val, float min, float max) {
        if (max < min) {
            (min, max) = (max, min);
        }
        return val >= min && val <= max;
    }

}