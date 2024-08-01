using UnityEngine;

namespace GWK.Kart {

    public class CarAudio : CarComponent {
        [Header("Engine Audio")]
        [SerializeField] private AudioSource engineSource;
        [Min(.001f)]
        [SerializeField] private float pitchResolution;
        [Header("Kart Misc")]
        [SerializeField] private AudioSource boostSource;
        [SerializeField] private AudioSource itemRollingSource;
        [SerializeField] private AudioSource tyreScreechSource;
        public AudioSource ItemRollingSource => itemRollingSource;
        public override void Init(bool _) {
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

        public void PlayOneShot(AudioSource source) {
            source.PlayOneShot(source.clip);
        }

        public void Update() {
            engineSource.pitch = car.RB.linearVelocity.magnitude / pitchResolution + 1;
        }
    }
}