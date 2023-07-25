using UnityEngine;
using System;

[Serializable]
public struct CarFrictionSettings
{
    public SurfaceType surface;
    [Range(0, 1)]
    public float grip;
    public float groundResistance;
    public float gripRecoverySpeed;
}