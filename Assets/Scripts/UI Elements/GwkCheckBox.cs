using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

namespace GWK.UI {
    public class CheckBox : UIElement {
        [SerializeField] private Image bg;
        [SerializeField] private Image tick;
        [SerializeField] private Color bgColourDeselected;
        [SerializeField] private Color bgColourSelected;
        private Color targetColour;
        private bool _value;
        public bool Value {
            get => _value;
            set {
                _value = value;
                tick.enabled = value;
                OnValueChanged.Invoke(value);
            }
        }
        public UnityEvent<bool> OnValueChanged;

        void Awake() {
            targetColour = bgColourDeselected;
        }

        public void Toggle() {
            Value = !Value;
        }

        public void SetColour(bool selected) {
            targetColour = selected ? bgColourSelected : bgColourDeselected;
        }

        void Update() {
            bg.color = Color.Lerp(bg.color, targetColour, 15 * Time.unscaledDeltaTime);
        }
    }
}