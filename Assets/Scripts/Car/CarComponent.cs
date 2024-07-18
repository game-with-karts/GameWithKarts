using UnityEngine;

namespace GWK.Kart {
    public abstract class CarComponent : MonoBehaviour
    {
        protected BaseCar car;

        protected virtual void Awake() {
            car = GetComponent<BaseCar>();
        }

        public abstract void Init(bool restarting);
        public virtual void StartRace() {}
    }
}