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
    private int numCars;

    void Update() {
        gauge.gameObject.SetActive(car.Drifting.IsDrifting && car.Drifting.CanDrift);
        gauge.value = car.Drifting.RelativeDriftTimer;
        gaugeFill.color = boostGaugeGradient.Evaluate(gauge.value);

        int numLap = Mathf.Clamp(car.Path.CurrentLap, 1, car.Path.numLaps);
        lapCounter.text = string.Format("{0:00}", numLap);
        maxLaps.text = string.Format("{0:00}", car.Path.numLaps);
        
        int place = car.Path.finalPlacement == -1 ? car.Path.currentPlacement : car.Path.finalPlacement;
        positionDisplay.text = FormatPlace(place);

        timeDisplay.text = CarLapTimer.GetFormattedTime(car.Timer.ElapsedTime);
    }

    public override void Init() {
        canvas.SetActive(false);
        transform.parent = null;
        lapCounter.text = "Lap -/-";
        positionDisplay.text = "-/-";
        car.Path.OnRaceEnd += RaceEnd;
        positionDisplay.gameObject.SetActive(!GameRulesManager.currentTrack.settings.timeAttackMode);
        timeDisplay.gameObject.SetActive(GameRulesManager.currentTrack.settings.timeAttackMode);
    }

    public void ActivateCanvas() => canvas.SetActive(!car.IsBot);

    private void RaceEnd(BaseCar _) {
        canvas.SetActive(false);
        car.Path.OnRaceEnd -= RaceEnd;
    }

    public static string FormatPlace(int place) {
        string suffix;
        if ((place / 10) % 10 != 1) {
            suffix = (place % 10) switch {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th"
            };
        }
        else suffix = "th";
        return $"{place}{suffix}";
    }

    public void SetNumberOfCars(int num) => numCars = num;
}