using UnityEngine;

public class TransformDebug : MonoBehaviour {
    [SerializeField] private Transform trackModel;
    void Start() {
        Debug.LogAssertion($"Current lossy scale is {transform.localScale}, track model scale is {trackModel.lossyScale}");
    }
}