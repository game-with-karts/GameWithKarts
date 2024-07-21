using System.Collections.Generic;
using System.Data;
using GWK.Kart;
using UnityEngine;
using UnityEngine.VFX;

public sealed class LaserDiscProjectile : ItemProjectile, IItemInteractable {
    private BaseCar _parentCar;
    public BaseCar parentCar => _parentCar;
    [SerializeField] private LayerMask collideWith;
    [SerializeField] private Transform model;
    [SerializeField] private VisualEffect effect;
    public void SetParentCar(BaseCar car) => _parentCar = car;
    private Quaternion modelRot = Quaternion.identity;
    private float lifetime = 10;
    
    public void OnItemBox() {
        parentCar.Item.RollItem();
    }

    void OnCollisionEnter(Collision collision) {
        int layer = 1 << (collision.gameObject.layer);

        if (collision.gameObject.TryGetComponent<CarCollider>(out var carCollider)) {
            carCollider.Hit();
            SelfDestruct();
            return;
        }

        if (collision.gameObject.TryGetComponent<ItemProjectile>(out var projectile)) {
            SelfDestruct();
            return;
        }

        // if ((layer & collideWith.value) > 0) {
        //     ContactPoint[] points = new ContactPoint[collision.contactCount];
        //     collision.GetContacts(points);
        //     for (int i = 0; i < collision.contactCount; i++) {
        //         ContactPoint point = points[i];
        //         if (Vector3.Dot(point.normal, localUp) < -0.1f) {
        //             SelfDestruct();
        //             return;
        //         }
        //     }
        // }
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
        model.rotation = modelRot;
        model.localRotation *= Quaternion.Euler(70, 0, 0);
        lifetime -= Time.deltaTime;
        if (lifetime <= 0) {
            SelfDestruct();
        }
    }
}