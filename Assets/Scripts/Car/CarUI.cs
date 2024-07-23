using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using GWK.Util;

namespace GWK.Kart {
    public class CarUI : CarComponent {
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasScaler canvasScaler;
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
        [Header("Target Display")]
        [SerializeField] private RectTransform targetDisplayTransform;
        [Header("Missile Approaching")]
        [SerializeField] private TMP_Text missileText;
        private Vector2 lastLapAnchoredPos;
        private int numCars;
        private bool lastLapEventSubscribed = false;
        private int bestLap = -1;

        private readonly System.Random rnd = new();
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

            DisplayItem();
            DisplayTarget();
            DisplayMissileApproaching();
        }
        
        private void DisplayItem() {
            if (car.Item == null) {
                return;
            }
            itemImage.enabled = car.Item.CurrentItem != null || car.Item.IsRolling;
            if (car.Item.IsRolling) {
                IEnumerable<Sprite> sprites = car.Item.GetItemSprites();
                itemImage.sprite = sprites.ElementAt(rnd.Next(sprites.Count()));
                return;
            }
            itemImage.sprite = car.Item.CurrentItem?.image;
        }

        private void DisplayTarget() {
            if (car.Item == null) {
                targetDisplayTransform.gameObject.SetActive(false);
                return;
            }
            bool visible = !(car.Item.target == null || car.Item.CurrentItem == null);
            targetDisplayTransform.gameObject.SetActive(visible);
            if (visible) {
                DisplayTargetAt(car.Item.target.Position);
            }
        }

        private void DisplayMissileApproaching() {
            missileText.enabled = car.currentProjectile != null;
            if (car.currentProjectile == null) {
                return;
            }

            missileText.text = $"{(transform.position - car.currentProjectile.transform.position).magnitude:f2} m";
            missileText.color = Color.Lerp(Color.red, Color.white, Mathf.Sin(6 * Mathf.PI * Time.time) / 2 + .5f);
        }

        public override void Init(bool restarting) {
            canvas.gameObject.SetActive(false);
            transform.parent = null;

            lapCounter.text = "Lap -/-";
            positionDisplay.text = "-/-";

            car.Path.OnRaceEnd += RaceEnd;

            RaceSettings settings = GameRulesManager.currentTrack?.settings;

            if (settings == null) {
                settings = ScriptableObject.CreateInstance<RaceSettings>();
            }

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

        public void ActivateCanvas() => canvas.gameObject.SetActive(!car.IsBot);

        private void RaceEnd(BaseCar _) {
            canvas.gameObject.SetActive(false);
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
            lastLapDiff.text = "";

            if (bestLap != -1) {
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

        public void DisplayTargetAt(Vector3 worldPos) {
            Vector2 screenPos = car.Camera.FrontFacingCamera.WorldToViewportPoint(worldPos);
            screenPos.x *= (canvas.transform as RectTransform).sizeDelta.x;
            screenPos.y *= canvasScaler.referenceResolution.y;
            targetDisplayTransform.anchoredPosition = screenPos;
        }
    }
}