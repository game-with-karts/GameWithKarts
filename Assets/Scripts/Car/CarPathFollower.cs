using PathCreation;
using UnityEngine;
using System;
using System.Collections;

public class CarPathFollower : CarComponent
{
    [SerializeField] private PathCreator path;
    private VertexPath currentPath;
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

    private const float maxPathTimeDelta = 0.1f;

    public Action OnFinalLap;
    public Action<BaseCar> OnRaceEnd;

    public void SetPath(VertexPath path) {
        currentPath = path;
        CurrentPathPoint = 0;
    }

    public Vector3 GetNextPoint() {
        if (CurrentPathPoint + 1 >= currentPath.NumPoints) 
            return currentPath.GetPoint(currentPath.NumPoints - 1);
        return currentPath.GetPoint(CurrentPathPoint + 1);
    }

    public void NextLap() {
        CurrentLap++;
        CurrentPathNumber = 1;
        CurrentPathPoint = 0;
        CurrentPathTime = 0;
        if (CurrentLap == numLaps) OnFinalLap?.Invoke();
        else if (CurrentLap > numLaps) {
            OnRaceEnd?.Invoke(car);
        }
    }

    private void Update() {
        prevDistanceToNextPoint = DistanceToNextPoint;
        DistanceToNextPoint = (GetNextPoint() - transform.position).magnitude;

        float pathTime = currentPath.GetClosestTimeOnPath(transform.position);
        float pathTimeDelta = pathTime - CurrentPathTime;
        if (pathTimeDelta <= maxPathTimeDelta && pathTimeDelta > 0)
            CurrentPathTime = pathTime;

        if (DistanceToNextPoint - prevDistanceToNextPoint > 0) {
            Vector3 closestPoint = currentPath.GetClosestPointOnPath(transform.position);
            for (int i = 0; i < currentPath.NumPoints; i++) {
                if (currentPath.GetPoint(i) == closestPoint) {
                    CurrentPathPoint = i;
                    break;
                }
            }
        }
        if (DistanceToNextPoint < distanceToSwitch) {
            CurrentPathPoint++;
        }
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
        CurrentPathPoint = 0;
        CurrentPathTime = 0;
        finalPlacement = -1;
    }

    public override void StartRace()
    {
        StartCoroutine(nameof(UpdatePoint));
    }

    private IEnumerator UpdatePoint() {
        while (true) {
            prevDistanceToNextPoint = DistanceToNextPoint;
            DistanceToNextPoint = (GetNextPoint() - transform.position).magnitude;

            if (DistanceToNextPoint - prevDistanceToNextPoint > 0) {
                Vector3 closestPoint = currentPath.GetClosestPointOnPath(transform.position);
                for (int i = 0; i < currentPath.NumPoints; i++) {
                    if (currentPath.GetPoint(i) == closestPoint) {
                        CurrentPathPoint = i;
                        break;
                    }
                }
            }
            if (DistanceToNextPoint < distanceToSwitch) {
                CurrentPathPoint++;
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    private void OnDestroy() {
        StopCoroutine(nameof(UpdatePoint));
    }
}