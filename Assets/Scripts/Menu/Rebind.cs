using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class Rebind : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string actionName;
    [SerializeField] private int bindingNumber;
    [Space]
    [SerializeField] private Button btn;
    [SerializeField] private TMP_Text keyDisplay;
    private InputAction action;
    void Awake() {
        foreach (var a in inputActions.actionMaps[0].actions) {
            if (a.name == actionName) {
                action = a;
                SetText();
                break;
            }
        }
    }

    private void SetText() {
        keyDisplay.text = InputControlPath.ToHumanReadableString(action.bindings[bindingNumber].effectivePath,
                                                                 InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    public void RebindAction() {
        btn.enabled = false;
        action.Disable();
        var rebindOperation = action.PerformInteractiveRebinding()
                                    .WithControlsExcluding("<Keyboard>/escape")
                                    .WithControlsExcluding("Mouse")
                                    .WithTargetBinding(bindingNumber)
                                    .OnMatchWaitForAnother(.1f)
                                    .OnComplete(op => OnRebind(op))
                                    .Start();
    }

    private void OnRebind(InputActionRebindingExtensions.RebindingOperation op) {
        SetText();
        btn.enabled = true;
        op.Dispose();
        action.Enable();
    }
}