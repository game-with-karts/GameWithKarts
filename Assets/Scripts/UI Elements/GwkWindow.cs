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

        private List<UIElement> elements;

        void Awake() {
            elements = GetComponentsInChildren<UIElement>(true).ToList();
            elements.ForEach(e => e.Init(this));
        }

        void OnEnable() {
            firstFocused.SetFocused(UINavigationInfo.Empty);
        }

        void OnDisable() {
            currentFocused.SetUnfocused();
            currentFocused = null;
        }

        public void SetFocused(UIElement element) {
            currentFocused?.SetUnfocused();
            currentFocused = element;
        }

        public void SetFirstFocused(UIElement elem) => firstFocused = elem;
    }
}