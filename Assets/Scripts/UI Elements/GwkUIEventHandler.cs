using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GWK.UI {
    public delegate void InputCallback(InputAction.CallbackContext ctx);
    public static class UIEventHandler {
        public static event InputCallback OnUpDown;
        public static event InputCallback OnLeftRight;
        public static event InputCallback OnConfirm;
        public static event InputCallback OnCancel;
        public static event InputCallback OnAlternative;
        public static event InputCallback OnTabs;
        public static event InputCallback OnScroll;

        public static readonly PlayerInputActions inputs = new();
        private static readonly bool debugInputs = false;

        private static void UpDown(InputAction.CallbackContext ctx) => OnUpDown?.Invoke(ctx);
        private static void LeftRight(InputAction.CallbackContext ctx) => OnLeftRight?.Invoke(ctx);
        private static void Confirm(InputAction.CallbackContext ctx) => OnConfirm?.Invoke(ctx);
        private static void Cancel(InputAction.CallbackContext ctx) => OnCancel?.Invoke(ctx);
        private static void Alternative(InputAction.CallbackContext ctx) => OnAlternative?.Invoke(ctx);
        private static void Tabs(InputAction.CallbackContext ctx) => OnTabs?.Invoke(ctx);
        private static void Scroll(InputAction.CallbackContext ctx) => OnScroll?.Invoke(ctx);

        static UIEventHandler() {
            OnEnable();
        }

        static readonly Finaliser finaliser = new();

        static void OnEnable() {
            EnableAction(inputs.UI.UpDown, UpDown);
            EnableAction(inputs.UI.LeftRight, LeftRight);
            EnableAction(inputs.UI.Confirm, Confirm);
            EnableAction(inputs.UI.Cancel, Cancel);
            EnableAction(inputs.UI.Alternative, Alternative);
            EnableAction(inputs.UI.Tabs, Tabs);
            EnableAction(inputs.UI.Scroll, Scroll);

            if (debugInputs) {
                EnableAction(inputs.UI.UpDown, DebugAction);
                EnableAction(inputs.UI.LeftRight, DebugAction);
                EnableAction(inputs.UI.Confirm, DebugAction);
                EnableAction(inputs.UI.Cancel, DebugAction);
                EnableAction(inputs.UI.Alternative, DebugAction);
                EnableAction(inputs.UI.Tabs, DebugAction);
                EnableAction(inputs.UI.Scroll, DebugAction);
            }
        }

        static void OnDisable() {
            DisableAction(inputs.UI.UpDown, UpDown);
            DisableAction(inputs.UI.LeftRight, LeftRight);
            DisableAction(inputs.UI.Confirm, Confirm);
            DisableAction(inputs.UI.Cancel, Cancel);
            DisableAction(inputs.UI.Alternative, Alternative);
            DisableAction(inputs.UI.Tabs, Tabs);
            DisableAction(inputs.UI.Scroll, Scroll);

            if (debugInputs) {
                DisableAction(inputs.UI.UpDown, DebugAction);
                DisableAction(inputs.UI.LeftRight, DebugAction);
                DisableAction(inputs.UI.Confirm, DebugAction);
                DisableAction(inputs.UI.Cancel, DebugAction);
                DisableAction(inputs.UI.Alternative, DebugAction);
                DisableAction(inputs.UI.Tabs, DebugAction);
                DisableAction(inputs.UI.Scroll, DebugAction);
            }
        }

        private static void EnableAction(InputAction action, Action<InputAction.CallbackContext> callback) {
            action.started += callback;
            action.performed += callback;
            action.canceled += callback;
            action.Enable();
        }

        private static void DisableAction(InputAction action, Action<InputAction.CallbackContext> callback) {
            action.Disable();
            action.started -= callback;
            action.performed -= callback;
            action.canceled -= callback;
        }

        private static void DebugAction(InputAction.CallbackContext ctx) {
            Debug.Log($"{ctx.action.name} is in phase {ctx.phase} with value of {ctx.ReadValue<float>()}");
        }

        sealed class Finaliser {
            ~Finaliser() {
                OnDisable();
            }
        }
    }
}