using UnityEngine;
using PathCreation;

public class StartFinish : MonoBehaviour
{
    [SerializeField] private PathCreator[] lapPaths;

    public VertexPath GetPathAtLap(int lap) {
        return lapPaths[(lap - 1) % lapPaths.Length].path;
    }

    public VertexPath FirstPath => lapPaths[0].path;
}