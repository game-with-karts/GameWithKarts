using UnityEngine;

namespace GWK.Kart {
    public abstract class CarComponent : MonoBehaviour {
        private IInputProvider inputProvider;
        public IInputProvider InputProvider {
            get => inputProvider;
            set {
                if (inputProvider is not null) {
                    UnsubscribeProviderEvents();
                }
                inputProvider = value;
                if (inputProvider is not null) {
                    SubscribeProviderEvents();
                }
            }
        }

        protected BaseCar car;

        protected virtual void Awake() {
            car = GetComponent<BaseCar>();
            inputProvider = null;
        }

        public abstract void Init(bool restarting);
        public virtual void StartRace() {}

        protected virtual void UnsubscribeProviderEvents() {}
        protected virtual void SubscribeProviderEvents() {}
    }
}