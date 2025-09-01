using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;
using System;

namespace GWK.UI {
    public class TextInputBox : UIElement {
        [SerializeField] private Color bgColourDeselected;
        [SerializeField] private Color bgColourSelected;
        private Color targetColour;
        [SerializeField] private Image bg;
        [SerializeField] private TMP_InputField input;
        public UnityEvent<string> OnValueChanged;
        public string Text {
            get => input.text;
            set {
                input.text = value;
                OnValueChanged.Invoke(value);
            }
        }
        void Awake() {
            targetColour = bgColourDeselected;
        }

        public void SetColour(bool selected) {
            targetColour = selected ? bgColourSelected : bgColourDeselected;
        }

        void Update() {
            bg.color = Color.Lerp(bg.color, targetColour, 15 * Time.unscaledDeltaTime);
        }
    }
}
