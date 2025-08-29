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
    [SerializeField] private AudioSource explosionSfx;
    [SerializeField] private AudioSource flightSfx;
    public void SetParentCar(BaseCar car) => _parentCar = car;
    private Quaternion modelRot = Quaternion.identity;
    private float lifetime = 10;
    private ContactPoint[] contacts = new ContactPoint[256];
    
    public void OnItemBox() {
        parentCar.Item.RollItem();
    }

    void Start() {
        flightSfx.Play();
    }

    public override void PlaySound(bool paused) {
        if (paused) {
            flightSfx.Pause();
        }
        else {
            flightSfx.Play();
        }
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
            Vector3 impulseDir = (cp.impulse - (Vector3.Dot(localUp, cp.impulse) * localUp)).normalized;
            postBounceVelocity += impulseDir * cp.impulse.magnitude;
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
        explosionSfx.Play();
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
