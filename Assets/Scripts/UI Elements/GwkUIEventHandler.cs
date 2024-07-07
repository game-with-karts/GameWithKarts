using System;
using UnityEngine;
using UnityEngine.InputSystem;



namespace GWK.UI {
    public delegate void InputCallback(InputAction.CallbackContext ctx);
    public class UIEventHandler : MonoBehaviour {
        public static event InputCallback OnUpDown;
        public static event InputCallback OnLeftRight;
        public static event InputCallback OnConfirm;
        public static event InputCallback OnCancel;
        public static event InputCallback OnAlternative;
        public static event InputCallback OnTabs;

        public static PlayerInputActions inputs { get; private set; }
        private readonly bool debugInputs = false;

        private void UpDown(InputAction.CallbackContext ctx) => OnUpDown?.Invoke(ctx);
        private void LeftRight(InputAction.CallbackContext ctx) => OnLeftRight?.Invoke(ctx);
        private void Confirm(InputAction.CallbackContext ctx) => OnConfirm?.Invoke(ctx);
        private void Cancel(InputAction.CallbackContext ctx) => OnCancel?.Invoke(ctx);
        private void Alternative(InputAction.CallbackContext ctx) => OnAlternative?.Invoke(ctx);
        private void Tabs(InputAction.CallbackContext ctx) => OnTabs?.Invoke(ctx);

        void Awake() {
            inputs = new();
            DontDestroyOnLoad(gameObject);
        }

        void OnEnable() {
            EnableAction(inputs.UI.UpDown, UpDown);
            EnableAction(inputs.UI.LeftRight, LeftRight);
            EnableAction(inputs.UI.Confirm, Confirm);
            EnableAction(inputs.UI.Cancel, Cancel);
            EnableAction(inputs.UI.Alternative, Alternative);
            EnableAction(inputs.UI.Tabs, Tabs);

            if (debugInputs) {
                EnableAction(inputs.UI.UpDown, DebugAction);
                EnableAction(inputs.UI.LeftRight, DebugAction);
                EnableAction(inputs.UI.Confirm, DebugAction);
                EnableAction(inputs.UI.Cancel, DebugAction);
                EnableAction(inputs.UI.Alternative, DebugAction);
                EnableAction(inputs.UI.Tabs, DebugAction);
            }
        }

        void OnDisable() {
            DisableAction(inputs.UI.UpDown, UpDown);
            DisableAction(inputs.UI.LeftRight, LeftRight);
            DisableAction(inputs.UI.Confirm, Confirm);
            DisableAction(inputs.UI.Cancel, Cancel);
            DisableAction(inputs.UI.Alternative, Alternative);
            DisableAction(inputs.UI.Tabs, Tabs);

            if (debugInputs) {
                DisableAction(inputs.UI.UpDown, DebugAction);
                DisableAction(inputs.UI.LeftRight, DebugAction);
                DisableAction(inputs.UI.Confirm, DebugAction);
                DisableAction(inputs.UI.Cancel, DebugAction);
                DisableAction(inputs.UI.Alternative, DebugAction);
                DisableAction(inputs.UI.Tabs, DebugAction);
            }
        }

        private void EnableAction(InputAction action, Action<InputAction.CallbackContext> callback) {
            action.started += callback;
            action.performed += callback;
            action.canceled += callback;
            action.Enable();
        }

        private void DisableAction(InputAction action, Action<InputAction.CallbackContext> callback) {
            action.started -= callback;
            action.performed -= callback;
            action.canceled -= callback;
            action.Disable();
        }

        private void DebugAction(InputAction.CallbackContext ctx) {
            Debug.Log($"{ctx.action.name} is in phase {ctx.phase} with value of {ctx.ReadValue<float>()}");
        }
    }
}