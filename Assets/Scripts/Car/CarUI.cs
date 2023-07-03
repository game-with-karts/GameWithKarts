using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarUI : CarComponent 
{
    [SerializeField] private GameObject canvas;
    [Header("Boost Gauge")]
    [SerializeField] private Slider gauge;
    [SerializeField] private Image gaugeFill;

    void Update()
    {
        gauge.gameObject.SetActive(car.Drifting.IsDrifting && car.Drifting.CanDrift);
        gauge.value = car.Drifting.RelativeDriftTimer;
        gaugeFill.color = gauge.value > .5f ? Color.red : Color.green;
    }

    public override void Init()
    {
        canvas.SetActive(!car.isBot);
        transform.parent = null;
    }
}