using UnityEngine;
using System;
using System.Collections;
using Unity.VisualScripting;

namespace GWK.Kart {
    public class CarCollider : CarComponent, IItemInteractable {
        public event Action<Collider> TriggerEnter;
        public event Action<Collider> TriggerStay;
        public event Action<Collider> TriggerExit;

        public event Action<Collision> CollisionEnter;
        public event Action<Collision> CollisionStay;
        public event Action<Collision> CollisionExit;

        void OnTriggerEnter(Collider other) => TriggerEnter?.Invoke(other);
        void OnTriggerStay(Collider other) => TriggerStay?.Invoke(other);
        void OnTriggerExit(Collider other) => TriggerExit?.Invoke(other);

        void OnCollisionEnter(Collision c) => CollisionEnter?.Invoke(c);
        void OnCollisionStay(Collision c) => CollisionStay?.Invoke(c);
        void OnCollisionExit(Collision c) => CollisionExit?.Invoke(c);

        void OnDestroy() {
            ClearEvent(TriggerEnter);
            ClearEvent(TriggerExit);
            ClearEvent(TriggerStay);
            ClearEvent(CollisionEnter);
            ClearEvent(CollisionExit);
            ClearEvent(CollisionStay);
        }

        void ClearEvent<T>(Action<T> e) {
            if (e is null) {
                return;
            }
            Delegate[] delegates = e.GetInvocationList();
            foreach (Delegate d in delegates) {
                e -= (Action<T>)d;
            }
        }

        public void SetBaseCar(BaseCar car) => this.car = car;

        public BaseCar parentCar => car;

        public void OnItemBox() {
            car.Item.RollItem();
        }

        public override void Init(bool restarting) {}

        public void Hit() {
            if (car.state == CarDrivingState.Hit) {
                return;
            }
            car.state = CarDrivingState.Hit;
            car.Appearance.PlayHitAnimation();
            StartCoroutine(HitCoroutine());
        }

        private IEnumerator HitCoroutine() {
            yield return new WaitForSeconds(CarAppearance.HIT_ANIMATION_LENGTH);
            car.state = CarDrivingState.Idle;
        }

        private IEnumerator SpinCoroutine() {
            yield return new WaitForSeconds(CarAppearance.SPIN_ANIMATION_LENGTH);
            car.state = CarDrivingState.Idle;
        }

        public void ItemTrapHit(ItemType type) {
            switch (type) {
                case ItemType.SpikeTrap:
                    if (car.state != CarDrivingState.Idle) {
                        break;
                    }
                    car.state = CarDrivingState.Spinning;
                    car.Appearance.PlaySpinAnimation();
                    StartCoroutine(SpinCoroutine());
                    break;
                default:
                    break;
            }
        }
    }
}