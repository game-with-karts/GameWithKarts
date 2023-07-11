using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : CarComponent
{
    [SerializeField] private CarStats stats;
    [SerializeField] private float reverseThreshold;
    [SerializeField] private float downforceAmount;
    [SerializeField] private float boostingSpeed = 40;
    [SerializeField] private float boostingAcceleration = 40;
    [SerializeField] private CarWheelRaycaster[] wheels;
    [SerializeField] private Transform wheel_fr;
    [SerializeField] private Transform wheel_fl;
    [SerializeField] private Transform wheel_rr;
    [SerializeField] private Transform wheel_rl;
    [SerializeField] private float gravity;
    [Range(0, 1)]
    [SerializeField] private float iceGrip;
    [SerializeField] private bool isAntigrav;
    [SerializeField] private float normalRayLength;
    [SerializeField] private LayerMask normalRayLayers;

    private float currSpeed = 0;
    private Vector3 normal = Vector3.up;

    bool isReversing;
    bool isBraking;
    public bool IsReversing => isReversing;
    public bool IsBraking => isBraking;
    public bool IsAntigrav => isAntigrav;
    public void SetStats(CarStats stats) => this.stats = stats;
    public bool IsGrounded { 
        get {
            bool g = false;
            foreach (var w in wheels) {
                g |= w.isGrounded;
            }
            return g;
        }
    }

    public bool IsFullyGrounded { 
        get {
            bool g = false;
            foreach (var w in wheels) {
                g &= w.isGrounded;
            }
            return g;
        }
    }

    private Vector3 localUp = Vector3.up;
    public Vector3 LocalUp => localUp;

    void Update() {
        foreach (var wheel in wheels) {
            wheel.localUp = localUp;
        }
    }

    void FixedUpdate() {
        if (Physics.Raycast(transform.position, -transform.up, out var hit, normalRayLength, normalRayLayers)) {
            normal = hit.normal;
        }
        localUp = (isAntigrav ? normal : Vector3.up);
        Vector3 vel = car.RB.velocity;
        Vector3 localVel = transform.InverseTransformDirection(car.RB.velocity);
        isReversing = Vector3.Dot(transform.forward, car.RB.velocity.normalized) < 0 && car.RB.velocity.magnitude > reverseThreshold;
        isBraking = Vector3.Dot(vel.normalized, transform.forward * car.Input.AxisVert) < 0;

        SurfaceType surface = GetSurface();
        if(!car.IsBot) print(surface);

        CorrectVelocityVector(surface == SurfaceType.Ice ? iceGrip : 1);

        if (car.Drifting.isBoosting) {
            Move(vel, boostingSpeed, boostingAcceleration);
        }
        else {
            if (isBraking) {
                Brake(vel);
            }
            else {
                if (car.Input.AxisVert > 0) Move(vel, stats.maxSpeed, stats.acceleration);
                else if (car.Input.AxisVert < 0) Reverse(vel);
                else currSpeed = 0;
            }
        }

        car.RB.velocity += currSpeed * transform.forward;

        float groundResistance = 1;
        if (surface == SurfaceType.Ground) groundResistance = 5;
        car.RB.velocity -= car.RB.velocity * stats.idleDeceleration * groundResistance * Time.fixedDeltaTime;

        float turnAmount = Mathf.Clamp(localVel.z, -stats.maxSpeed, stats.maxSpeed);
        if (!IsGrounded) turnAmount = stats.maxSpeed;
        float turnAngle = stats.turnAngle * turnAmount * (car.Input.AxisHori + car.Drifting.DriftDirection);
        car.RB.angularVelocity = localUp * turnAngle;
        //align the normals to the normal vector
        if (!IsGrounded)
        {
            float angle = Vector3.Angle(localUp, transform.up);
            Vector3 perpendicular = Vector3.Cross(localUp, transform.up);
            transform.RotateAround(transform.position, perpendicular, -angle * 4 * Time.fixedDeltaTime); 
        }
        
        // downforce
        car.RB.AddForce(-transform.up * downforceAmount * (vel.magnitude / stats.maxSpeed));

        // gravity
        car.RB.AddForce(-gravity * car.RB.mass * localUp);
    }

    private void Reverse(Vector3 vel) {
        currSpeed = -Mathf.Lerp(stats.reverseAcceleration, 0, vel.magnitude / stats.maxReverseSpeed) * Time.fixedDeltaTime;
    }

    private void Move(Vector3 vel, float maxSpeed, float accel) {
        currSpeed = Mathf.Lerp(accel, 0, vel.magnitude / maxSpeed) * Time.fixedDeltaTime;
    }

    private void Brake(Vector3 vel) {
        float brake = stats.brakeForce * ((vel.magnitude + stats.brakeForce / 2)/ stats.maxSpeed) * -car.Input.AxisVert;
        currSpeed = -brake * Time.fixedDeltaTime;
    }

    private void UpdateWheelPosition(WheelCollider collider, Transform wheel) {
        collider.GetWorldPose(out var pos, out var rot);
        wheel.SetPositionAndRotation(pos, rot);
    }
    
    void CorrectVelocityVector(float grip) {
        Vector3 vel = transform.InverseTransformDirection(car.RB.velocity);
        float velHoriz = vel.z;
        float velVert = vel.y;
        float gripExp = Mathf.Pow(.005f, 1 - grip);
        if (!car.IsBot) print(gripExp);
        Vector3 finalVector = Vector3.Lerp(car.RB.velocity, transform.forward * velHoriz + transform.up * velVert, gripExp);
        car.RB.velocity = finalVector;
    }

    public SurfaceType GetSurface() {
        SurfaceType s = SurfaceType.Ice;
        foreach(var w in wheels)
        {
            if ((int)w.surface > (int)s) s = w.surface;
        }
        return s;
    }

    public void SetAntigrav(bool antigrav) => this.isAntigrav = antigrav;

    public override void Init() {
        return;
    }
}
