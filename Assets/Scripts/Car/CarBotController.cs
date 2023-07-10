using PathCreation;
using UnityEngine;
using System;

using Rnd = UnityEngine.Random;
public class CarBotController : CarComponent
{
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
    private Vector3 ForwardRayOriginOffset => transform.forward * forwardRayOriginOffset + transform.up * .25f;
    private Vector3 BackwardRayOriginOffset => transform.forward * -forwardRayOriginOffset + transform.up * .25f;
    private Vector3 RightRayOriginOffset => ForwardRayOriginOffset + rightRayOriginOffset * transform.right;
    private Vector3 LeftRayOriginOffset => ForwardRayOriginOffset - rightRayOriginOffset * transform.right;
    private Vector3 RightRayDirection => (transform.forward * narrowness + transform.right).normalized;
    private Vector3 LeftRayDirection => (transform.forward * narrowness - transform.right).normalized;

    private bool forwardHit;
    private bool forwardRightHit;
    private bool forwardLeftHit;
    private bool rightHit;
    private bool leftHit;

    private bool isStuck = false;

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
        Ray forwardRight = new Ray(RightRayOriginOffset + transform.position, transform.forward);
        Ray forwardLeft = new Ray(LeftRayOriginOffset + transform.position, transform.forward);
        Ray right = new Ray(RightRayOriginOffset + transform.position, RightRayDirection);
        Ray left = new Ray(LeftRayOriginOffset + transform.position, LeftRayDirection);

        RaycastHit info;

        forwardRightHit = Physics.Raycast(forwardRight, out info, rayLength, checkLayers);
        forwardLeftHit = Physics.Raycast(forwardLeft, out info, rayLength, checkLayers);
        rightHit = Physics.Raycast(right, out info, rayLength, checkLayers);
        leftHit = Physics.Raycast(left, out info, rayLength, checkLayers);
        forwardHit = Physics.Raycast(forward, out info, rayLength, checkLayers);

        if (isStuck)
        {
            Ray backward = new Ray(BackwardRayOriginOffset + transform.position, -transform.forward);
            vert = -1;
            horiz = dir * -1;
            if (Physics.Raycast(backward, out info, rayLength / 3, checkLayers)) {
                vert = 1;
                horiz *= -1;
            }
            if (Vector3.Dot((car.Path.GetNextPoint() - transform.position).normalized, transform.forward) >= .85f) {
                isStuck = false;
            }
            car.Input.SetAxes(vert, horiz, 0f, 0f, 0f);
            return;
        }
        if (forwardHit) {
            horiz = transform.InverseTransformDirection(info.normal).x > 0 ? 1 : -1;
            if (info.distance < rayLength / 2) vert = 0;
            if (info.distance < rayLength / 3) isStuck = true;
        }
        if (forwardLeftHit) {
            horiz += .5f;
        }
        if (leftHit) {
            horiz += .5f;
        }
        if (forwardRightHit) {
            horiz -= .5f;
        }
        if (rightHit) {
            horiz -= .5f;
        }

        if (car.Movement.GetSurface() == SurfaceType.Ice)
            horiz = Mathf.Clamp(((Mathf.Abs(nextPoint.x) - angleThreshold / 3) * dir) + horiz, -1, 1);
        else
            horiz = Mathf.Clamp(((Mathf.Abs(nextPoint.x) - angleThreshold) * dir / turnSmoothing) + horiz, -1, 1);
        if (vert < 0) horiz *= -1;
        car.Input.SetAxes(vert, horiz, 0f, 0f, 0f);
    }

    void OnDrawGizmos() {
        if (car is null) return;
        Gizmos.color = new(1f, 1f, 0f);
        Gizmos.DrawLine(transform.position, car.Path.GetNextPoint());

        Gizmos.color = forwardHit ? Color.red : new(1f, .5f, 1f);
        Gizmos.DrawLine(transform.position + ForwardRayOriginOffset, transform.position + ForwardRayOriginOffset + transform.forward * rayLength);

        Gizmos.color = forwardRightHit ? Color.red : new(1f, .5f, 1f);
        Gizmos.DrawLine(transform.position + RightRayOriginOffset, transform.position + RightRayOriginOffset + transform.forward * rayLength);

        Gizmos.color = forwardLeftHit ? Color.red : new(1f, .5f, 1f);
        Gizmos.DrawLine(transform.position + LeftRayOriginOffset, transform.position + LeftRayOriginOffset + transform.forward * rayLength);

        Gizmos.color = rightHit ? Color.red : new(1f, .5f, 1f);
        Gizmos.DrawLine(transform.position + RightRayOriginOffset, transform.position + ForwardRayOriginOffset + RightRayDirection * rayLength);

        Gizmos.color = leftHit ? Color.red : new(1f, .5f, 1f);
        Gizmos.DrawLine(transform.position + LeftRayOriginOffset, transform.position + ForwardRayOriginOffset + LeftRayDirection * rayLength);
    }

    public override void Init() {
        car.Path.OnRaceEnd += (BaseCar _) => { this.enabled = true; };
        if (!car.IsBot) this.enabled = false;
    }
}