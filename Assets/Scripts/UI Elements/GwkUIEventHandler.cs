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

        public void UpDown(InputAction.CallbackContext ctx) => OnUpDown?.Invoke(ctx);
        public void LeftRight(InputAction.CallbackContext ctx) => OnLeftRight?.Invoke(ctx);
        public void Confirm(InputAction.CallbackContext ctx) => OnConfirm?.Invoke(ctx);
        public void Cancel(InputAction.CallbackContext ctx) => OnCancel?.Invoke(ctx);
        public void Alternative(InputAction.CallbackContext ctx) => OnAlternative?.Invoke(ctx);
    }
}