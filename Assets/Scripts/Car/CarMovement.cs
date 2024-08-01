using UnityEngine;
using System;
using System.Collections;

namespace GWK.Kart {
    public class CarMovement : CarComponent {
        [SerializeField] private CarStats stats;
        [SerializeField] private float reverseThreshold;
        [SerializeField] private float downforceAmount;
        [SerializeField] private float boostingSpeed = 40;
        [SerializeField] private float boostingAcceleration = 40;
        [SerializeField] private CarWheelRaycaster[] wheels;
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
            surfaceOverride = null;
        }

        public override void StartRace() {
            controlable = true;
        }

        void Update() {
            foreach (var wheel in wheels) {
                wheel.localUp = localUp;
            }
        }

        float axisH;
        float axisV;

        private void SetVertical(float v) => axisV = v;
        private void SetHorizontal(float h) => axisH = h;

        protected override void SubscribeProviderEvents() {
            InputProvider.VerticalPerformed += SetVertical;
            InputProvider.HorizontalPerformed += SetHorizontal;
        }

        protected override void UnsubscribeProviderEvents() {
            InputProvider.VerticalPerformed -= SetVertical;
            InputProvider.HorizontalPerformed -= SetHorizontal;
        }

        void FixedUpdate() {
            if (Physics.Raycast(transform.position, -transform.up, out var hit, normalRayLength, normalRayLayers)) {
                normal = hit.normal;
            }
            localUp = isAntigrav ? normal : Vector3.up;
            orientationUp = IsGrounded ? normal : localUp;
            Vector3 vel = car.RB.linearVelocity;
            Vector3 localVel = transform.InverseTransformDirection(car.RB.linearVelocity);

            currentForwardFriction = currentFrictionSettings.forwardFriction;
            currentSidewaysFriction = currentFrictionSettings.sidewaysFriction;
            UpdateCurrentGrip();

            switch(car.state) {
                case CarDrivingState.Idle:
                    MovementIdle(vel, localVel, axisV, axisH);
                    break;
                case CarDrivingState.Hit:
                case CarDrivingState.Spinning:
                    MovementIdle(vel, localVel, 0, 0);
                    break;
            }
            car.RB.AddForce(currSpeed * transform.forward, ForceMode.Acceleration);
            Vector3 horizVel = transform.TransformDirection(new(localVel.x, 0, localVel.z));
            car.RB.AddForce(-horizVel * currentFrictionSettings.groundResistance * stats.idleDeceleration, ForceMode.Acceleration);

            ApplySidewaysFriction(currentSidewaysFriction, localVel, vel.normalized);

            // downforce
            car.RB.AddForce(-transform.up * downforceAmount * (vel.magnitude / stats.maxSpeed) * (isAntigrav ? 2.5f : 1));

            // gravity
            if (IsAffectedByGravity) car.RB.AddForce(-gravity * localUp, ForceMode.Acceleration);
            transform.position = car.RB.transform.position;
        }

        void MovementIdle(Vector3 vel, Vector3 localVel, float axisV, float axisH) {
            isReversing = Vector3.Dot(transform.forward, car.RB.linearVelocity.normalized) < 0 && car.RB.linearVelocity.magnitude > reverseThreshold;
            isBraking = Vector3.Dot(vel.normalized, transform.forward * axisV) < 0;

            if (controlable) {
                PerformMovement(vel, axisV);
                Turn(localVel, axisH);
            }
        }

        private void Turn(Vector3 localVel, float axisH) {
            float turnAmount = Mathf.Clamp(localVel.z, -stats.maxSpeed, stats.maxSpeed);
            if (!IsGrounded) turnAmount = stats.maxSpeed;
            float turnAngle = stats.turnAngle * turnAmount * (axisH + car.Drifting.DriftDirection);
            transform.localRotation *= Quaternion.Euler(Vector3.up * turnAngle);

            //align the normals to the normal vector
            float angle = Vector3.Angle(orientationUp, transform.up);
            Vector3 perpendicular = Vector3.Cross(orientationUp, transform.up);
            transform.RotateAround(transform.position, perpendicular, -angle * 4 * Time.fixedDeltaTime);
        }

        private void PerformMovement(Vector3 vel, float axisV) {
            if (car.Drifting.isBoosting) {
                Move(vel, boostingSpeed * BoostTierOperations.AsFloat(car.Drifting.BoostTier), boostingAcceleration);
            }
            else {
                if (isBraking) {
                    Brake(vel);
                }
                else {
                    if (axisV > 0) Move(vel, stats.maxSpeed, stats.acceleration);
                    else if (axisV < 0) Reverse(vel);
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
            float brake = stats.brakeForce * ((vel.magnitude + stats.brakeForce / 2)/ stats.maxSpeed) * -axisV;
            currSpeed -= brake * Time.fixedDeltaTime;
        }

        void ApplySidewaysFriction(float sideways, Vector3 localVel, Vector3 velDir) {
            float sidewaysFriction = localVel.x * sideways * 5f;
            car.RB.AddForce(sidewaysFriction * -transform.right * car.RB.mass, ForceMode.Force);
        }

        public SurfaceType GetSurface() {
            if (surfaceOverride is not null) return (SurfaceType)surfaceOverride;
            SurfaceType s = SurfaceType.Ice;
            foreach(var w in wheels) {
                if (w.surface > s) s = w.surface;
            }
            return s;
        }

        public void SetSurfaceOverride(SurfaceType? surface) => surfaceOverride = surface;
        public void SetAntigrav(bool antigrav) => isAntigrav = antigrav;
        public void SetControllableState(bool state) => controlable = state;

        public IEnumerator StopAllMotion(Vector3 pos, Quaternion rot, int iter = 2) {
            for (int i = 0; i < iter; i++) {
                car.RB.linearVelocity = Vector3.zero;
                car.RB.angularVelocity = Vector3.zero;
                car.RB.transform.SetPositionAndRotation(pos, rot);
                yield return new WaitForFixedUpdate();
            }
        }
    }
}