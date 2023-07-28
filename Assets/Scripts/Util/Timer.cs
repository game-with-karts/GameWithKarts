public class Timer
{
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
        if (running) time += delta;
    }

    public void Stop() {
        running = false;
    }

    public void Reset() {
        time = 0;
    }

}