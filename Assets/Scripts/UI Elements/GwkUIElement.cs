using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace GWK.UI {

    public class UIElement : MonoBehaviour {
        protected Window window;
        public bool focused => ReferenceEquals(window.CurrentFocused, this);
        [SerializeField] private UIElement selectUp;
        [SerializeField] private UIElement selectDown;
        [SerializeField] private UIElement selectRight;
        [SerializeField] private UIElement selectLeft;
        public UnityEvent OnFocusGained;
        public UnityEvent OnFocusLost;
        public void Init(Window win) {
            window = win;
            Debug.Log($"Init done! {gameObject.name}");
        }

        public void SetFocused() {
            if (focused) {
                return;
            }
            window.SetFocused(this);
            OnFocusGained.Invoke();
        }

        public void SetUnfocused() {
            if (!focused) {
                return;
            }
            OnFocusLost.Invoke();
        }

        public void OnUpDown(InputAction.CallbackContext ctx) {
            HandleUIInput(ctx.ReadValue<float>(), selectUp, selectDown);
        }
        public void OnLeftRight(InputAction.CallbackContext ctx) {
            HandleUIInput(ctx.ReadValue<float>(), selectRight, selectLeft);
        }

        private void HandleUIInput(float val, UIElement pos, UIElement neg) {
            Debug.Log($"Processing value {val} for {gameObject.name}");
            if (val > 0.5f && pos is not null) {
                // window.PollForFocusChange(this, pos);
                pos.SetFocused();
            }
            if (val < -0.5f && neg is not null) {
                // window.PollForFocusChange(this, neg);
                neg.SetFocused();
            }
        }
    }
}

