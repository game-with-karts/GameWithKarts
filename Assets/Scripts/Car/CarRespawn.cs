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

        IEnumerator respawnCoroutine;

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag(RESPAWN_TRIGGER_TAG)) {
                if (respawnCoroutine == null) {
                    respawnCoroutine = Respawn();
                    StartCoroutine(respawnCoroutine);
                }
            }
        }

        private IEnumerator Respawn() {
            car.Movement.SetControllableState(false);
            car.Camera.IsFollowingPlayer = false;

            Vector3 respawnPosition = car.Path.GetNextPoint();

            float respawnPathTime = car.Path.CurrentPath.GetClosestTimeOnPath(respawnPosition);
            Vector3 respawnForward = car.Path.CurrentPath.GetDirection(respawnPathTime);
            Vector3 respawnRight = car.Path.CurrentPath.GetNormal(respawnPathTime);
            Vector3 respawnUp = Vector3.Cross(respawnForward, respawnRight);
            if (GameRulesManager.instance.currentTrack.settings.mirrorMode) {
                respawnUp *= -1;
            }

            RaycastHit hit;
            if (Physics.Raycast(respawnPosition, -respawnUp, out hit, 20f, surfaceLayers)) {
                respawnUp = hit.normal;
            }
            respawnPosition = respawnVerticalPosition * respawnUp + hit.point;
            Quaternion respawnRotation = Quaternion.LookRotation(respawnForward, respawnUp);
            car.Drifting?.ResetBoostTank();
            yield return new WaitForSeconds(waitDuration);

            car.Camera.IsFollowingPlayer = true;
            car.Movement.IsAffectedByGravity = false;
            car.RB.isKinematic = true;
            car.RB.transform.position = respawnPosition;
            transform.rotation = respawnRotation;

            car.Item.DisableInvincibility();
            car.Item.DisableShield(false);

            float s = 0;
            while (s < respawnDuration) {
                s += Time.deltaTime;
                car.RB.transform.position -= respawnSpeed * Time.deltaTime * respawnUp;
                yield return new WaitForEndOfFrame();
            }
            car.RB.isKinematic = false;

            car.Movement.SetControllableState(true);
            car.Movement.IsAffectedByGravity = true;
            OnRespawn?.Invoke();
            respawnCoroutine = null;
        }
        public override void Init(bool _) {
            if (respawnCoroutine != null) {
                StopCoroutine(respawnCoroutine);
            }
            respawnCoroutine = null;
        }
        
        void Start() {
            car.Collider.TriggerEnter += OnTriggerEnter;
        }
    }
}
