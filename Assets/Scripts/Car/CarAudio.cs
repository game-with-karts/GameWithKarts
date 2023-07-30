using UnityEngine;
using UnityEngine.Audio;

public class CarAudio : CarComponent
{
    [Header("Engine Audio")]
    [SerializeField] private AudioSource engineSource;
    [Header("Kart Misc")]
    [SerializeField] private AudioSource boostSource;
    [SerializeField] private AudioSource tyreScreechSource;
    public override void Init() {
        return;
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
        engineSource.pitch = car.RB.velocity.magnitude / 20f + 1;
    }
}