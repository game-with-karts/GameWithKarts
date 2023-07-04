using PathCreation;
using UnityEngine;

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

    private void Update() {
        Vector3 nextPoint = transform.InverseTransformPoint(car.Path.GetNextPoint());
        float dir = Mathf.Sign(nextPoint.x);
        // float horiz = Mathf.Abs(nextPoint.x) > angleThreshold ? dir : 0f;
        float horiz = Mathf.Clamp01((Mathf.Abs(nextPoint.x) - angleThreshold) / turnSmoothing) * dir;
        car.Input.SetAxes(1f, horiz, 0f, 0f, 0f);
    }

    void OnDrawGizmos()
    {
        if (car is null) return;
        Gizmos.color = new(1f, 1f, 0f);
        Gizmos.DrawLine(transform.position, car.Path.GetNextPoint());
    }

    public override void Init()
    {
        car.Path.OnRaceEnd += delegate (CarPathFollower _) { this.enabled = true; };
        if (!car.IsBot) this.enabled = false;
    }
}