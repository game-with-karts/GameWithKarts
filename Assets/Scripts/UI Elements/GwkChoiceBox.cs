using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

namespace GWK.UI {
    public class ChoiceBox : UIElement {
        [SerializeField] private Color bgColourDeselected;
        [SerializeField] private Color bgColourSelected;
        private Color targetColour;
        [SerializeField] private Image bg;
        [SerializeField] private TMP_Text mainText;
        [SerializeField] private TMP_Text transitionText;
        [SerializeField] private AnimationTarget animationSettings;
        public UnityEvent<int> OnValueChanged;
        private AnimationTarget mainAnimation, transitionAnimation;
        [SerializeField] private string[] entries;
        
        private RectTransform mainTransform;
        private RectTransform transitionTransform;
        
        public bool Playing => mainAnimation is not null && mainAnimation.CurrentProgress < 1;
        private int _value = 0;
        public int Value {
            get => _value;
            set {
                _value = value;
                mainText.text = entries[value];
                OnValueChanged.Invoke(value);
            }
        }
        private readonly Gradient gradient = new() {
            colorKeys = new GradientColorKey[] {
                new(Color.white, 0),
                new(Color.white, 1),
            },
            alphaKeys = new GradientAlphaKey[] {
                new(0, 0),
                new(1, 1),
            }
        };

        void Awake() {
            targetColour = bgColourDeselected;

            mainAnimation = new();
            transitionAnimation = new();
            
            mainTransform = mainText.transform as RectTransform;
            transitionTransform = transitionText.transform as RectTransform;
        }

        public void SetColour(bool selected) {
            targetColour = selected ? bgColourSelected : bgColourDeselected;
        }

        public override void OnLeftRight(InputAction.CallbackContext ctx) {
            if (!ctx.started) {
                return;
            }
            float val = ctx.ReadValue<float>();
            int valInt = (int)(val == 0 ? val : Mathf.Sign(val));
            if (val == 0) {
                return;
            }
            Advance(valInt);
        }

        public void Advance(int delta) {
            if (Playing) {
                return;
            }
            transitionText.text = entries[Value];
            int newValue = (Value + delta) % entries.Length;
            if (newValue < 0) {
                newValue += entries.Length;
            }
            Value = newValue;
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

            mainAnimation.SetTransform(mainTransform);
            transitionAnimation.SetTransform(transitionTransform);

            transitionTransform.localPosition = mainTransform.localPosition;
            mainTransform.localPosition -= animationSettings.PositionDelta * dir;

            mainAnimation.Play();
            transitionAnimation.Play();
            SoundManager.OnHoverUI();
        }

        void Update() {
            if (mainAnimation is null || transitionAnimation is null) {
                return;
            }
            mainAnimation.Tick(Time.deltaTime);
            transitionAnimation.Tick(Time.deltaTime);
            mainText.color = gradient.Evaluate(mainAnimation.CurrentProgress);
            transitionText.color = gradient.Evaluate(1 - transitionAnimation.CurrentProgress);

            bg.color = Color.Lerp(bg.color, targetColour, 15 * Time.unscaledDeltaTime);
        }
    }
}