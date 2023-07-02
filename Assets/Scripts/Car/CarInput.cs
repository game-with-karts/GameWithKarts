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
        vert = ctx.ReadValue<float>();
    }
    public void GetHorizontal(InputAction.CallbackContext ctx) {
        horiz = ctx.ReadValue<float>();
    }
    public void GetJump1(InputAction.CallbackContext ctx) {
        jump1 = ctx.ReadValue<float>();
    }
    public void GetJump2(InputAction.CallbackContext ctx) {
        jump2 = ctx.ReadValue<float>();
    }
    public void GetItem(InputAction.CallbackContext ctx) {
        item = ctx.ReadValue<float>();
    }
    public float AxisVert => vert;
    public float AxisHori => horiz;
    public float AxisJump1 => jump1;
    public float AxisJump2 => jump2;

}