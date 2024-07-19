using UnityEngine;
using PathCreation;
using GWK.Kart;

public class CarBuilder {
    private BaseCar car;
    private bool isBot;
    private bool startOnAntigrav;

    public CarBuilder(BaseCar prefab, Transform startPos, string name) {
        car = GameObject.Instantiate(prefab);
        car.transform.SetPositionAndRotation(startPos.position, startPos.rotation);
        car.gameObject.name = name;
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

    public BaseCar Build() {
        car.Init(isBot, startOnAntigrav);
        return car;
    }
}