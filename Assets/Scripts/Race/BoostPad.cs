using UnityEngine;
using GWK.Kart;

public class BoostPad : MonoBehaviour {
    [SerializeField] private Transform decalTransform;

    public BoostTier boostTier = BoostTier.Normal;

    void Awake() {
        if (GameRulesManager.currentTrack.settings.mirrorMode) {
            decalTransform.localScale = new(-1f, 1, 1);
        }
    }
}