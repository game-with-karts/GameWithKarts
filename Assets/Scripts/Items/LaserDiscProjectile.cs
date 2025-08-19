using System.Collections.Generic;
using System.Data;
using GWK.Kart;
using UnityEngine;
using UnityEngine.VFX;

public sealed class LaserDiscProjectile : ItemProjectile, IItemInteractable {
    private BaseCar _parentCar;
    public BaseCar parentCar => _parentCar;
    [SerializeField] private Transform model;
    [SerializeField] private VisualEffect effect;
    public void SetParentCar(BaseCar car) => _parentCar = car;
    private Quaternion modelRot = Quaternion.identity;
    private float lifetime = 10;
    private ContactPoint[] contacts = new ContactPoint[256];
    
    public void OnItemBox() {
        parentCar.Item.RollItem();
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.TryGetComponent<CarCollider>(out var carCollider)) {
            carCollider.Hit();
            SelfDestruct();
            return;
        }

        if (collision.gameObject.TryGetComponent<ItemProjectile>(out var projectile)) {
            SelfDestruct();
            return;
        }
        collision.GetContacts(contacts);
        Vector3 postBounceVelocity = RB.linearVelocity;
        for (int i = 0; i < collision.contactCount; i++) {
            ContactPoint cp = contacts[i];
            postBounceVelocity += cp.impulse * 0.8f;
        }
        RB.linearVelocity = postBounceVelocity;
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Respawn Trigger")) {
            SelfDestruct();
            return;
        }

        if (other.gameObject.CompareTag("Item")) {
            SelfDestruct();
            return;
        }
    }

    public override void SelfDestruct() {
        base.SelfDestruct();
        effect.transform.parent = null;
        effect.transform.rotation = Quaternion.identity;
        effect.Play();
        Destroy(effect.gameObject, 3);
        Destroy(gameObject);
    }

    void Update() {
        modelRot *= Quaternion.Euler(0, 720 * Time.deltaTime, 0);
        model.rotation = modelRot * Quaternion.Euler(70, 0, 0);
        lifetime -= Time.deltaTime;
        if (lifetime <= 0) {
            SelfDestruct();
        }
    }
}
