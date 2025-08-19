using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace GWK.UI
{
    public class Button : UIElement
    {
        public enum ClickMode {
            Default,
            Confirm,
            Back
        }

        [Header("Button specific")]
        [SerializeField] private bool playSoundOnHover = false;
        [SerializeField] private ClickMode clickMode;
        [Header("Events")]
        [SerializeField] private UnityEvent onClick;
        [SerializeField] private UnityEvent onHover;
        [SerializeField] private UnityEvent onHoverEnd;
        [Header("Animations")]
        [SerializeField] private AnimationTarget hoverAnimation;
        [SerializeField] private AnimationTarget hoverEndAnimation;
        public UnityEvent OnClick => onClick;
        public UnityEvent OnHover => onHover;
        public UnityEvent OnHoverEnd => onHoverEnd;

        public void Click() => onClick.Invoke();
        public void Hover() { 
            onHover.Invoke();
            hoverAnimation.Play();
            hoverEndAnimation.Stop();
        }
        public void EndHover() {
            onHoverEnd.Invoke();
            hoverEndAnimation.Play();
            hoverAnimation.Stop();
        }

        private void Awake() {
            if (playSoundOnHover)
                onHover.AddListener(SoundManager.OnHoverUI);
            UnityAction call = clickMode switch {
                ClickMode.Default => SoundManager.OnConfirmUI,
                ClickMode.Confirm => SoundManager.OnConfirmUI,
                ClickMode.Back => SoundManager.OnBackUI,
                _ => () => {}
            };
            onClick.AddListener(call);
            hoverAnimation.SetTransform(transform as RectTransform);
            hoverEndAnimation.SetTransform(transform as RectTransform);
        }

        void Update() {
            hoverAnimation.Tick(Time.unscaledDeltaTime);
            hoverEndAnimation.Tick(Time.unscaledDeltaTime);
        }

        void OnDisable() {
            hoverAnimation.Revert();
        }
    }
}
