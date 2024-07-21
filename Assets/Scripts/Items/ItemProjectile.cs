using UnityEngine;
using GWK.Kart;
using UnityEngine.Rendering;

public abstract class ItemProjectile : MonoBehaviour, ISelfDestructable {
    [SerializeField] private Rigidbody rb;
    public Rigidbody RB => rb;
    public Vector3 localUp { protected get; set; }
    public bool isAntigrav { protected get; set; }

    void Awake() {
        RaceManager.allItems.Add(this);
    }

    protected virtual void FixedUpdate() {
        if (!isAntigrav) {
            localUp = Vector3.up;
        }
        else {
            if (Physics.Raycast(transform.position, -localUp, out var hitInfo, 20, ~0)) {
                Vector3 normal = hitInfo.normal;
                localUp = normal;
            }
        }
        rb.AddForce(-localUp * 20f, ForceMode.Acceleration);
    }

    public virtual void SelfDestruct(bool removeFromList) {
        if (removeFromList) {
            RaceManager.allItems.Remove(this);
        }
    }
}