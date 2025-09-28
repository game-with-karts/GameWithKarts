using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Data;
using GWK.Kart;
using UnityEngine;
using UnityEngine.VFX;
using PathCreation;

public sealed class FireballProjectile : ItemProjectile, IItemInteractable {
    private enum State {
        Chasing,
        Targeting,
    }
    private State state = State.Chasing;
    private BaseCar _parentCar;
    public BaseCar parentCar => _parentCar;
    private BaseCar targetCar => RaceManager.instance.CarsInPlacementOrder.First();
    private VertexPath path;
    [SerializeField] private VisualEffect explosion;
    [SerializeField] private float speed = 65;
    [SerializeField] private float pointDetectionThreshold = 15;
    [SerializeField] private AudioSource flightSfx;
    [SerializeField] private AudioSource explosionSfx;
    private int nextPointIdx;
    private Vector3 nextPoint;
    int currentLap;

    private IEnumerator selfDestructCoroutine = null;
    
    void Start() {
        flightSfx.Play();
        MinimapDisplay.AddFireball(transform);
    }
    public void OnItemBox() {
        parentCar.Item.RollItem();
    }
    
    public void SetParentCar(BaseCar car) {
        _parentCar = car;
        path = car.Path.CurrentPath;
        currentLap = car.Path.CurrentLap;
    }

    public override void PlaySound(bool paused) {
        if (paused) {
            flightSfx.Pause();
        }
        else {
            flightSfx.Play();
        }
    }

    public void SetNextPoint(int idx) {
        nextPointIdx = idx < 0 ? idx + path.NumPoints : idx % path.NumPoints;
        nextPoint = path.GetPoint(nextPointIdx);
    }

    public void FindNextPoint() {
        switch (state) {
            case State.Chasing:
                do {
                    nextPointIdx = (nextPointIdx + 1) % path.NumPoints;
                    nextPoint = path.GetPoint(nextPointIdx);
                                    
                } while (Vector3.Dot(transform.forward, nextPoint - transform.position) < 0);
                break;
            case State.Targeting:
                nextPoint = targetCar.Position;
                break;
            default:
                break;
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.TryGetComponent<CarCollider>(out var carCollider)) {
            carCollider.Hit();
            if (carCollider.car == targetCar) {
                SelfDestruct();
            }
            return;
        }

        if (collision.gameObject.TryGetComponent<ItemProjectile>(out var projectile)) {
            SelfDestruct();
            return;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Start Finish")) {
            path = other.gameObject.GetComponent<StartFinish>().GetPathAtLap(++currentLap);
            nextPointIdx = 0;
            FindNextPoint();
        }
    }

    public override void SelfDestruct() {
        base.SelfDestruct();
        MinimapDisplay.RemoveFireball(transform);
        explosion.transform.parent = null;
        explosion.enabled = true;
        explosion.Play();
        explosionSfx.Play();
        Destroy(explosion.gameObject, 3);
        Destroy(gameObject);
    }

    void Update() {
        Vector3 vec = nextPoint - transform.position;
        switch (state) {
            case State.Chasing:
                if (vec.magnitude <= pointDetectionThreshold || Vector3.Dot(transform.forward, vec) < 0) {
                    FindNextPoint();
                }
                if (IsCloseTarget()) {
                    state = State.Targeting;
                }
                break;
            case State.Targeting:
                FindNextPoint();
                break;
        }
        transform.forward = vec.normalized;
        RB.linearVelocity = transform.forward * speed;
        if (_parentCar == targetCar) {
            if (selfDestructCoroutine == null) {
                selfDestructCoroutine = SelfDestructIfTargetIsParent();
                StartCoroutine(selfDestructCoroutine);
            }
            return;
        }
    }

    private bool IsCloseTarget() {
        bool behindOnPath = targetCar.Path.CurrentPath == path
             && (targetCar.Path.CurrentPathPoint - nextPointIdx < 5 
             && targetCar.Path.CurrentPathPoint - nextPointIdx >= 0
             || targetCar.Path.CurrentPathPoint + path.NumPoints - nextPointIdx < 5);
        return (targetCar.Position - transform.position).magnitude < 50 && behindOnPath;
    }

    IEnumerator SelfDestructIfTargetIsParent() {
        yield return new WaitForSeconds(1f);
        SelfDestruct();
    }

#if UNITY_EDITOR
    void OnDrawGizmos() {
        Gizmos.color = new(0f, 1f, 0f);
        Gizmos.DrawLine(transform.position, nextPoint);
    }

#endif
}
