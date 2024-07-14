using UnityEngine;

public class BoostPad : MonoBehaviour {
    [SerializeField] private Transform decalTransform;

    void Awake() {
        if (GameRulesManager.currentTrack.settings.mirrorMode) {
            decalTransform.localScale = new(-1f, 1, 1);
        }
    }
}