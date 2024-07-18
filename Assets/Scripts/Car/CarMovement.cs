using System;
using System.Collections;
using UnityEngine;

namespace GWK.Kart {
    public class CarMovement : CarComponent {
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
        [SerializeField] private CarFrictionSettings[] frictionSettings;
        [SerializeField] private bool isAntigrav;
        [SerializeField] private float normalRayLength;
        [SerializeField] private LayerMask normalRayLayers;
        private bool controlable;
        public bool IsControlable => controlable;

        private float currSpeed = 0;
        private Vector3 normal = Vector3.up;

        private SurfaceType? surfaceOverride = null;

        bool isReversing;
        bool isBraking;
        public bool IsReversing => isReversing;
        public bool IsBraking => isBraking;
        public bool IsAntigrav => isAntigrav;
        public bool IsAffectedByGravity { get; set; }
        public Vector3 startingPosition { get; set; }
        public Quaternion startingRotation { get; set; }
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
                bool g = true;
                foreach (var w in wheels) {
                    g &= w.isGrounded;
                }
                return g;
            }
        }

        /// <summary>
        /// DO NOT USE THIS ALONE!!! USE currentFrictionSettings INSTEAD!!!
        /// </summary>
        private CarFrictionSettings cfs;
        private CarFrictionSettings currentFrictionSettings {
            get {
                SurfaceType st = GetSurface();
                if (cfs.surface != st)
                    cfs = Array.Find(frictionSettings, x => x.surface == GetSurface());
                return cfs;
            }
        }
        /// <summary>
        /// DO NOT USE THIS ALONE!!! USE currentForwardFriction INSTEAD!!!
        /// </summary>
        private float cff = 1f;
        /// <summary>
        /// DO NOT USE THIS ALONE!!! USE currentSidewaysFriction INSTEAD!!!
        /// </summary>
        private float csf = 1f;
        private void UpdateCurrentGrip() {
            if (cff < currentFrictionSettings.forwardFriction)
                cff += Time.fixedDeltaTime * currentFrictionSettings.gripRecoverySpeed;
            if (csf < currentFrictionSettings.sidewaysFriction)
                csf += Time.fixedDeltaTime * currentFrictionSettings.gripRecoverySpeed;
        }
        private float currentForwardFriction {
            get => cff;
            set {
                if (value < cff) {
                    cff = value;
                }
            }
        }

        private float currentSidewaysFriction {
            get => csf;
            set {
                if (value < csf) {
                    csf = value;
                }
            }
        }

        private Vector3 localUp = Vector3.up;
        public Vector3 LocalUp => localUp;
        private Vector3 orientationUp = Vector3.up;

        public override void Init(bool _) {
            controlable = false;
            IsAffectedByGravity = true;
            currSpeed = 0;
            StartCoroutine(StopAllMotion(startingPosition, startingRotation, 5));
            transform.rotation = startingRotation;
        }

        public override void StartRace() {
            controlable = true;
        }

        void Update() {
            foreach (var wheel in wheels) {
                wheel.localUp = localUp;
            }
        }

        void FixedUpdate() {
            if (Physics.Raycast(transform.position, -transform.up, out var hit, normalRayLength, normalRayLayers)) {
                normal = hit.normal;
            }
            localUp = isAntigrav ? normal : Vector3.up;
            orientationUp = IsGrounded ? normal : localUp;
            Vector3 vel = car.RB.velocity;
            Vector3 localVel = transform.InverseTransformDirection(car.RB.velocity);
            isReversing = Vector3.Dot(transform.forward, car.RB.velocity.normalized) < 0 && car.RB.velocity.magnitude > reverseThreshold;
            isBraking = Vector3.Dot(vel.normalized, transform.forward * car.Input.AxisVert) < 0;

            currentForwardFriction = currentFrictionSettings.forwardFriction;
            currentSidewaysFriction = currentFrictionSettings.sidewaysFriction;
            UpdateCurrentGrip();

            // CorrectVelocityVector(currentForwardFriction);
            if (controlable) {
                PerformMovement(vel);
                Turn(localVel);
            }
            // car.RB.velocity += currSpeed * transform.forward;
            car.RB.AddForce(currSpeed * transform.forward, ForceMode.Acceleration);
            //car.RB.velocity -= car.RB.velocity * stats.idleDeceleration * currentFrictionSettings.groundResistance * Time.fixedDeltaTime;
            Vector3 horizVel = transform.TransformDirection(new(localVel.x, 0, localVel.z));
            if (car.Input.AxisVert == 0 || car.Drifting.isBoosting){
                car.RB.AddForce(-horizVel * stats.idleDeceleration * currentFrictionSettings.groundResistance, ForceMode.Acceleration);
            }

            ApplySidewaysFriction(currentSidewaysFriction, localVel, vel.normalized);

            // downforce
            car.RB.AddForce(-transform.up * downforceAmount * (vel.magnitude / stats.maxSpeed) * (isAntigrav ? 2.5f : 1));

            // gravity
            if (IsAffectedByGravity) car.RB.AddForce(-gravity * localUp, ForceMode.Acceleration);
            transform.position = car.RB.transform.position;
        }

        private void Turn(Vector3 localVel) {
            float turnAmount = Mathf.Clamp(localVel.z, -stats.maxSpeed, stats.maxSpeed);
            if (!IsGrounded) turnAmount = stats.maxSpeed;
            float turnAngle = stats.turnAngle * turnAmount * (car.Input.AxisHori + car.Drifting.DriftDirection);
            // car.RB.angularVelocity = localUp * turnAngle;
            transform.localRotation *= Quaternion.Euler(Vector3.up * turnAngle);

            //align the normals to the normal vector
            float angle = Vector3.Angle(orientationUp, transform.up);
            Vector3 perpendicular = Vector3.Cross(orientationUp, transform.up);
            transform.RotateAround(transform.position, perpendicular, -angle * 4 * Time.fixedDeltaTime);
        }

        private void PerformMovement(Vector3 vel) {
            if (car.Drifting.isBoosting) {
                Move(vel, boostingSpeed * BoostTierOperations.AsFloat(car.Drifting.BoostTier), boostingAcceleration);
            }
            else {
                if (isBraking) {
                    Brake(vel);
                }
                else {
                    if (car.Input.AxisVert > 0) Move(vel, stats.maxSpeed, stats.acceleration);
                    else if (car.Input.AxisVert < 0) Reverse(vel);
                    else currSpeed *= stats.idleDeceleration;
                }
            }
        }

        private void Reverse(Vector3 vel) {
            float t = vel.magnitude / stats.maxReverseSpeed;
            currSpeed = -Mathf.Lerp(stats.reverseAcceleration, 0, t * t) * Time.fixedDeltaTime;
        }

        private void Move(Vector3 vel, float maxSpeed, float accel) {
            float t = vel.magnitude / maxSpeed;
            currSpeed = Mathf.Lerp(accel, 0, t * t) * currentForwardFriction * Time.fixedDeltaTime;
        }

        private void Brake(Vector3 vel) {
            float brake = stats.brakeForce * ((vel.magnitude + stats.brakeForce / 2)/ stats.maxSpeed) * -car.Input.AxisVert;
            currSpeed -= brake * Time.fixedDeltaTime;
        }

        // private void UpdateWheelPosition(WheelCollider collider, Transform wheel) {
        //     collider.GetWorldPose(out var pos, out var rot);
        //     wheel.SetPositionAndRotation(pos, rot);
        // }
        
        // void CorrectVelocityVector(float grip) {
        //     Vector3 vel = transform.InverseTransformDirection(car.RB.velocity);
        //     float velHoriz = vel.z;
        //     float velVert = vel.y;
        //     float gripExp = Mathf.Pow(.005f, 1 - grip);
        //     Vector3 finalVector = Vector3.Lerp(car.RB.velocity, transform.forward * velHoriz + transform.up * velVert, gripExp);
        //     car.RB.velocity = finalVector;
        // }

        void ApplySidewaysFriction(float sideways, Vector3 localVel, Vector3 velDir) {
            float sidewaysFriction = localVel.x * sideways * 5f;
            car.RB.AddForce(sidewaysFriction * -transform.right * car.RB.mass, ForceMode.Force);
        }

        public SurfaceType GetSurface() {
            if (surfaceOverride is not null) return (SurfaceType)surfaceOverride;
            SurfaceType s = SurfaceType.Ice;
            foreach(var w in wheels)
            {
                if (w.surface > s) s = w.surface;
            }
            return s;
        }

        public void SetSurfaceOverride(SurfaceType? surface) => surfaceOverride = surface;
        public void SetAntigrav(bool antigrav) => this.isAntigrav = antigrav;
        public void SetControllableState(bool state) => controlable = state;

        public IEnumerator StopAllMotion(Vector3 pos, Quaternion rot, int iter = 2) {
            for (int i = 0; i < iter; i++) {
                car.RB.velocity = Vector3.zero;
                car.RB.angularVelocity = Vector3.zero;
                car.RB.transform.SetPositionAndRotation(pos, rot);
                yield return new WaitForFixedUpdate();
            }
        }

        void OnDrawGizmos() {
            Gizmos.color = new(.6f, 1, .6f);
            if (car is not null) {
                Gizmos.matrix.SetTRS(car.RB.transform.position, car.RB.transform.rotation, car.RB.transform.localScale);
                Gizmos.DrawWireSphere(car.RB.transform.position, 1);
                return;
            }
            Gizmos.DrawWireSphere(transform.position, 1);
        }
    }
}