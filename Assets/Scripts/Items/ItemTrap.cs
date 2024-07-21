using UnityEngine;
using System.Collections;
using GWK.Kart;

public class ItemTrap : MonoBehaviour, ISelfDestructable {
    [SerializeField] private ItemType type;
    public bool Active { get; private set; }
    private readonly float animLength = .5f;
    void Awake() {
        Active = false;
        transform.localScale = Vector3.zero;
        RaceManager.allItems.Add(this);
        StartCoroutine(Activate());
    }

    private IEnumerator Activate() {
        float t = 0;
        Quaternion originalRot = transform.rotation;
        while (t <= animLength) {
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t / animLength);
            transform.rotation = originalRot * Quaternion.Euler(Vector3.up * 1440 * (t - .5f) * (t - .5f));
        }
        transform.rotation = originalRot;
        Active = true;
    }

    void OnTriggerEnter(Collider other) {
        if (!Active) {
            return;
        }

        if (other.gameObject.TryGetComponent<ItemProjectile>(out _)) {
            SelfDestruct();
        }

        if (other.gameObject.TryGetComponent<CarCollider>(out var collider)) {
            collider.ItemTrapHit(type);
            SelfDestruct();
        }
    }

    public void SelfDestruct() {
        Destroy(gameObject);
    }

    void OnDestroy() {
        RaceManager.allItems.Remove(this);
    }
}