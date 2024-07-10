using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.DualShock;

namespace GWK.UI {
    public class ControlHint : MonoBehaviour {
        [SerializeField] private Hint confirmHint;
        [SerializeField] private Hint cancelHint;
        [SerializeField] private Hint alternativeHint;
        [SerializeField] private Hint upDownHint;
        [SerializeField] private Hint leftRightHint;
        [SerializeField] private Hint tabsHint;

        void OnEnable() {
            UpdateIcons();
        }

        void UpdateIcons() {
            string key = "Keyboard";
            if (Gamepad.all.Count > 0) {
                // bad solution :(
                Gamepad activePad = Gamepad.all[0];
                if (activePad is DualShockGamepad) {
                    key = "Dualshock";
                }
                if (activePad is XInputController) {
                    key = "XInput";
                }
                if (activePad is SwitchProControllerHID) {
                    key = "Switch";
                }
            }
            confirmHint?.SetSprite(HintDict.instance.GetIcons(key).confirm);
            cancelHint?.SetSprite(HintDict.instance.GetIcons(key).cancel);
            alternativeHint?.SetSprite(HintDict.instance.GetIcons(key).alternative);
            upDownHint?.SetSprite(HintDict.instance.GetIcons(key).upDown);
            leftRightHint?.SetSprite(HintDict.instance.GetIcons(key).leftRight);
            tabsHint?.SetSprite(HintDict.instance.GetIcons(key).tabsL);
        }
    }
}