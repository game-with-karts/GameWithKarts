using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace GWK.UI {
    public class Window : MonoBehaviour {
        [SerializeField] private UIElement firstFocused;
        public UIElement FirstFocused => firstFocused;
        private UIElement currentFocused;
        public UIElement CurrentFocused => currentFocused;
        [SerializeField] private UnityEvent OnBackPressed;

        private List<UIElement> elements;

        void Awake() {
            elements = GetComponentsInChildren<UIElement>().ToList();
            elements.ForEach(e => e.Init(this));
        }

        void OnEnable() {
            firstFocused.SetFocused();
            UIEventHandler.OnCancel += Back;
        }

        void OnDisable() {
            UIEventHandler.OnCancel -= Back;
            currentFocused.SetUnfocused();
            currentFocused = null;
        }

        public void SetFocused(UIElement element) {
            currentFocused?.SetUnfocused();
            currentFocused = element;
        }

        public void Back(InputAction.CallbackContext ctx) {
            Debug.Log("Back button pressed");
            if (!ctx.performed) {
                return;
            }
            OnBackPressed.Invoke();
        }
    }
}