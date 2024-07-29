using System;

public class TimedEvent {
    public readonly Action @event;
    public readonly float timestamp;

    public TimedEvent(Action action, float timestamp) {
        @event = action;
        this.timestamp = timestamp;
    }

    public TimedEvent Invoke() {
        @event?.Invoke();
        return this;
    }
}