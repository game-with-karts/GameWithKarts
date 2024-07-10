using UnityEngine;

public class CarAudio : CarComponent
{
    [Header("Engine Audio")]
    [SerializeField] private AudioSource engineSource;
    [Min(.001f)]
    [SerializeField] private float pitchResolution;
    [Header("Kart Misc")]
    [SerializeField] private AudioSource boostSource;
    [SerializeField] private AudioSource tyreScreechSource;
    public override void Init() {
        Play(false);
    }

    public void Play(bool isPaused) {
        if (!isPaused) {
            engineSource.Play();
        }
        else {
            engineSource.Pause();
        }
    }

    public void Update() {
        engineSource.pitch = car.RB.velocity.magnitude / pitchResolution + 1;
    }
}