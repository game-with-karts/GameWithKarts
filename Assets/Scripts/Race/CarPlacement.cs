using UnityEngine;
using System.Linq;
using System;
public class CarPlacement : MonoBehaviour
{
    private CarPathFollower[] cars;
    void Update() {
        if (cars is null) return;
        cars = cars.OrderBy(x => x.DistanceToNextPoint)
                   .OrderByDescending(x => x.CurrentPathPoint)
                   .OrderByDescending(x => x.CurrentPathNumber)
                   .OrderByDescending(x => x.CurrentLap)
                   .ToArray();
        for (int i = 0; i < cars.Length; i++) {
            cars[i].currentPlacement = i + 1;
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
    }

    private void SendFinalPlacement(CarPathFollower car) {
        car.finalPlacement = Array.IndexOf(cars, car) + 1;
        car.OnRaceEnd -= SendFinalPlacement;
    }
}