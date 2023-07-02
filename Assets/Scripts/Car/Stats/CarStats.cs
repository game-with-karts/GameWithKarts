using UnityEngine;

[CreateAssetMenu(fileName = "Car Stats", menuName = "Car Stats")]
public class CarStats : ScriptableObject
{
    public float maxSpeed;
    public float acceleration;
    public float maxReverseSpeed;
    public float reverseAcceleration;
    public float brakeForce;
    public float turnAngle;
    public float idleDeceleration;
}