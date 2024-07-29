using UnityEngine;
using System;
using System.Collections;

namespace GWK.Kart {
    public class CarRespawn : CarComponent {
        public Action OnRespawn;
        const string RESPAWN_TRIGGER_TAG = "Respawn Trigger";
        [SerializeField] private float waitDuration = 2f;
        [SerializeField] private float respawnVerticalPosition = 5f;
        [SerializeField] private float respawnDuration = 1f;
        [SerializeField] private float respawnSpeed = 2f;
        [SerializeField] private LayerMask surfaceLayers;

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag(RESPAWN_TRIGGER_TAG)) {
                StartCoroutine(nameof(Respawn));
            }
        }

        private IEnumerator Respawn() {
            car.Movement.SetControllableState(false);
            car.Camera.IsFollowingPlayer = false;

            Vector3 respawnPosition = car.Path.GetNextPoint();
            Quaternion respawnRotation = Quaternion.LookRotation(car.Path.GetDirectionToNextPoint(), car.Movement.LocalUp);
            Vector3 respawnUpDirection = car.Movement.LocalUp;

            RaycastHit hit;
            Physics.Raycast(respawnPosition, -respawnUpDirection, out hit, 20f, surfaceLayers);
            respawnPosition = respawnVerticalPosition * respawnUpDirection + hit.point;
            car.Drifting?.ResetBoostTank();
            yield return new WaitForSeconds(waitDuration);

            car.Camera.IsFollowingPlayer = true;
            car.Movement.IsAffectedByGravity = false;
            car.RB.isKinematic = true;
            car.RB.transform.position = respawnPosition;
            transform.rotation = respawnRotation;

            float s = 0;
            while (s < respawnDuration) {
                s += Time.deltaTime;
                car.RB.transform.position -= respawnSpeed * Time.deltaTime * car.Movement.LocalUp;
                yield return new WaitForEndOfFrame();
            }
            car.RB.isKinematic = false;

            car.Movement.SetControllableState(true);
            car.Movement.IsAffectedByGravity = true;
        }
        public override void Init(bool _) {
            StopCoroutine(nameof(Respawn));
        }
        
        void Start() {
            car.Collider.TriggerEnter += OnTriggerEnter;
        }
    }
}