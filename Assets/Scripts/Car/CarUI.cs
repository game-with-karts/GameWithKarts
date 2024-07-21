using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using GWK.Util;

namespace GWK.Kart {
    public class CarUI : CarComponent {
        [SerializeField] private GameObject canvas;
        [Header("Boost Gauge")]
        [SerializeField] private Slider gauge;
        [SerializeField] private Image gaugeFill;
        [Space]
        [SerializeField] private Gradient boostGaugeGradient;
        [Space]
        [Header("Lap Counter")]
        [SerializeField] private TMP_Text lapCounter;
        [SerializeField] private TMP_Text maxLaps;
        [Space]
        [Header("Position Display")]
        [SerializeField] private TMP_Text positionDisplay;
        [Space]
        [Header("Time display")]
        [SerializeField] private TMP_Text timeDisplay;
        [Space]
        [Header("Minimap")]
        [SerializeField] private MinimapDisplay minimap;
        public MinimapDisplay Minimap => minimap;
        [Header("Last Lap display")]
        [SerializeField] private RectTransform lastLapTransform;
        [SerializeField] private TMP_Text lastLapText;
        [SerializeField] private TMP_Text lastLapDiff;
        [SerializeField] private TMP_ColorGradient gradientFaster;
        [SerializeField] private TMP_ColorGradient gradientEqual;
        [SerializeField] private TMP_ColorGradient gradientSlower;
        [SerializeField] private float lastLapDuration = 5;
        [SerializeField] private AnimationCurve lastLapCurve;
        [Header("Item")]
        [SerializeField] private Image itemImage;
        private Vector2 lastLapAnchoredPos;
        private int numCars;
        private bool lastLapEventSubscribed = false;
        private int bestLap = -1;
        void Update() {
            gauge.gameObject.SetActive(car.Drifting.IsDrifting && car.Drifting.CanDrift);
            gauge.value = car.Drifting.RelativeDriftTimer;
            gaugeFill.color = boostGaugeGradient.Evaluate(gauge.value);

            int numLap = Mathf.Clamp(car.Path.CurrentLap, 1, car.Path.numLaps);
            lapCounter.text = string.Format("{0:00}", numLap);
            maxLaps.text = string.Format("{0:00}", car.Path.numLaps);
            
            int place = car.Path.finalPlacement == -1 ? car.Path.currentPlacement : car.Path.finalPlacement;
            positionDisplay.text = StringsUtil.FormatPlace(place);

            timeDisplay.text = StringsUtil.GetFormattedTime(car.Timer.ElapsedTime);
        }

        public override void Init(bool restarting) {
            canvas.SetActive(false);
            transform.parent = null;

            lapCounter.text = "Lap -/-";
            positionDisplay.text = "-/-";

            car.Path.OnRaceEnd += RaceEnd;

            RaceSettings settings = ScriptableObject.CreateInstance<RaceSettings>();
            settings ??= GameRulesManager.currentTrack?.settings;

            positionDisplay.gameObject.SetActive(!settings.timeAttackMode);
            timeDisplay.gameObject.SetActive(settings.timeAttackMode);

            lastLapAnchoredPos = lastLapTransform.anchoredPosition;
            lastLapAnchoredPos.x = -lastLapTransform.rect.width;
            lastLapTransform.anchoredPosition = lastLapAnchoredPos;

            bestLap = -1;
            
            StopAllCoroutines();
            if (!lastLapEventSubscribed) {
                car.Timer.OnLapSaved += LastLapCoroutine;
                lastLapEventSubscribed = true;
            }
        }

        public void ActivateCanvas() => canvas.SetActive(!car.IsBot);

        private void RaceEnd(BaseCar _) {
            canvas.SetActive(false);
            car.Path.OnRaceEnd -= RaceEnd;
            car.Timer.OnLapSaved -= LastLapCoroutine;
            lastLapEventSubscribed = false;
        }

        private void LastLapCoroutine(int time) {
            StartCoroutine(ShowLastLap(time));
        }

        private IEnumerator ShowLastLap(int time) {
            float elapsed = 0;
            float t;
            lastLapText.text = StringsUtil.GetFormattedTime(time);
            
            Debug.Log($"time = {time}, bestLap = {bestLap}");
            lastLapDiff.text = "";
            if (bestLap != -1) {
                Debug.Log("Split time shown");
                int diff = Math.Abs(time - bestLap);
                string text = StringsUtil.GetFormattedTime(diff, false);
                if (diff == 0) {
                    lastLapDiff.colorGradientPreset = gradientEqual;
                }
                else if (time < bestLap) {
                    lastLapDiff.text = $"-{text}";
                    lastLapDiff.colorGradientPreset = gradientFaster;
                    bestLap = time;
                }
                else {
                    lastLapDiff.text = $"+{text}";
                    lastLapDiff.colorGradientPreset = gradientSlower;
                }
            }
            else {
                Debug.Log("First lap");
                bestLap = time;
            }

            while (elapsed < lastLapDuration) {
                elapsed += Time.deltaTime;
                t = elapsed / lastLapDuration;
                lastLapAnchoredPos.x = (lastLapCurve.Evaluate(t) - 1) * lastLapTransform.rect.width;
                lastLapTransform.anchoredPosition = lastLapAnchoredPos;
                yield return new WaitForEndOfFrame();
            }
            lastLapAnchoredPos.x = -lastLapTransform.rect.width;
            lastLapTransform.anchoredPosition = lastLapAnchoredPos;
        }

        public void SetNumberOfCars(int num) => numCars = num;

        public void SetItemImage(ItemEntry? entry) {
            if (entry is null) {
                itemImage.enabled = false;
                return;
            }
            itemImage.enabled = true;
            itemImage.sprite = entry?.image;
        }
    }
}