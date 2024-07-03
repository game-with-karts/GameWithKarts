using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;

namespace GWK.UI {
    public class Window : MonoBehaviour {
        [SerializeField] private UIElement firstFocused;
        public UIElement FirstFocused => firstFocused;
        private UIElement currentFocused;
        public UIElement CurrentFocused => currentFocused;

        private List<UIElement> elements;

        void Awake() {
            elements = GetComponentsInChildren<UIElement>().ToList();
            elements.ForEach(e => e.Init(this));
        }

        void OnEnable() {
            Debug.Log($"OnEnable done! Window");
            firstFocused.SetFocused();
        }

        void OnDisable() {
            currentFocused.SetUnfocused();
            currentFocused = null;
        }

        public void SetFocused(UIElement element) {
            currentFocused?.SetUnfocused();
            currentFocused = element;
        }
    }
}