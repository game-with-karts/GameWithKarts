using System;
using UnityEngine;

namespace GWK.Kart {

    public enum DriftState {
        Idle,
        Jumping,
        Drifting,
        DriftingAfterBoost
    }

    public class CarDrifting : CarComponent {
        [Tooltip("Boost depletion rate in units/sec")]
        [SerializeField] private float tankDepletionRate;
        [SerializeField] private float jumpStrength;
        [SerializeField] private float jumpBoostTime;
        [SerializeField] private float jumpBoostAmount;
        [SerializeField] private float driftMinTime;
        [SerializeField] private float driftMinAmount;
        [SerializeField] private float driftMaxTime;
        [SerializeField] private float driftMaxAmount;

        public event Action OnJump;
        public event Action OnLand;
        public event Action<float, int> OnDriftBoost;

        private float tank = 0;
        public float BoostTank => tank;

        private DriftState state = DriftState.Idle;
        private BoostTier tier = BoostTier.None;
        private Timer jumpTimer = new();
        private Timer driftTimer = new();

        private int driftKey = 0;
        private float driftDirection;
        private int driftBoostCount = 0;
        public float DriftDirection => driftDirection;
        public bool isBoosting => tank > 0;
        public bool isTankEmpty => tank <= 0;
        public float RelativeDriftTimer => driftTimer.Time / driftMaxTime;
        public bool CanDrift => driftBoostCount < 3;
        public bool IsDrifting => state == DriftState.Drifting;
        public BoostTier BoostTier => tier;
        [SerializeField] private float jumpVerticalVelocityThreshold;

        private bool hasLeftGround;
        private Vector3 localVel {
            get {
                return transform.InverseTransformDirection(car.RB.velocity);
            }
        }

        private float axisV;
        private float axisH;

        private bool jump1;
        private bool jump2;
        private void SetAxisV(float a) => axisV = a;
        private void SetAxisH(float a) => axisH = a;

        private Action<bool> jump1Delegate;
        private Action<bool> jump2Delegate;
        protected override void SubscribeProviderEvents() {
            jump1Delegate = v => HandleJumpBtn(v, 1);
            jump2Delegate = v => HandleJumpBtn(v, 2);

            InputProvider.VerticalPerformed += SetAxisV;
            InputProvider.HorizontalPerformed += SetAxisH;
            InputProvider.Jump1 += jump1Delegate;
            InputProvider.Jump2 += jump2Delegate;
        }

        protected override void UnsubscribeProviderEvents() {
            InputProvider.VerticalPerformed -= SetAxisV;
            InputProvider.HorizontalPerformed -= SetAxisH;
            InputProvider.Jump1 -= jump1Delegate;
            InputProvider.Jump2 -= jump2Delegate;
        }

        private void HandleJumpBtn(bool pressed, int button) {
            jump1 = button == 1 ? pressed : jump1;
            jump2 = button == 2 ? pressed : jump2;
            switch(state) {
                default:
                    break;

                case DriftState.Idle:
                    if (pressed && car.Movement.IsGrounded && car.Movement.IsControlable) {
                        Jump(1.05f);
                    }
                    break;

                case DriftState.Drifting:
                    if (button == driftKey) {
                        if (!pressed) {
                            driftTimer.Stop();
                            driftTimer.Reset();
                            state = DriftState.Idle;
                        }
                        break;
                    }
                    CheckDriftCondition(pressed);
                    break;
            }
        }

        void Update() {
            jumpTimer.Tick(Time.deltaTime);
            driftTimer.Tick(Time.deltaTime);
            tank -= tankDepletionRate * Time.deltaTime;
            if (tank < 0 || (axisV < 0 && car.Movement.IsGrounded)) {
                tier = BoostTier.Normal;
                tank = 0;
            }

            if (car.state != CarDrivingState.Idle) {
                ResetBoostTank();
                state = DriftState.Idle;
                return;
            }
            
            switch(state) {
                default:
                    break;

                case DriftState.Idle:
                    driftDirection = 0;
                    break;

                case DriftState.Jumping:
                    if (car.Movement.IsGrounded && localVel.y < jumpVerticalVelocityThreshold && hasLeftGround) {
                        jumpTimer.Stop();
                        OnLand?.Invoke();
                        if (jumpTimer.Time >= jumpBoostTime) {
                            AddBoost(jumpBoostAmount, BoostTier.Normal);
                        }
                        jumpTimer.Reset();
                        state = DriftState.Idle;
                        if ((jump1 || jump2) && axisH != 0 && car.RB.velocity.magnitude > 5 && !car.Movement.IsReversing) {
                            state = DriftState.Drifting;
                            driftBoostCount = 0;
                            driftKey = jump1 ? 1 : (jump2 ? 2 : 0);
                            driftDirection = Mathf.Sign(axisH);
                            driftTimer.Start();
                        }
                    }
                    else if (!car.Movement.IsGrounded) {
                        hasLeftGround = true;
                    }
                    else if (localVel.y < jumpVerticalVelocityThreshold && Time.timeScale > 0) {
                        // TODO: something with this shite
                        Jump(0.4f);
                    }
                    break;
            }
        }

        private void Jump(float boostValue) {
            OnJump?.Invoke();
            state = DriftState.Jumping;
            hasLeftGround = false;
            float dot = Vector3.Dot(transform.up, car.Movement.LocalUp);
            float jumpBoost = boostValue + (1 - Mathf.Abs(dot)) / 2;
            car.RB.AddForce(car.Movement.LocalUp * jumpStrength * jumpBoost * car.RB.mass);
            jumpTimer.Start();
        }

        private void CheckDriftCondition(bool secondary) {
            if (driftTimer.Time > driftMaxTime) {
                driftBoostCount = 3;
            }
            if (secondary && driftBoostCount < 3) {
                driftBoostCount++;

                float boostT = (driftMaxTime - driftTimer.Time) / (driftMaxTime - driftMinTime);
                float boostAmount = Mathf.LerpUnclamped(driftMaxAmount, driftMinAmount, boostT);

                OnDriftBoost?.Invoke(boostT, driftBoostCount);

                BoostTier tier = this.tier == BoostTier.None ? BoostTier.Normal : this.tier;

                if (driftBoostCount == 3 && RelativeDriftTimer >= .9f) {
                    tier = BoostTierOperations.OneUp(tier);
                }
                AddBoost(boostAmount, tier);

                driftTimer.Reset();
            }
        }

        public void AddBoost(float boostAmount, BoostTier tier, bool overrideTier = false) {
            if (tier > this.tier || overrideTier) {
                this.tier = tier;
            }
            tank += boostAmount;
        }
        public void ResetBoostTank() {
            tank = 0;
            tier = BoostTier.None;
        }

        public override void Init(bool _) {
            state = DriftState.Idle;
            ResetBoostTank();
        }

        protected override void Awake() {
            base.Awake();
            car.Collider.TriggerEnter += OnTriggerEnter;
            car.Collider.TriggerStay += OnTriggerStay;
        }

        void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Boost") && car.Movement.IsGrounded) {
                BoostTier boostPadTier = other.gameObject.GetComponent<BoostPad>().boostTier;
                AddBoost(20, boostPadTier);
            }
        }

        void OnTriggerStay(Collider other) {
            if (other.gameObject.CompareTag("Boost") && tank < 20 && car.Movement.IsGrounded) {
                tank = 20;
            }
        }

        
    }
}