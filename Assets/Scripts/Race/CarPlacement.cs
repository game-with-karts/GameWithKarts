using UnityEngine;
using System.Linq;
using System;
using System.Collections;
public class CarPlacement : MonoBehaviour
{
    private BaseCar[] cars;
    public Action<int> OnFinalPlacement;

    private IEnumerator CalculatePlacements() {
        if (cars is null) yield return null;
        while (true) {
            cars = cars.OrderBy(x => x.Path.DistanceToNextPoint)
                   .OrderByDescending(x => x.Path.CurrentPathPoint)
                   .OrderByDescending(x => x.Path.CurrentPathTime)
                   .OrderByDescending(x => x.Path.CurrentPathNumber)
                   .OrderByDescending(x => x.Path.CurrentLap)
                   .OrderBy(x => x.Path.finalPlacement)
                   .OrderByDescending(x => x.Path.finalPlacement != -1)
                   .OrderByDescending(x => !x.isEleminated)
                   .ToArray();
            for (int i = 0; i < cars.Length; i++) {
                cars[i].Path.currentPlacement = i + 1;
            }
            yield return new WaitForSecondsRealtime(.1f);
        }
    }

    public void Init(BaseCar[] cars) {
        BaseCar[] newCars = new BaseCar[cars.Length];
        for (int i = 0; i < newCars.Length; i++) {
            newCars[i] = cars[i];
            newCars[i].UI.SetNumberOfCars(newCars.Length);
            newCars[i].Path.OnRaceEnd += SendFinalPlacement;
        }
        this.cars = newCars;
        StartCoroutine(nameof(CalculatePlacements));
    }

    private void SendFinalPlacement(BaseCar car) {
        int place = Array.IndexOf(cars, car) + 1;
        car.Path.finalPlacement = place;
        car.Path.OnRaceEnd -= SendFinalPlacement;
        if(car.playerControlled) OnFinalPlacement?.Invoke(place);
    }
}