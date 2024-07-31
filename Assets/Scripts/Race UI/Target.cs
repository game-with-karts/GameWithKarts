using UnityEngine;

public class Target : MonoBehaviour {
    void Update() {
        transform.eulerAngles += Vector3.forward * 180 * Time.deltaTime;
    }
}