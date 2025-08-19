using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;
using System;

namespace GWK.UI {
    public class NumberInputBox : UIElement {
        [SerializeField] private Color bgColourDeselected;
        [SerializeField] private Color bgColourSelected;
        private Color targetColour;
        [SerializeField] private Image bg;
        [SerializeField] private TMP_InputField input;
        public UnityEvent<int> OnValueChanged;
        [SerializeField] private int minValue = 1;
        [SerializeField] private int maxValue = 99;
        [SerializeField] private int incDecRate = 1;
        private int _value = 0;
        public int Value {
            get => _value;
            set {
                _value = value;
                input.text = value.ToString();
                OnValueChanged.Invoke(value);
            }
        }
        void Awake() {
            targetColour = bgColourDeselected;
        }

        public void SetColour(bool selected) {
            targetColour = selected ? bgColourSelected : bgColourDeselected;
        }

        public override void OnLeftRight(InputAction.CallbackContext ctx) {
            float val = ctx.ReadValue<float>();
            int valInt = (int)(val == 0 ? val : Mathf.Sign(val));
            if (valInt == 0) {
                return;
            }
            Advance(valInt);
        }

        public void Advance(int delta) {
            Value = Math.Clamp(Value + delta * incDecRate, minValue, maxValue);
            SoundManager.OnHoverUI();
        }

        public void ParseFromText(string numStr) {
            if (!int.TryParse(numStr, out int num)) {
                Value = Value; // also resets the input text to its previous state
                return;
            }
            Value = num;
        }

        void Update() {
            bg.color = Color.Lerp(bg.color, targetColour, 15 * Time.unscaledDeltaTime);
        }
    }
}