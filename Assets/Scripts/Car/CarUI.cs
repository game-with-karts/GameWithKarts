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
    [Space]
    [Header("Position Display")]
    [SerializeField] private TMP_Text positionDisplay;
    private int numCars;

    void Update() {
        gauge.gameObject.SetActive(car.Drifting.IsDrifting && car.Drifting.CanDrift);
        gauge.value = car.Drifting.RelativeDriftTimer;
        gaugeFill.color = gauge.value > .5f ? Color.red : Color.green;

        int numLap = Mathf.Clamp(car.Path.CurrentLap, 1, car.Path.numLaps);
        lapCounter.text = $"Lap {numLap}/{car.Path.numLaps}";
        
        int place = car.Path.finalPlacement == -1 ? car.Path.currentPlacement : car.Path.finalPlacement;
        positionDisplay.text = $"{place}/{numCars}";
    }

    public override void Init() {
        canvas.SetActive(!car.IsBot);
        transform.parent = null;
        lapCounter.text = "Lap -/-";
        positionDisplay.text = "-/-";
        car.Path.OnRaceEnd += RaceEnd;
    }

    private void RaceEnd(BaseCar _) {
        canvas.SetActive(false);
        car.Path.OnRaceEnd -= RaceEnd;
    }

    public void SetNumberOfCars(int num) => numCars = num;
}