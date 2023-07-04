using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarUI : CarComponent 
{
    [SerializeField] private GameObject canvas;
    [Header("Boost Gauge")]
    [SerializeField] private Slider gauge;
    [SerializeField] private Image gaugeFill;
    [Space]
    [Header("Lap Counter")]
    [SerializeField] private TMP_Text lapCounter;

    void Update() {
        gauge.gameObject.SetActive(car.Drifting.IsDrifting && car.Drifting.CanDrift);
        gauge.value = car.Drifting.RelativeDriftTimer;
        gaugeFill.color = gauge.value > .5f ? Color.red : Color.green;
        int numLap = Mathf.Clamp(car.Path.CurrentLap, 1, car.Path.numLaps);
        lapCounter.text = $"Lap {numLap}/{car.Path.numLaps}";
    }

    public override void Init() {
        canvas.SetActive(!car.IsBot);
        transform.parent = null;
        lapCounter.text = "Lap -/-";
        car.Path.OnRaceEnd += RaceEnd;
    }

    private void RaceEnd() {
        canvas.SetActive(false);
        car.Path.OnRaceEnd -= RaceEnd;
    }
}