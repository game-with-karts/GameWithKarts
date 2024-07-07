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
            UIEventHandler.inputs.UI.UpDown.performed += OnUpDown;
            UIEventHandler.inputs.UI.LeftRight.performed += OnLeftRight;
            UIEventHandler.inputs.UI.Confirm.started += OnInputConfirm;
            UIEventHandler.inputs.UI.Cancel.started += OnInputCancel;
            UIEventHandler.inputs.UI.Alternative.started += OnInputAlternative;
            UIEventHandler.inputs.UI.Tabs.performed += OnInputTabs;
        }

        public void SetUnfocused() {
            if (!focused) {
                return;
            }
            UIEventHandler.inputs.UI.UpDown.performed -= OnUpDown;
            UIEventHandler.inputs.UI.LeftRight.performed -= OnLeftRight;
            UIEventHandler.inputs.UI.Confirm.started -= OnInputConfirm;
            UIEventHandler.inputs.UI.Cancel.started -= OnInputCancel;
            UIEventHandler.inputs.UI.Alternative.started -= OnInputAlternative;
            UIEventHandler.inputs.UI.Tabs.performed -= OnInputTabs;
            OnFocusLost.Invoke();
        }

        public void SetSelectDown(UIElement elem) => selectDown = elem;
        public void SetSelectUp(UIElement elem) => selectUp = elem;

        public virtual void OnUpDown(InputAction.CallbackContext ctx) {
            Debug.Log($"Event received for {gameObject.name}");
            HandleMoveEvents(ctx, selectUp, selectDown);
        }
        public virtual void OnLeftRight(InputAction.CallbackContext ctx) {
            HandleMoveEvents(ctx, selectRight, selectLeft);
        }

        private void HandleMoveEvents(InputAction.CallbackContext ctx, UIElement pos, UIElement neg) {
            float val = ctx.ReadValue<float>();
            val = val == 0 ? val : Mathf.Sign(val);
            HandleUIInput(val, pos, neg);
        }

        public virtual void OnInputConfirm(InputAction.CallbackContext ctx) {
            OnConfirm.Invoke();
        }

        public virtual void OnInputCancel(InputAction.CallbackContext ctx) {
            OnCancel.Invoke();
        }

        public virtual void OnInputAlternative(InputAction.CallbackContext ctx) {
            OnAlternative.Invoke();
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