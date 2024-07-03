using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace GWK.UI {
    public class Window : MonoBehaviour {
        [SerializeField] private UIElement firstFocused;
        public UIElement FirstFocused => firstFocused;
        private UIElement currentFocused;
        public UIElement CurrentFocused => currentFocused;

        private List<UIElement> elements;
        private List<(UIElement sender, UIElement dest)> polledElements;

        void Awake() {
            elements = GetComponentsInChildren<UIElement>().ToList();
            elements.ForEach(e => e.Init(this));
            polledElements = new(elements.Count);
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
            Debug.Log($"Currently focused {element.gameObject.name}");
        }

        public void PollForFocusChange(UIElement sender, UIElement dest) {
            Debug.Log($"Polling from {sender} wanting to switch to {dest}");
            polledElements.Add((sender, dest));
            Debug.Log($"Tuple added! Current count: {polledElements.Count}");
            if (elements.Count == polledElements.Count) {
                polledElements.Where(e => e.sender.focused).First().dest.SetFocused();
                polledElements.Clear();
            }
        }
    }
}