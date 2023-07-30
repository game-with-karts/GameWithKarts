using UnityEngine;

public class MinimapTransform : MonoBehaviour
{
    [SerializeField] private float size;
    private float uiSize;
    public void SetUISize(float size) {
        uiSize = size;
    }

    public float MinimapScale => size / uiSize;
    public Vector3 Offset => new(transform.position.x, transform.position.z, 0);

    #if UNITY_EDITOR
    private void OnDrawGizmos() {
        Gizmos.color = new(1, 0, 0, 0.5f);
        Gizmos.DrawCube(transform.position, new(size, 0, size));

        Gizmos.color = new(1, .5f, .5f, 0.5f);
        Gizmos.DrawWireCube(transform.position, new(size, 0, size));
    }
    #endif
}