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

    protected override void Awake()
    {
        base.Awake();
        transform.parent = null;
    }

    void Update()
    {
        gauge.gameObject.SetActive(car.Drifting.IsDrifting && car.Drifting.CanDrift);
        gauge.value = car.Drifting.RelativeDriftTimer;
        gaugeFill.color = gauge.value > .5f ? Color.red : Color.green;
    }
}