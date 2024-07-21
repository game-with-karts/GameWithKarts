using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

namespace GWK.Kart {
    public class CarAppearance : CarComponent {
        private Volume volume;
        private ChromaticAberration ca;
        private LensDistortion lens;
        [SerializeField] private Transform model;
        [SerializeField] private Transform skidmarksParent;
        [SerializeField] private float jumpAmount;
        [SerializeField] private float landAmount;
        [SerializeField] private float animationSpeed;
        [SerializeField] private float defaultFOV = 60;
        [SerializeField] private float boostFOV = 80;
        [SerializeField] private ParticleSystem speedLines;
        [SerializeField] private TrailRenderer[] skidmarks;
        [SerializeField] private Gradient skidmarkDriftGradient;
        [SerializeField] private Gradient skidmarkIceGradient;
        [Space]
        [SerializeField] private List<VisualEffect> fireExhausts;
        [SerializeField] private Gradient fireNormalBoost;
        [SerializeField] private Gradient fireSuperBoost;
        [SerializeField] private Gradient fireUltraBoost;
        [SerializeField] private Gradient fireUltimateBoost;
        [Space]
        [SerializeField] private AnimationCurve chromaticAberrationCurve;
        private float caAmount;
        private float caTime;

        private readonly Vector3 defaultScale = new(1, 1, 1);
        private readonly Vector3 rotationCorrect = new(0, 360, 0);
        private Quaternion currentRot = Quaternion.Euler(0, 90, 0);

        private float targetBoostTime = 0;

        public const float HIT_ANIMATION_LENGTH = 3;
        private float hitAnimTime = 0;
        private Quaternion hitRotation = Quaternion.identity;
        private Vector3 hitPosition = Vector3.zero;
        private readonly Vector3 localPos = new(0, -.75f, 0);

        public const float SPIN_ANIMATION_LENGTH = 1.5f;
        private float spinAnimTime = 0;
        private Quaternion spinRotation = Quaternion.identity;

        private bool usePost;
        public override void Init(bool restarting) {
            if (!restarting) {
                car.Drifting.OnJump += JumpAnimation;
                car.Drifting.OnLand += LandAnimation;
                car.Drifting.OnDriftBoost += DriftEffect;

                usePost = PlayerPrefs.GetInt(SettingsMenu.EnablePostProcessingKey) == 1;
                if (usePost) {
                    volume = GameObject.FindGameObjectWithTag("Global Volume")?.GetComponent<Volume>();
                    volume?.profile.TryGet(out ca);
                    volume?.profile.TryGet(out lens);
                }
            }
            speedLines.Stop();

            fireExhausts.ForEach(c => c.Stop());
        }

        public void PlayHitAnimation() {
            hitAnimTime = 0;
            hitRotation = Quaternion.identity;
        }

        public void PlaySpinAnimation() {
            spinAnimTime = 0;
        }

        void Update() {
            model.localScale += (defaultScale - model.localScale) * animationSpeed * Time.deltaTime;

            Vector3 rotDelta = new Vector3(0, 30 * car.Drifting.DriftDirection + 90, 0) - currentRot.eulerAngles;

            if (car.state == CarDrivingState.Hit) {
                hitAnimTime = Mathf.Clamp(hitAnimTime + Time.deltaTime, 0, HIT_ANIMATION_LENGTH);
                float hitAnimStep = Mathf.Ceil(3 - hitAnimTime);

                float hitRotDelta = -80 * (hitAnimTime - HIT_ANIMATION_LENGTH) * (hitAnimTime - HIT_ANIMATION_LENGTH);
                float hitPosDelta = -2 * hitAnimTime + 6;
                hitRotation = Quaternion.Euler(Vector3.forward * hitRotDelta);
                hitPosition = Mathf.Abs(Mathf.Sin((-hitAnimTime - 5.2f) * (-hitAnimTime - 5.2f) / 9f * Mathf.PI)) * Vector3.up * hitPosDelta;
            }
            else {
                hitRotation = Quaternion.identity;
                hitPosition = Vector3.zero;
            }

            if (car.state == CarDrivingState.Spinning) {
                spinAnimTime = Mathf.Clamp(spinAnimTime + Time.deltaTime, 0, SPIN_ANIMATION_LENGTH);
                spinRotation = Quaternion.Euler(Vector3.up * 720 * spinAnimTime);
            }
            else {
                spinRotation = Quaternion.identity;
            }

            currentRot *= Quaternion.Euler(rotDelta * animationSpeed * Time.deltaTime);
            
            model.localRotation = currentRot * Quaternion.Euler(0, -90, 0) * hitRotation * spinRotation;
            model.localPosition = localPos + hitPosition;
            skidmarksParent.localRotation = currentRot * Quaternion.Euler(0, -90, 0);

            
            float targetFOV = car.Drifting.isBoosting ? boostFOV * BoostTierOperations.AsFloat(car.Drifting.BoostTier) : defaultFOV;
            car.Camera.FrontFacingCamera.fieldOfView = Mathf.Lerp(car.Camera.FrontFacingCamera.fieldOfView, targetFOV, animationSpeed * Time.deltaTime);
            car.Camera.BackFacingCamera.fieldOfView = Mathf.Lerp(car.Camera.FrontFacingCamera.fieldOfView, targetFOV, animationSpeed * Time.deltaTime);
            if (usePost && volume is not null) {
                ca.intensity.value = chromaticAberrationCurve.Evaluate(caTime) * caAmount;
                lens.intensity.value = chromaticAberrationCurve.Evaluate(caTime) * caAmount * -.75f;
                caTime += Time.deltaTime;
            }

            if (car.Drifting.isBoosting) {
                if (speedLines.isStopped) {
                    speedLines.Play();
                }
                fireExhausts.ForEach(c => {
                    c.Play();
                    c.SetGradient("Colour Gradient", car.Drifting.BoostTier switch {
                        BoostTier.Normal => fireNormalBoost,
                        BoostTier.Super => fireSuperBoost,
                        BoostTier.Ultra => fireUltraBoost,
                        BoostTier.Ultimate => fireUltimateBoost,
                        _ => fireNormalBoost
                    });
                    c.SetFloat("Spread", ((float)car.Drifting.BoostTier - 1) / 3);
                });

            }
            else {
                speedLines.Stop();
                fireExhausts.ForEach(c => c.Stop());
            }

            SurfaceType surfaceType = car.Movement.GetSurface();

            foreach(var r in skidmarks) {
                r.colorGradient = surfaceType == SurfaceType.Ice ? skidmarkIceGradient : skidmarkDriftGradient;
                r.emitting = car.Drifting.IsDrifting || (surfaceType == SurfaceType.Ice && car.Movement.IsGrounded);
            }
        }

        void JumpAnimation() {
            model.localScale = new Vector3(.5f, 2, .5f) * jumpAmount + defaultScale;
        }

        void LandAnimation() {
            model.localScale = new Vector3(2, .5f, 2) * landAmount + defaultScale;
        }

        void DriftEffect(float relTime, int _)  {
            caAmount = Mathf.Clamp01(1 - relTime);
            caTime = 0;
        }
    }
}