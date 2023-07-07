using UnityEngine;
using PathCreation;

public class StartFinish : MonoBehaviour
{
    [SerializeField] private PathCreator[] lapPaths;
    [SerializeField] private Transform[] startPositions;
    public Transform[] StartPositions => startPositions;

    public VertexPath GetPathAtLap(int lap) {
        return lapPaths[(lap - 1) % lapPaths.Length].path;
    }

    public VertexPath FirstPath => lapPaths[0].path;

    void OnDrawGizmos()
    {
        Gizmos.color = new(0f, .3f, 1);
        foreach(Transform pos in startPositions) {
            Gizmos.DrawWireCube(pos.position, new(2, 0, 2));
            Gizmos.DrawWireCube(pos.position, new(1.5f, 0, 1.5f));
        }
    }
}