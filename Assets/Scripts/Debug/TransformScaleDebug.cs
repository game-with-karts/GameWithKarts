using UnityEngine;

public class TransformScaleDebug : MonoBehaviour {
    Vector3 startScale;
    void Start() {
        startScale = transform.lossyScale;
    }
    void Update() {
        startScale.x = Mathf.Sin(Time.unscaledTime);
        transform.localScale = startScale;
    }
}