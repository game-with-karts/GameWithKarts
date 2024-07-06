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
        public UnityEvent OnCancel;
        public UnityEvent OnAlternative;
        public UnityEvent<float> OnTabs;

        protected static readonly ElementSwitchLock moveLock = new((v, l) => v != 0 && l == 0);
        protected static readonly ElementSwitchLock confirmLock = new((v, l) => v == 0 && l != 0);
        protected static readonly ElementSwitchLock cancelLock = new((v, l) => v == 0 && l != 0);
        protected static readonly ElementSwitchLock altLock = new((v, l) => v == 0 && l != 0);
        protected static readonly ElementSwitchLock tabsLock = new((v, l) => v != 0 && l == 0);
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
            UIEventHandler.OnLeftRight += OnLeftRight;
            UIEventHandler.OnConfirm += OnInputConfirm;
            UIEventHandler.OnCancel += OnInputCancel;
            UIEventHandler.OnAlternative += OnInputAlternative;
            UIEventHandler.OnTabs += OnInputTabs;
        }

        public void SetUnfocused() {
            if (!focused) {
                return;
            }
            UIEventHandler.OnUpDown -= OnUpDown;
            UIEventHandler.OnLeftRight -= OnLeftRight;
            UIEventHandler.OnConfirm -= OnInputConfirm;
            UIEventHandler.OnCancel -= OnInputCancel;
            UIEventHandler.OnAlternative -= OnInputAlternative;
            UIEventHandler.OnTabs -= OnInputTabs;
            OnFocusLost.Invoke();
        }

        public void SetSelectDown(UIElement elem) => selectDown = elem;
        public void SetSelectUp(UIElement elem) => selectUp = elem;

        public virtual void OnUpDown(InputAction.CallbackContext ctx) {
            IEnumerator coroutine = HandleMoveEvents(ctx, selectUp, selectDown);
            StartCoroutine(coroutine);
        }
        public virtual void OnLeftRight(InputAction.CallbackContext ctx) {
            IEnumerator coroutine = HandleMoveEvents(ctx, selectRight, selectLeft);
            StartCoroutine(coroutine);
        }

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
            if (shouldSwitch){
                HandleUIInput(val, pos, neg);
            }
        }

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

        public virtual void OnInputCancel(InputAction.CallbackContext ctx) {
            // triggered on release
            if (!ctx.canceled) {
                if (ctx.started) {
                    cancelLock.ShouldSwitch(1);
                }
                return;
            }
            if (!focused) {
                return;
            }
            float val = Mathf.Round(ctx.ReadValue<float>());
            bool shouldSwitch = cancelLock.ShouldSwitch(val);
            if (shouldSwitch){
                OnCancel.Invoke();
            }
        }

        public virtual void OnInputAlternative(InputAction.CallbackContext ctx) {
            // triggered on release
            if (!ctx.canceled) {
                if (ctx.started) {
                    altLock.ShouldSwitch(1);
                }
                return;
            }
            if (!focused) {
                return;
            }
            float val = Mathf.Round(ctx.ReadValue<float>());
            bool shouldSwitch = altLock.ShouldSwitch(val);
            if (shouldSwitch){
                OnAlternative.Invoke();
            }
        }

        public virtual void OnInputTabs(InputAction.CallbackContext ctx) {
            if (!ctx.started) {
                return;
            }
            float val = ctx.ReadValue<float>();
            OnTabs.Invoke(val);
        }


        protected void HandleUIInput(float val, UIElement pos, UIElement neg) {
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