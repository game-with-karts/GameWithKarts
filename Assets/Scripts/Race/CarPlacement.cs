using UnityEngine;
using System.Linq;
using System;
using System.Collections;
public class CarPlacement : MonoBehaviour
{
    private CarPathFollower[] cars;

    private IEnumerator CalculatePlacements() {
        if (cars is null) yield return null;
        while (true) {
            cars = cars.OrderBy(x => x.DistanceToNextPoint)
                   .OrderByDescending(x => x.CurrentPathPoint)
                   .OrderByDescending(x => x.CurrentPathTime)
                   .OrderByDescending(x => x.CurrentPathNumber)
                   .OrderByDescending(x => x.CurrentLap)
                   .ToArray();
            for (int i = 0; i < cars.Length; i++) {
                cars[i].currentPlacement = i + 1;
            }
            yield return new WaitForSecondsRealtime(.1f);
        }
    }

    public void Init(BaseCar[] cars) {
        int numCars = 0;
        for (int i = 0; i < cars.Length; i++) {
            if (cars[i] is null) break;
            numCars++;
        }
        CarPathFollower[] carPaths = new CarPathFollower[numCars];
        for (int i = 0; i < numCars; i++) {
            if (cars[i] is null) break;
            cars[i].UI.SetNumberOfCars(numCars);
            carPaths[i] = cars[i].Path;
            carPaths[i].OnRaceEnd += SendFinalPlacement;
        }
        this.cars = carPaths;
        StartCoroutine(nameof(CalculatePlacements));
    }

    private void SendFinalPlacement(CarPathFollower car) {
        car.finalPlacement = Array.IndexOf(cars, car) + 1;
        car.OnRaceEnd -= SendFinalPlacement;
    }
}