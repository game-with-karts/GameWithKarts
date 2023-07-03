using PathCreation;
using UnityEngine;

public class CarPathFollower : CarComponent
{
    [SerializeField] private PathCreator path;
    private VertexPath currentPath;
    [Min(0.001f)]
    [SerializeField] private float distanceToSwitch;
    private float prevDistanceToNextPoint;
    public float DistanceToNextPoint { get; private set; }
    public int CurrentPathPoint { get; private set; }
    public int CurrentPathNumber { get; private set; }
    public int CurrentLap { get; private set; }


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
    }

    private void Update() {
        prevDistanceToNextPoint = DistanceToNextPoint;
        DistanceToNextPoint = (transform.position - GetNextPoint()).magnitude;
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
        if (other.gameObject.CompareTag(Constants.StartFinishTag)) {
            //TODO: do stuff
        }
    }

    public override void Init()
    {
        currentPath = path.path;
    }
}