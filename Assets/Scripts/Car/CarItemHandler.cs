using UnityEngine;

namespace GWK.Kart {
    public class CarItemHandler : CarComponent {
        private bool eventSubscribed = false;
        public override void Init(bool restarting) {
            if (!eventSubscribed) {
                car.Collider.TriggerEnter += OnTriggerEnter;
                eventSubscribed = true;
            }
        }

        void OnTriggerEnter(Collider other) {
            if (!other.gameObject.CompareTag("Item Box")) {
                return;
            }
            // testing
            if (car.IsBot) {
                return;
            }
            Debug.Log("Item collected!");
        }
    }
}