using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CarCamera))]
[RequireComponent(typeof(CarDrifting))]
[RequireComponent(typeof(CarInput))]
public class BaseCar : MonoBehaviour
{
    
    [SerializeField] private CarMovement movement;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CarCamera camera;
    [SerializeField] private CarDrifting drifting;
    [SerializeField] private CarInput input;
    public CarMovement Movement => movement;
    public Rigidbody RB => rb;
    public CarCamera Camera => camera;
    public CarDrifting Drifting => drifting;
    public CarInput Input => input;

    void Awake()
    {
        rb.centerOfMass = new Vector3(0, -0.4f, 0);
        Application.targetFrameRate = 30;
    }
}
