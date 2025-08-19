using UnityEngine;
using System;

[Serializable]
public class CameraSequence
{
    [SerializeField] private Transform start;
    [SerializeField] private Transform end;
    [SerializeField] private float duration = 1.6f;
    [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    public Action OnSequenceEnd;
    public float time { get; private set; }
    public void Init() => time = 0;

    public (Vector3, Quaternion) Evaluate(float delta) {
        time += delta;
        float t = curve.Evaluate(time / duration);
        Vector3 pos = Vector3.Lerp(start.position, end.position, t);
        Quaternion rot = Quaternion.Lerp(start.rotation, end.rotation, t);
        if (time >= duration) StopSequence();
        return (pos, rot);
    }

    public void StopSequence() {
        OnSequenceEnd?.Invoke();
    }

#if UNITY_EDITOR
    [SerializeField] private Color gizmoColor;
    [SerializeField] private float gizmoSize = 5f;
    public void DrawGizmo() {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(end.position, Vector3.one * gizmoSize);
        Gizmos.DrawWireSphere(start.position, gizmoSize);
        Gizmos.DrawLine(start.position, end.position);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(start.position, start.position + start.right * gizmoSize * 2f);
        Gizmos.DrawLine(end.position, end.position + end.right * gizmoSize * 2f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(start.position, start.position + start.up * gizmoSize * 2f);
        Gizmos.DrawLine(end.position, end.position + end.up * gizmoSize * 2f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(start.position, start.position + start.forward * gizmoSize * 2f);
        Gizmos.DrawLine(end.position, end.position + end.forward * gizmoSize * 2f);
    }
#endif

}