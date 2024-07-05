using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

namespace GWK.UI {
    public class Slideshow : UIElement {
        [Header("Slideshow specific")]
        [SerializeField] private Image mainImage;
        [SerializeField] private Image transitionImage;
        [SerializeField] private TMP_Text mainLabel;
        [SerializeField] private TMP_Text transitionLabel;
        [Space]
        [SerializeField] private UnityEvent<int> OnSelected;
        [Space]
        [SerializeField] private AnimationTarget animationSettings;
        private AnimationTarget mainAnimation, transitionAnimation;
        [Space]
        [SerializeField] private SlideshowEntry[] entries;
        private int currentIdx;
        public int CurrentIdx {
            get => currentIdx;
            set {
                currentIdx = value;
                if (currentIdx < 0) {
                    currentIdx += entries.Length;
                }
                SetGraphic();
            }
        }
        private Gradient gradient;
        public bool Playing => mainAnimation is not null && mainAnimation.CurrentProgress < 1;

        private RectTransform mainImgTransform;
        private RectTransform transitionImgTransform;

        void Awake() {
            gradient = new() {
                colorKeys = new GradientColorKey[] {
                    new(Color.white, 0),
                    new(Color.white, 1),
                },
                alphaKeys = new GradientAlphaKey[] {
                    new(0, 0),
                    new(1, 1),
                }
            };

            mainAnimation = new();
            transitionAnimation = new();
            
            mainImgTransform = mainImage.transform as RectTransform;
            transitionImgTransform = transitionImage.transform as RectTransform;

            animationSettings.SetTransform(mainImgTransform);
        }

        void OnEnable() {
            CurrentIdx = 0;
        }

        void SetGraphic() {
            mainImage.sprite = entries[CurrentIdx].image;
            mainLabel.text = entries[CurrentIdx].caption;
        }

        public void Advance(float d) {
            if (Playing) {
                return;
            }
            int delta = (int)d;
            transitionImage.sprite = entries[CurrentIdx].image;
            CurrentIdx = (CurrentIdx + delta) % entries.Length;

            Play(-delta);
        }

        void Play(int dir) {
            mainAnimation = new(
                animationSettings.PositionDelta * dir,
                animationSettings.EulerTarget,
                animationSettings.ScaleTarget,
                animationSettings.Duration,
                animationSettings.Curve
            );
            transitionAnimation = new(
                animationSettings.PositionDelta * dir,
                animationSettings.EulerTarget,
                animationSettings.ScaleTarget,
                animationSettings.Duration,
                animationSettings.Curve
            );

            mainAnimation.SetTransform(mainImgTransform);
            transitionAnimation.SetTransform(transitionImgTransform);

            transitionImgTransform.localPosition = mainImgTransform.localPosition;
            mainImgTransform.localPosition -= animationSettings.PositionDelta * dir;

            mainAnimation.Play();
            transitionAnimation.Play();
        }

        void Update() {
            if (mainAnimation is null || transitionAnimation is null) {
                return;
            }
            mainAnimation.Tick(Time.deltaTime);
            transitionAnimation.Tick(Time.deltaTime);
            mainImage.color = gradient.Evaluate(mainAnimation.CurrentProgress);
            transitionImage.color = gradient.Evaluate(1 - transitionAnimation.CurrentProgress);
        }

        public void SelectTrack() {
            OnSelected.Invoke(entries[CurrentIdx].trackIndex);
        }
    }
    [Serializable]
    public struct SlideshowEntry {
        public Sprite image;
        public string caption;
        public int trackIndex;
    }
}