using System.Collections;
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
        public UnityEvent OnConfirm;
        public void Init(Window win) {
            window = win;
            // GetComponent<PlayerInput>().enabled = false;
        }

        public void SetFocused() {
            if (focused) {
                return;
            }
            window.SetFocused(this);
            OnFocusGained.Invoke();
            UIEventHandler.OnUpDown += OnUpDown;
            UIEventHandler.OnLeftRight += OnUpDown;
            UIEventHandler.OnConfirm += OnInputConfirm;
        }

        public void SetUnfocused() {
            if (!focused) {
                return;
            }
            UIEventHandler.OnUpDown -= OnUpDown;
            UIEventHandler.OnLeftRight -= OnUpDown;
            UIEventHandler.OnConfirm -= OnInputConfirm;
            OnFocusLost.Invoke();
        }

        public virtual void OnUpDown(InputAction.CallbackContext ctx) {
            IEnumerator coroutine = HandleMoveEvents(ctx, selectUp, selectDown);
            StartCoroutine(coroutine);
        }
        public virtual void OnLeftRight(InputAction.CallbackContext ctx) {
            IEnumerator coroutine = HandleMoveEvents(ctx, selectRight, selectLeft);
            StartCoroutine(coroutine);
        }

        private static readonly ElementSwitchLock moveLock = new((v, l) => v != 0 && l == 0);
        // no idea if this even should be a coroutine but it works so idgaf :)
        private IEnumerator HandleMoveEvents(InputAction.CallbackContext ctx, UIElement pos, UIElement neg) {
            if (!ctx.started) {
                if (ctx.canceled) {
                    moveLock.ShouldSwitch(0);
                }
                yield break;
            }
            yield return new WaitForFixedUpdate();
            // float val = Mathf.Clamp(Mathf.Round(ctx.ReadValue<float>()) * 10f, -1, 1);
            float val = ctx.ReadValue<float>();
            val = val == 0 ? val : Mathf.Sign(val);
            bool shouldSwitch = moveLock.ShouldSwitch(val);
            Debug.Log($"{ctx.ReadValue<float>()} {val}");
            if (shouldSwitch){
                HandleUIInput(val, pos, neg);
            }
        }
        private static readonly ElementSwitchLock confirmLock = new((v, l) => v == 0 && l != 0);

        public virtual void OnInputConfirm(InputAction.CallbackContext ctx) {
            // triggered on release
            if (!ctx.canceled) {
                if (ctx.started) {
                    confirmLock.ShouldSwitch(1);
                }
                return;
            }
            if (!focused) {
                return;
            }
            float val = Mathf.Round(ctx.ReadValue<float>());
            bool shouldSwitch = confirmLock.ShouldSwitch(val);
            if (shouldSwitch){
                OnConfirm.Invoke();
            }
        }


        private void HandleUIInput(float val, UIElement pos, UIElement neg) {
            if (!focused) {
                return;
            }
            if (val > 0f && pos is not null) {
                pos.SetFocused();
            }
            if (val < 0f && neg is not null) {
                neg.SetFocused();
            }
        }
    }
}