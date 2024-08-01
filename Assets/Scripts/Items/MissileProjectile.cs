using System.Reflection.Emit;
using GWK.Kart;
using UnityEngine;
using UnityEngine.VFX;

public sealed class MissileProjectile : ItemProjectile, IItemInteractable {
    private BaseCar _parentCar;
    public BaseCar parentCar => _parentCar;
    [SerializeField] private LayerMask collideWith;
    [SerializeField] private Transform model;
    [SerializeField] private VisualEffect effect;
    [Space]
    [SerializeField] private float speed = 60;
    [SerializeField] private float rotateSpeed = 95;
    private Quaternion modelRot = Quaternion.identity;
    private float lifetime = 10;
    private ITargetable target;
    public void SetParentCar(BaseCar car) { 
        _parentCar = car;
        model.rotation = car.transform.rotation;
    }
    
    public void OnItemBox() {
        parentCar.Item.RollItem();
    }

    void OnCollisionEnter(Collision collision) {
        int layer = 1 << (collision.gameObject.layer);

        if (collision.gameObject.TryGetComponent<CarCollider>(out var carCollider)) {
            carCollider.Hit();
            carCollider.parentCar.ClearTarget();
            SelfDestruct();
            return;
        }

        if (collision.gameObject.TryGetComponent<ItemProjectile>(out var projectile)) {
            SelfDestruct();
            return;
        }
    }

    public void SetTarget(ITargetable target) {
        this.target = target;
        target?.MarkAsTarget(this);
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

    protected override void FixedUpdate() {
        if (target is null) {
            return;
        }
        Vector3 direction = (target.Position - transform.position).normalized * speed;
        RB.AddForce((direction - RB.linearVelocity) * 10, ForceMode.Acceleration);

        base.FixedUpdate();
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
        model.forward = RB.linearVelocity.normalized;
        lifetime -= Time.deltaTime;
        if (lifetime <= 0) {
            SelfDestruct();
        }
    }
}