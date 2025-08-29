using UnityEngine;

namespace GWK.Kart {

    public class CarAudio : CarComponent {
        [Header("Engine Audio")]
        [SerializeField] private AudioSource engineSource;
        [Min(.001f)]
        [SerializeField] private float pitchResolution;
        [Header("Kart Misc")]
        [SerializeField] private AudioSource boostSource;
        [SerializeField] private AudioClip boostWeakClip;
        [SerializeField] private AudioClip boostStrongClip;
        [SerializeField] private AudioClip overdriftClip;
        [Space]
        [SerializeField] private AudioSource itemRollingSource;
        [SerializeField] private AudioSource tyreSkidSource;
        [Header("Item Sources")]
        [SerializeField] private AudioSource itemHitSource;
        [SerializeField] private AudioClip freezerHitClip;
        [SerializeField] private AudioClip spikeTrapHitClip;
        [Space]
        [SerializeField] private AudioSource shieldStart;
        [SerializeField] private AudioSource shieldEnd;
        [SerializeField] private AudioSource shieldAmbience;
        [SerializeField] private AudioSource shieldBreak;
        [Space]
        [SerializeField] private AudioSource invincibilityMusic;
        
        public AudioSource ItemRollingSource => itemRollingSource;

        public override void Init(bool resetting) {
            Play(false);
            shieldAmbience.Stop();
            shieldEnd.Stop();
            shieldStart.Stop();
            shieldBreak.Stop();
            boostSource.Stop();
            tyreSkidSource.Stop();
            if (!resetting) {
                PauseMenu.instance.OnPause += HandlePause;
            }
        }

        void OnEnable() {
            car.Item.OnShieldStart += PlayEnableShield;
            car.Item.OnShieldEnd += PlayDisableShield;
            car.Drifting.OnBoost += PlayBoost;
            car.Drifting.OnDriftBoost += PlayDriftBoost;
            car.Drifting.OnOverdrift += PlayOverdrift;
            car.Collider.OnItemHit += PlayItemHit;
            car.Item.OnInvincibility += PlayInvincibility;
        }

        void OnDisable() {
            car.Item.OnShieldStart -= PlayEnableShield;
            car.Item.OnShieldEnd -= PlayDisableShield;
            car.Drifting.OnBoost -= PlayBoost;
            car.Drifting.OnDriftBoost -= PlayDriftBoost;
            car.Drifting.OnOverdrift -= PlayOverdrift;
            car.Collider.OnItemHit -= PlayItemHit;
            car.Item.OnInvincibility -= PlayInvincibility;
            PauseMenu.instance.OnPause -= HandlePause;
        }

        private void PlayEnableShield() {
            shieldStart.Play();
            shieldAmbience.Play();
        }

        private void PlayDisableShield(bool hit) {
            shieldAmbience.Stop();
            if (hit) {
                shieldBreak.Play();
            }
            else {
                shieldEnd.Play();
            }
        }

        private void PlayInvincibility(bool invincible) {
            if (invincible) {
                invincibilityMusic.Play();
                if (!car.IsBot) {
                    SoundManager.PauseMusic();
                }
            }
            else {
                invincibilityMusic.Stop();
                if (!car.IsBot && RaceManager.RaceStarted && !SoundManager.IsMusicPlaying()) {
                    SoundManager.PlayMusic();
                }
            }
        }

        private bool wasPlaying;
        private void HandlePause(bool paused) {
            if (paused) {
                wasPlaying = invincibilityMusic.isPlaying;
                invincibilityMusic.Pause();
                return;
            }
            if (wasPlaying) {
                invincibilityMusic.Play();    
                return;
            }
        }

        private void PlayBoost() {
            boostSource.PlayOneShot(boostStrongClip);
        }

        private void PlayDriftBoost(float boostT, int count) {
            if (boostT >= .9f) {
                boostSource.PlayOneShot(boostStrongClip);
            }
            else {
                boostSource.PlayOneShot(boostWeakClip);
            }
        }

        private void PlayOverdrift() {
            boostSource.PlayOneShot(overdriftClip);
        }

        private void PlayItemHit(ItemType item) {
            switch (item) {
                case ItemType.Freezer:
                    itemHitSource.PlayOneShot(freezerHitClip);
                    break;
                case ItemType.SpikeTrap:
                    itemHitSource.PlayOneShot(spikeTrapHitClip);
                    break;
            }
        }

        public void Play(bool isPaused) {
            if (!isPaused) {
                engineSource.Play();
                if (car.Item.IsShieldActive) {
                    shieldAmbience.Play();
                }
            }
            else {
                engineSource.Pause();
                shieldAmbience.Pause();
            }
        }

        public void PlayOneShot(AudioSource source) {
            source.PlayOneShot(source.clip);
        }

        public void Update() {
            engineSource.pitch = car.RB.linearVelocity.magnitude / pitchResolution + 1;
            if (car.state == CarDrivingState.Spinning 
             || car.state == CarDrivingState.Idle 
             && (car.Drifting.State == DriftState.Drifting || car.Drifting.State == DriftState.DriftingAfterBoost)) {
                if (!tyreSkidSource.isPlaying) tyreSkidSource.Play();
            }
            else if (tyreSkidSource.isPlaying) tyreSkidSource.Stop();
        }
    }
}
