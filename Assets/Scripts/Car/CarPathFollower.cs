using PathCreation;
using UnityEngine;
using System;
using System.Collections;

public class CarPathFollower : CarComponent
{
    private VertexPath currentPath;
    public VertexPath CurrentPath => currentPath;
    [Min(0.001f)]
    [SerializeField] private float distanceToSwitch;
    private float prevDistanceToNextPoint;
    public float DistanceToNextPoint { get; private set; }
    public int CurrentPathPoint { get; private set; }
    public float CurrentPathTime { get; private set; }
    public int CurrentPathNumber { get; private set; }
    public int CurrentLap { get; private set; }
    public int numLaps;
    public int finalPlacement { get; set; }
    public int currentPlacement { get; set; }

    private const float maxPathTimeDelta = 0.3f;

    public event Action OnFinalLap;
    public event Action<BaseCar> OnNextLap;
    public event Action<BaseCar> OnRaceEnd;

    private int timeCalcCurrentPoint; 
    private int timeCalcNextPoint => (timeCalcCurrentPoint + 1) % currentPath.NumPoints;
    private float timeCalcPrevDistanceToNext;
    private bool justChanged = false;

    public void SetPath(VertexPath path) {
        currentPath = path;
        CurrentPathPoint = 0;
        timeCalcCurrentPoint = 0;
    }

    public Vector3 GetNextPoint() {
    //     Vector3 normal = currentPath.GetNormal(CurrentPathPoint);
    //     if (isVectorNaN(normal)) normal = Vector3.zero;
    //     Vector3 offset = car.IsBot ? car.BotController.PathHorizontalDeviation * normal : Vector3.zero;
        if (CurrentPathPoint + 1 >= currentPath.NumPoints) 
            return currentPath.GetPoint(currentPath.NumPoints - 1);
        return currentPath.GetPoint(CurrentPathPoint + 1);
    }

    public Quaternion GetRotationOnPath() {
        return currentPath.GetRotationAtDistance(CurrentPathTime);
    }

    public void NextLap() {
        CurrentLap++;
        CurrentPathNumber = 1;
        CurrentPathPoint = 0;
        CurrentPathTime = 0;
        OnNextLap?.Invoke(car);
        if (CurrentLap == numLaps) OnFinalLap?.Invoke();
        else if (CurrentLap > numLaps) {
            OnRaceEnd?.Invoke(car);
        }
    }

    private void FixedUpdate() {
        if (!car.Movement.IsControlable) return;
        float timeCalcDistanceToNext = (currentPath.GetPoint(timeCalcNextPoint) - transform.position).magnitude;
        float delta = timeCalcDistanceToNext - timeCalcPrevDistanceToNext;
        Vector3 tangent = currentPath.GetTangent(timeCalcNextPoint);
        float dot = Vector3.Dot(car.RB.velocity.normalized, tangent);
        if (delta > 0.1f && dot > 0) {
            if (justChanged) {
                justChanged = false;
            }
            else {
                timeCalcCurrentPoint = (timeCalcCurrentPoint + 1) % currentPath.NumPoints;
                justChanged = true;
            }
        }
        timeCalcPrevDistanceToNext = timeCalcDistanceToNext;
        float pathTime = GetClosestTime();
        float pathTimeDelta = pathTime - CurrentPathTime;
        if (pathTimeDelta <= maxPathTimeDelta && pathTimeDelta > 0)
            CurrentPathTime = pathTime;
        
    }

    private void OnTriggerEnter(Collider other) {
        if (CurrentPathPoint < currentPath.NumPoints / 2) return;
        if (CurrentPathTime < .7f) return;
        if (other.gameObject.CompareTag(Constants.StartFinishTag)) {
            NextLap();
            currentPath = other.gameObject.GetComponent<StartFinish>().GetPathAtLap(CurrentLap);
        }
    }

    public override void Init() {
        CurrentLap = 1;
        CurrentPathNumber = 1;
        CurrentPathPoint = GetClosestIndex();
        CurrentPathTime = 0;
        finalPlacement = -1;
        justChanged = false;
        if (currentPath is not null) {
            timeCalcCurrentPoint = GetClosestIndex();
        }
    }

    private void Start() {
        timeCalcCurrentPoint = GetClosestIndex();
    }

    private int GetClosestIndex(int startFrom = 0, bool loopAround = false, bool ignoreMaxDeltaCheck = true) {
        float maxDistanceDelta = 10;
        Vector3 closestPoint = currentPath.GetPoint(startFrom);
        int i = loopAround ? 0 : startFrom;
        int currIdx = 0;
        int closestIdx = startFrom;
        float minDistance = (closestPoint - transform.position).magnitude;
        float currDistance;
        for (; i < currentPath.NumPoints; i++) {
            currIdx = loopAround ? (i + startFrom) % currentPath.NumPoints : i;
            currDistance = (transform.position - currentPath.GetPoint(currIdx)).magnitude;
            if (currDistance < minDistance){
                minDistance = currDistance;
                closestIdx = currIdx;
            }
            else if (!ignoreMaxDeltaCheck && currDistance > maxDistanceDelta) break;
        }
        return closestIdx;
    }

    private float GetClosestTime() {
        float timeBegin = currentPath.times[timeCalcCurrentPoint];
        float timeEnd = currentPath.times[timeCalcNextPoint];
        if (timeEnd == 0) timeEnd = 1;
        Vector3 ac = transform.position - currentPath.GetPoint(timeCalcCurrentPoint);
        Vector3 ab = currentPath.GetPoint(timeCalcNextPoint) - currentPath.GetPoint(timeCalcCurrentPoint);
        float t = Vector3.Dot(ac, ab) / ab.sqrMagnitude;
        return Mathf.Lerp(timeBegin, timeEnd, t);
    }

    public override void StartRace() {
        StartCoroutine(nameof(UpdatePoint));
    }

    private IEnumerator UpdatePoint() {
        while (true) {
            prevDistanceToNextPoint = DistanceToNextPoint;
            DistanceToNextPoint = (GetNextPoint() - transform.position).magnitude;
            float distanceDelta = DistanceToNextPoint - prevDistanceToNextPoint;
            if (DistanceToNextPoint < distanceToSwitch) {
                CurrentPathPoint++;
            }
            if (distanceDelta > 0.01f) {
                int closest = GetClosestIndex();
                for (; closest < CurrentPath.NumPoints && (currentPath.GetPoint(closest) - transform.position).magnitude < distanceToSwitch; closest++) {}
                CurrentPathPoint = Mathf.Clamp(closest, 0, CurrentPath.NumPoints - 1);
            }
            
            yield return new WaitForSeconds(.1f);
        }
    }

    private void OnDestroy() {
        StopCoroutine(nameof(UpdatePoint));
    }

    #if UNITY_EDITOR

    private void OnDrawGizmosSelected() {
        if (currentPath is null) return;
        Vector3 closestPoint = currentPath.GetPointAtTime(CurrentPathTime);
        Vector3 segmentStart = currentPath.GetPoint(timeCalcCurrentPoint);
        Vector3 segmentEnd = currentPath.GetPoint(timeCalcNextPoint);

        Gizmos.color = new(0.45f, 0.55f, 1f);
        Gizmos.DrawWireSphere(segmentStart, 3);
        Gizmos.DrawWireCube(segmentEnd, new(3, 3, 3));
        Gizmos.DrawLine(segmentStart, segmentEnd);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3);
        Gizmos.DrawSphere(closestPoint, 1);
        Gizmos.DrawLine(transform.position, closestPoint);
    }

    #endif
}