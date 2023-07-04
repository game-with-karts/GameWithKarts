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
    private float jump2;
    private float item;
    public void GetVertical(InputAction.CallbackContext ctx) {
        if (!car.IsBot) vert = ctx.ReadValue<float>();
    }
    public void GetHorizontal(InputAction.CallbackContext ctx) {
        if (!car.IsBot)horiz = ctx.ReadValue<float>();
    }
    public void GetJump1(InputAction.CallbackContext ctx) {
        if (!car.IsBot)jump1 = ctx.ReadValue<float>();
    }
    public void GetJump2(InputAction.CallbackContext ctx) {
        if (!car.IsBot)jump2 = ctx.ReadValue<float>();
    }
    public void GetItem(InputAction.CallbackContext ctx) {
        if (!car.IsBot)item = ctx.ReadValue<float>();
    }
    public float AxisVert => vert;
    public float AxisHori => horiz;
    public float AxisJump1 => jump1;
    public float AxisJump2 => jump2;

    public override void Init()
    {
        if (car.IsBot) {
            print("bot");
        }
    }

    public void SetAxes(float vert, float horiz, float jump1, float jump2, float item) {
        this.vert = vert;
        this.horiz = horiz;
        this.jump1 = jump1;
        this.jump2 = jump2;
        this.item = item;
    }

}