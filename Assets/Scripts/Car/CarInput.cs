using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GWK.Kart {
    public class CarInput : CarComponent, IInputProvider {
        private PlayerInputActions inputs;


        Action<InputAction.CallbackContext> verticalDelegate;
        public event Action<float> VerticalPerformed {
            add {
                verticalDelegate = ctx => value(ctx.ReadValue<float>());
                inputs.Car.Vertical.performed += verticalDelegate;
                inputs.Car.Vertical.canceled += verticalDelegate;
                inputs.Car.Vertical.Enable();
            }
            remove {
                inputs.Car.Vertical.Disable();
                inputs.Car.Vertical.performed -= verticalDelegate;
                inputs.Car.Vertical.canceled -= verticalDelegate;
            }
        }
        Action<InputAction.CallbackContext> horizontalDelegate;
        public event Action<float> HorizontalPerformed {
            add {
                horizontalDelegate = ctx => value(ctx.ReadValue<float>());
                inputs.Car.Horizontal.performed += horizontalDelegate;
                inputs.Car.Horizontal.canceled += horizontalDelegate;
                inputs.Car.Horizontal.Enable();
            }
            remove {
                inputs.Car.Horizontal.Disable();
                inputs.Car.Horizontal.performed -= horizontalDelegate;
                inputs.Car.Horizontal.canceled -= horizontalDelegate;
            }
        }

        Action<InputAction.CallbackContext> jump1Delegate;
        public event Action<bool> Jump1 {
            add {
                jump1Delegate = ctx => value(ctx.ReadValueAsButton());
                inputs.Car.Jump1.started += jump1Delegate;
                inputs.Car.Jump1.canceled += jump1Delegate;
                inputs.Car.Jump1.Enable();
            }
            remove {
                inputs.Car.Jump1.Disable();
                inputs.Car.Jump1.started -= jump1Delegate;
                inputs.Car.Jump1.canceled -= jump1Delegate;
            }
        }

        Action<InputAction.CallbackContext> jump2Delegate;
        public event Action<bool> Jump2 {
            add {
                jump2Delegate = ctx => value(ctx.ReadValueAsButton());
                inputs.Car.Jump2.started += jump2Delegate;
                inputs.Car.Jump2.canceled += jump2Delegate;
                inputs.Car.Jump2.Enable();
            }
            remove {
                inputs.Car.Jump2.Disable();
                inputs.Car.Jump2.started -= jump2Delegate;
                inputs.Car.Jump2.canceled -= jump2Delegate;
            }
        }

        Action<InputAction.CallbackContext> itemDelegate;
        public event Action Item {
            add {
                itemDelegate = _ => value();
                inputs.Car.Item.performed += itemDelegate;
                inputs.Car.Item.Enable();
            }
            remove {
                inputs.Car.Item.Disable();
                inputs.Car.Item.performed -= itemDelegate;
            }
        }

        Action<InputAction.CallbackContext> backCameraDelegate;
        public event Action<bool> BackCamera {
            add {
                backCameraDelegate = ctx => value(ctx.ReadValueAsButton());
                inputs.Car.BackCamera.performed += backCameraDelegate;
                inputs.Car.BackCamera.canceled += backCameraDelegate;
                inputs.Car.BackCamera.Enable();
            }
            remove {
                inputs.Car.BackCamera.Disable();
                inputs.Car.BackCamera.performed -= backCameraDelegate;
                inputs.Car.BackCamera.canceled -= backCameraDelegate;
            }
        }


        public override void Init(bool _) { }

        void OnEnable() {
            if (car.IsBot) {
                return;
            }
            inputs = new();
            inputs.UI.Disable();

            SettingsMenu.OnSettingsUpdated += UpdateInputOverrides;
        }

        void OnDisable() {
            SettingsMenu.OnSettingsUpdated -= UpdateInputOverrides;
        }

        private void UpdateInputOverrides() {
            inputs.LoadBindingOverridesFromJson(PlayerPrefs.GetString(SettingsMenu.BindingOverridesKey));
        }

    }
}