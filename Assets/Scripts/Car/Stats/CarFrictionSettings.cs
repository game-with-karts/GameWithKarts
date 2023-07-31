using UnityEngine;
using System;

[Serializable]
public struct CarFrictionSettings
{
    public SurfaceType surface;
    [Range(0, 1)]
    public float forwardFriction;
    [Range(0, 1)]
    public float sidewaysFriction;
    public float groundResistance;
    public float gripRecoverySpeed;
}