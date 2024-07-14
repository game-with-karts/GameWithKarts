using UnityEngine;
using PathCreation;
using System.Collections.Generic;
using UnityEngine.InputSystem.UI;

public class LightProbeGenerator : MonoBehaviour {
    [SerializeField] private PathCreator path;
    [SerializeField] private int numberOfProbes;
    [SerializeField] private float trackWidth = 40;
    [SerializeField] private float height = 20;
    [SerializeField] private float upOffset = -5;
    [SerializeField] private LightProbeGroup probeGroup;
    [SerializeField] private bool invertNormals = false;
    [SerializeField] private bool overwriteProbes = true;

    public void Generate() {
        float pathTimeDelta = 1f / numberOfProbes;
        float currentTime = 0;
        List<Vector3> positions = new();
        if (!overwriteProbes) {
            positions = new(probeGroup.probePositions);
        }

        VertexPath vertexPath = path.path;
        float pathLength = vertexPath.length;

        Vector3 normal;

        Vector3 closest;
        Vector3 next;
        Vector3 forward;

        Vector3 up;

        for (int i = 0; i < numberOfProbes; i++) {
            normal = vertexPath.GetNormalAtDistance(currentTime * pathLength);
            closest = vertexPath.GetPointAtTime(currentTime);
            next = vertexPath.GetPointAtTime(Frac(currentTime + pathTimeDelta));
            forward = (next - closest).normalized;
            up = Vector3.Cross(normal, forward);
            if (invertNormals) {
                up *= -1;
            }

            positions.Add(closest + normal * (trackWidth / 2));
            positions.Add(closest);
            positions.Add(closest - normal * (trackWidth / 2));
            positions.Add(closest - normal * (trackWidth / 2) + height * up + upOffset * up);
            positions.Add(closest + height * up + upOffset * up);
            positions.Add(closest + normal * (trackWidth / 2) + height * up + upOffset * up);
            currentTime += pathTimeDelta;
        }

        probeGroup.probePositions = positions.ToArray();
    }

    private float Frac(float x) => x - (int)x;
}