using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;
using System;

namespace GWK.UI {
    public class Slider : UIElement {
        [SerializeField] private Color bgColourDeselected;
        [SerializeField] private Color bgColourSelected;
        private Color targetColour;
        [SerializeField] private Image bg;
        [Space]
        [SerializeField] private RectTransform fillArea;
        [SerializeField] private RectTransform fill;
        [Space]
        [SerializeField] private float minValue = 0;
        [SerializeField] private float maxValue = 1;
        [InspectorName("Value")]
        [SerializeField] private float _value = 0;
        [SerializeField] private float incDecRate = 5;
        public UnityEvent<float> OnValueChanged;
        private Vector2 targetAnchorMax;
        public float Value {
            get => _value;
            set {
                _value = value;
                targetAnchorMax.x = Mathf.InverseLerp(minValue, maxValue, value);
                OnValueChanged.Invoke(value);
            }
        }

        void Awake() {
            targetAnchorMax = new(Mathf.InverseLerp(minValue, maxValue, Value), 1f);
            targetColour = bgColourDeselected;
        }

        private static Vector2 MousePosition() {
            Vector2Control mousePos = Mouse.current.position;
            return new(mousePos.x.value, mousePos.y.value);
        }

        public void OnPointerDown(BaseEventData eventData) {
            Vector2 mousePos = MousePosition();
            bool ok = RectTransformUtility.ScreenPointToLocalPointInRectangle(fillArea, mousePos, Camera.main, out var localPoint);
            float relPos = Mathf.Clamp01((localPoint.x + fillArea.rect.width * fillArea.pivot.x) / fillArea.rect.width);
            Value = Mathf.Lerp(minValue, maxValue, relPos);
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
        }

        void Update() {
            bg.color = Color.Lerp(bg.color, targetColour, 15 * Time.unscaledDeltaTime);
            fill.anchorMax = Vector2.Lerp(fill.anchorMax, targetAnchorMax, 20 * Time.unscaledDeltaTime);
        }
    }
}