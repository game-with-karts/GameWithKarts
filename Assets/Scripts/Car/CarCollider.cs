using UnityEngine;
using System;
using System.Linq;

namespace GWK.Kart {
    public class CarCollider : MonoBehaviour {
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
    }
}