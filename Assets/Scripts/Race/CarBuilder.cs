using UnityEngine;
using PathCreation;

public class CarBuilder {
    private GameObject carObj;
    private BaseCar car;
    private bool isBot;
    private bool startOnAntigrav;

    public CarBuilder(GameObject prefab, Transform startPos, string name) {
        carObj = GameObject.Instantiate(prefab);
        carObj.transform.SetPositionAndRotation(startPos.position, startPos.rotation);
        car = carObj.GetComponent<BaseCar>();
        carObj.name = name;
    }

    public CarBuilder SetStats(CarStats stats) {
        car.Movement.SetStats(stats);
        return this;
    }

    public CarBuilder IsBot(bool isBot) {
        this.isBot = isBot;
        return this;
    }

    public CarBuilder StartOnAntigrav(bool startOnAntigrav) {
        this.startOnAntigrav = startOnAntigrav;
        return this;
    }

    public CarBuilder SetPath(VertexPath path) {
        car.Path.SetPath(path);
        return this;
    }

    public CarBuilder SetNumberOfLaps(byte laps) {
        car.Path.numLaps = laps;
        return this;
    }

    public (BaseCar, GameObject) Build() {
        car.Init(isBot, startOnAntigrav);
        return (car, carObj);
    }
}