using UnityEngine;
using GWK.Kart;

public class CarQuickStarter : MonoBehaviour {
    public BaseCar car;

    void Awake() {
        car.Init(false, false);
        car.Camera.ActivateCamera();
        car.UI.ActivateCanvas();
        car.StartRace();
    }
}