using UnityEngine;
using System;

using Rnd = UnityEngine.Random;

namespace GWK.Kart {

    public class CarBotController : CarComponent, IInputProvider {
        [Tooltip("An angle from the car's forward direction to the next point at which the car will start to turn")]
        [Min(0f)]
        [SerializeField] private float angleThreshold = 3;
        [Tooltip("An angle at which the kart will start drifting")]
        [Min(0f)]
        [SerializeField] private float driftThreshold = 5;
        [Tooltip("Larger number will cause the kart to turn more smoothly")]
        [Min(0.001f)]
        [SerializeField] private float turnSmoothing = 2;
        [Tooltip("Length of rays used for obstacle detection")]
        [Min(0)]
        [Space]
        [Header("Raycast settings")]
        [SerializeField] private float rayLength = 10;
        [SerializeField] private float iceRayLength = 20;
        [SerializeField] private LayerMask checkLayers;
        [SerializeField] private float narrowness = 4;
        [SerializeField] private float forwardRayOriginOffset;
        [SerializeField] private float rightRayOriginOffset;
        [SerializeField] private float maxPathHorizontalDeviation;
        [SerializeField] private float wallThreshold = .2f;
        [Space]
        [SerializeField] private CarWheelRaycaster[] frontWheels;
        [SerializeField] private CarWheelRaycaster[] rearWheels;
        private Vector3 ForwardRayOriginOffset => transform.forward * forwardRayOriginOffset + transform.up * .25f;
        private Vector3 BackwardRayOriginOffset => transform.forward * -forwardRayOriginOffset + transform.up * .25f;
        private Vector3 RightRayOriginOffset => ForwardRayOriginOffset + rightRayOriginOffset * transform.right;
        private Vector3 LeftRayOriginOffset => ForwardRayOriginOffset - rightRayOriginOffset * transform.right;
        private Vector3 RightRayDirection => (transform.forward * narrowness + transform.right).normalized;
        private Vector3 ForwardRightRayDirection => (transform.forward * narrowness + transform.right / 2).normalized;
        private Vector3 LeftRayDirection => (transform.forward * narrowness - transform.right).normalized;
        private Vector3 ForwardLeftRayDirection => (transform.forward * narrowness - transform.right / 2).normalized;

        private bool forwardHit;
        private bool forwardRightHit;
        private bool forwardLeftHit;
        private bool rightHit;
        private bool leftHit;

        public float RayLength {
            get {
                if (car.Movement.GetSurface() == SurfaceType.Ice) {
                    return iceRayLength;
                }
                return rayLength;
            }
        }

        private bool isFrontGrounded {
            get {
                bool result = false;
                foreach (var w in frontWheels) {
                    result |= w.isGrounded;
                }
                return result;
            }
        }

        private bool isRearGrounded {
            get {
                bool result = false;
                foreach (var w in rearWheels) {
                    result |= w.isGrounded;
                }
                return result;
            }
        }

        private float itemTimer = 0f;
        private float targetItemTimer = -1f;

        private event Action<float> horizontal;
        private event Action<float> vertical;
        private event Action<bool> jump1;
        private event Action<bool> jump2;
        private event Action item;
        private event Action<bool> backCam;

        event Action<float> IInputProvider.HorizontalPerformed {
            add => horizontal += value;
            remove => horizontal -= value;
        }

        event Action<float> IInputProvider.VerticalPerformed {
            add => vertical += value;
            remove => vertical -= value;
        }

        event Action<bool> IInputProvider.Jump1 {
            add => jump1 += value;
            remove => jump1 -= value;
        }

        event Action<bool> IInputProvider.Jump2 {
            add => jump2 += value;
            remove => jump2 -= value;
        }

        event Action IInputProvider.Item {
            add => item += value;
            remove => item -= value;
        }

        event Action<bool> IInputProvider.BackCamera {
            add => backCam += value;
            remove => backCam -= value;
        }

        private bool isVectorNaN(Vector3 v) => float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);

        private void FixedUpdate() {
            Vector3 pathPointNormal;
            try {
                pathPointNormal = car.Path.CurrentPath.GetNormal(car.Path.CurrentPathPoint);
            }
            catch (IndexOutOfRangeException) {
                pathPointNormal = Vector3.zero;
            }
            
            if (isVectorNaN(pathPointNormal)) pathPointNormal = Vector3.zero;
            Vector3 nextPoint = transform.InverseTransformPoint(car.Path.GetNextPoint() + pathPointNormal * Rnd.Range(0, maxPathHorizontalDeviation));
            float dir = Mathf.Sign(nextPoint.x);
            float vert = 1;
            float horiz = 0;

            Ray forward = new Ray(ForwardRayOriginOffset + transform.position, transform.forward);
            Ray forwardRight = new Ray(RightRayOriginOffset + transform.position, ForwardRightRayDirection);
            Ray forwardLeft = new Ray(LeftRayOriginOffset + transform.position, ForwardLeftRayDirection);
            Ray right = new Ray(RightRayOriginOffset + transform.position, RightRayDirection);
            Ray left = new Ray(LeftRayOriginOffset + transform.position, LeftRayDirection);

            RaycastHit infoForward;
            RaycastHit infoForwardLeft;
            RaycastHit infoForwardRight;
            RaycastHit infoLeft;
            RaycastHit infoRight;

            forwardRightHit = Physics.Raycast(forwardRight, out infoForwardRight, RayLength, checkLayers);
            forwardLeftHit = Physics.Raycast(forwardLeft, out infoForwardLeft, RayLength, checkLayers);
            rightHit = Physics.Raycast(right, out infoRight, RayLength, checkLayers);
            leftHit = Physics.Raycast(left, out infoLeft, RayLength, checkLayers);
            forwardHit = Physics.Raycast(forward, out infoForward, RayLength, checkLayers);

            if (forwardHit && !IsWall(infoForward.normal)) {
                horiz = (transform.InverseTransformDirection(infoForward.normal).x > 0 ? 1 : -1) * GetImportance(infoForward);
                if (infoForward.distance < RayLength / 2) vert = 0;
            }
            if (forwardLeftHit) {
                horiz += .5f * GetImportance(infoForwardLeft);
            }
            if (leftHit) {
                horiz += .5f * GetImportance(infoLeft);
            }
            if (forwardRightHit) {
                horiz -= .5f * GetImportance(infoForwardRight);
            }
            if (rightHit) {
                horiz -= .5f * GetImportance(infoRight);
            }

            // turn towards next point
            if (car.Movement.GetSurface() == SurfaceType.Ice)
                horiz = Mathf.Clamp(((Mathf.Abs(nextPoint.x) - angleThreshold * 3) * dir * 2) + horiz, -1, 1);
            else
                horiz = Mathf.Clamp(((Mathf.Abs(nextPoint.x) - angleThreshold) * dir / turnSmoothing) + horiz, -1, 1);
            if (vert < 0) horiz *= -1;

            // if front not grounded and rear grounded, jump
            bool jumpAxis = !isFrontGrounded && isRearGrounded;

            if (targetItemTimer != -1) {
                itemTimer += Time.fixedDeltaTime;
                if (itemTimer >= targetItemTimer) {
                    item?.Invoke();
                    targetItemTimer = -1;
                    itemTimer = 0;
                }
            }

            //car.Input.SetAxes(vert, horiz, jumpAxis, 0f, itemAxis);
            vertical?.Invoke(vert);
            horizontal?.Invoke(horiz);
            jump1?.Invoke(jumpAxis);
        }

        public void SetupItem(bool cancel = false) {
            if (cancel) {
                targetItemTimer = -1;
                itemTimer = 0;
                return;
            }
            targetItemTimer = Rnd.Range(0, 5);
        }

        void OnDrawGizmos() {
            if (car is null) return;
            Gizmos.color = new(1f, 1f, 0f);
            Gizmos.DrawLine(transform.position, car.Path.GetNextPoint());

            Gizmos.color = forwardHit ? Color.red : new(1f, .5f, 1f);
            Gizmos.DrawLine(transform.position + ForwardRayOriginOffset, transform.position + ForwardRayOriginOffset + transform.forward * RayLength);

            Gizmos.color = forwardRightHit ? Color.red : new(1f, .5f, 1f);
            Gizmos.DrawLine(transform.position + RightRayOriginOffset, transform.position + RightRayOriginOffset + ForwardRightRayDirection * RayLength);

            Gizmos.color = forwardLeftHit ? Color.red : new(1f, .5f, 1f);
            Gizmos.DrawLine(transform.position + LeftRayOriginOffset, transform.position + LeftRayOriginOffset + ForwardLeftRayDirection * RayLength);

            Gizmos.color = rightHit ? Color.red : new(1f, .5f, 1f);
            Gizmos.DrawLine(transform.position + RightRayOriginOffset, transform.position + ForwardRayOriginOffset + RightRayDirection * RayLength);

            Gizmos.color = leftHit ? Color.red : new(1f, .5f, 1f);
            Gizmos.DrawLine(transform.position + LeftRayOriginOffset, transform.position + ForwardRayOriginOffset + LeftRayDirection * RayLength);
        }

        private bool IsWall(Vector3 normal) {
            return Vector3.Dot(normal, transform.up) < wallThreshold;
        }
        public override void Init(bool restarting) {
            if (!restarting) {
                car.Path.OnRaceEnd += (BaseCar _) => { this.enabled = true; };
            }
            if (!car.IsBot) this.enabled = false;
        }

        public float GetImportance(RaycastHit hit) {
            return 1 - hit.distance / RayLength;
        }
    }
}