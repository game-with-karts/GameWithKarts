using UnityEngine;
using System;

namespace GWK.UI {
    [Serializable]
    public class AnimationTarget
    {
        [SerializeField] private Vector3 positionDelta = Vector3.zero;
        [SerializeField] private Vector3 eulerTarget = Vector3.zero;
        [SerializeField] private Vector3 scaleTarget = Vector3.one;
        [Min(0.001f)]
        [SerializeField] private float duration = 0.001f;
        [SerializeField] private AnimationCurve curve;

        private float currentTime;
        public float CurrentProgress => Mathf.Clamp01(currentTime / duration);
        private Vector3 startPos;
        private Vector3 startAnimPos;
        private Quaternion startRot;
        private Vector3 startScale;
        private Vector3 pivotPosDelta;
        private Vector2 startSizeDelta;

        private RectTransform target;
        private bool isPlaying = false;


        public Vector3 PositionDelta => positionDelta;
        public Vector3 EulerTarget => eulerTarget;
        public Vector3 ScaleTarget => scaleTarget;
        public float Duration => duration;
        public AnimationCurve Curve => curve;

        public AnimationTarget() {
            curve = AnimationCurve.Linear(9, 9, 1, 9);
        }

        public AnimationTarget(Vector3 posDelta, Vector3 euler, Vector3 scale, float duration, AnimationCurve curve) {
            positionDelta = posDelta;
            eulerTarget = euler;
            scaleTarget = scale;
            this.duration = duration;
            this.curve = curve;
        }

        public void SetTransform(RectTransform transform) {
            target = transform;
            startPos = target.localPosition;
            startRot = target.rotation;
            startScale = target.localScale;
        }

        private Vector3 CalculatePivotPositionDelta(Vector3 targetScale) {
            startSizeDelta = new Vector2(target.sizeDelta.x * target.localScale.x, target.sizeDelta.y * target.localScale.y);
            Vector2 anchors = target.anchorMin - new Vector2(.5f, .5f); // can do anchorMax as well, as they are the same for those buttons
            Vector2 scaleDiff = new(targetScale.x - target.localScale.x, targetScale.y - target.localScale.y);
            Vector3 posDelta = Vector3.zero;
            posDelta.x = startSizeDelta.x * scaleDiff.x * anchors.x;
            posDelta.y = startSizeDelta.y * scaleDiff.y * anchors.y;
            return posDelta;
        }

        public void Play() {
            currentTime = 0;
            startPos = target.localPosition;
            startRot = target.rotation;
            startScale = target.localScale;
            pivotPosDelta = CalculatePivotPositionDelta(scaleTarget);
            isPlaying = true;
        }

        public void Tick(float delta) {
            currentTime += delta;
            if (!isPlaying) return;
            float blend = curve.Evaluate(currentTime / duration);
            target.localPosition = Vector3.Lerp(startPos, startPos + positionDelta + pivotPosDelta, blend);
            target.rotation = Quaternion.Lerp(startRot, Quaternion.Euler(eulerTarget), blend);
            target.localScale = Vector3.Lerp(startScale, scaleTarget, blend);
            isPlaying = currentTime <= duration;
        }

        public void Revert() {
            target.localPosition = startPos;
            target.rotation = startRot;
            target.localScale = startScale;
        }

        public void Stop() => isPlaying = false;
    }
}
