using UnityEngine;
using GWK.Kart;

public class Explosion : MonoBehaviour {
    [SerializeField] private new MeshRenderer renderer;
    [SerializeField] private new SphereCollider collider;
    [Space]
    [SerializeField] private float timePower = 10;
    [SerializeField] private float targetRadius = 10;
    [SerializeField] private float effectDuration = 2;
    [SerializeField] private float triggerActiveDuration = .5f;

    private float time = 0;

    void Awake() {
        transform.localScale = 2 * targetRadius * Vector3.one;
    }

    void Update() {
        time += Time.deltaTime;
        collider.enabled = time <= triggerActiveDuration;
        if (time > effectDuration) {
            Destroy(gameObject);
        }

        float t = time / effectDuration;
        collider.radius = (1 - Mathf.Pow(1 - t, timePower)) / 2;
        renderer.material.SetFloat("_Animation_Time", t);
        renderer.material.SetFloat("_Size_Power", timePower);
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.TryGetComponent<CarCollider>(out var carCollider)) {
            carCollider.ExplosionHit();
        }

        if (other.gameObject.TryGetComponent<ISelfDestructable>(out var item)) {
            item.SelfDestruct();
        }
    }
}