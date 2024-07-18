using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GWK.Kart {
    public class CarInput : CarComponent
    {
        private float vert;
        private float horiz;
        private float jump1;
        private float jump1Prev;
        private float jump2;
        private float jump2Prev;
        private float item;
        private float backCamera;
        private PlayerInputActions inputs;
        public void GetVertical(InputAction.CallbackContext ctx) {
            if (!car.IsBot) vert = ctx.ReadValue<float>();
        }
        public void GetHorizontal(InputAction.CallbackContext ctx) {
            if (!car.IsBot) horiz = ctx.ReadValue<float>();;
        }
        public void GetJump1(InputAction.CallbackContext ctx) {
            if (!car.IsBot) {
                jump1 = ctx.ReadValue<float>();
                AxisJump1ThisFrame = jump1 - jump1Prev > 0;
            }
        }
        public void GetJump2(InputAction.CallbackContext ctx) {
            if (!car.IsBot) {
                jump2 = ctx.ReadValue<float>();
                AxisJump2ThisFrame = jump2 - jump2Prev > 0;
            }
        }
        public void GetItem(InputAction.CallbackContext ctx) {
            if (!car.IsBot)item = ctx.ReadValue<float>();
        }

        public void GetBackCamera(InputAction.CallbackContext ctx) {
            if (!car.IsBot)backCamera = Mathf.Round(ctx.ReadValue<float>());
        }
        public float AxisVert => vert;
        public float AxisHori => horiz;
        public float AxisJump1 => jump1;
        public float AxisJump2 => jump2;
        public bool AxisJump1ThisFrame { get; private set; }
        public bool AxisJump2ThisFrame { get; private set; }
        public float BackCamera => backCamera;

        public override void Init(bool _) {}

        void OnEnable() {
            if (car.IsBot) {
                return;
            }
            inputs = new();

            EnableAction(inputs.Car.Vertical, GetVertical);
            EnableAction(inputs.Car.Horizontal, GetHorizontal);
            EnableAction(inputs.Car.Jump1, GetJump1);
            EnableAction(inputs.Car.Jump2, GetJump2);
            EnableAction(inputs.Car.Item, GetItem);
            EnableAction(inputs.Car.BackCamera, GetBackCamera);

            SettingsMenu.OnSettingsUpdated += UpdateInputOverrides;
        }

        void OnDisable() {
            DisableAction(inputs.Car.Vertical, GetVertical);
            DisableAction(inputs.Car.Horizontal, GetHorizontal);
            DisableAction(inputs.Car.Jump1, GetJump1);
            DisableAction(inputs.Car.Jump2, GetJump2);
            DisableAction(inputs.Car.Item, GetItem);
            DisableAction(inputs.Car.BackCamera, GetBackCamera);
            
            SettingsMenu.OnSettingsUpdated -= UpdateInputOverrides;
        }

        private void UpdateInputOverrides() {
            inputs.LoadBindingOverridesFromJson(PlayerPrefs.GetString(SettingsMenu.BindingOverridesKey));
        }

        private void EnableAction(InputAction action, Action<InputAction.CallbackContext> func) {
            action.performed += func;
            action.canceled += func;
            action.Enable();
        }

        private void DisableAction(InputAction action, Action<InputAction.CallbackContext> func) {
            action.performed -= func;
            action.canceled -= func;
            action.Disable();
        }

        private void Update() {
            if (!car.Movement.IsControlable) return;
            AxisJump1ThisFrame = jump1 - jump1Prev == 1;
            AxisJump2ThisFrame = jump2 - jump2Prev == 1;
            jump1Prev = jump1;
            jump2Prev = jump2;
        }

        public void SetAxes(float vert, float horiz, float jump1, float jump2, float item) {
            this.vert = vert;
            this.horiz = horiz;
            this.jump1 = jump1;
            this.jump2 = jump2;
            this.item = item;

            AxisJump1ThisFrame = jump1 - jump1Prev == 1;
            AxisJump2ThisFrame = jump2 - jump2Prev == 1;
            jump1Prev = jump1;
            jump2Prev = jump2;
        }

        public void AddToHorizontal(float amount) {
            horiz = Mathf.Clamp(horiz + amount, -1, 1);
        }

    }
}