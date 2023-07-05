using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarInput : CarComponent
{
    private float vert;
    private float horiz;
    private float jump1;
    private float jump1Prev;
    private float jump2;
    private float jump2Prev;
    private float item;
    public void GetVertical(InputAction.CallbackContext ctx) {
        if (!car.IsBot) vert = ctx.ReadValue<float>();
    }
    public void GetHorizontal(InputAction.CallbackContext ctx) {
        if (!car.IsBot)horiz = ctx.ReadValue<float>();
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
    public float AxisVert => vert;
    public float AxisHori => horiz;
    public float AxisJump1 => jump1;
    public float AxisJump2 => jump2;
    public bool AxisJump1ThisFrame { get; private set; }
    public bool AxisJump2ThisFrame { get; private set; }

    public override void Init() {
        
    }

    private void Update() {
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
    }

    public void AddToHorizontal(float amount) {
        horiz = Mathf.Clamp(horiz + amount, -1, 1);
    }

}