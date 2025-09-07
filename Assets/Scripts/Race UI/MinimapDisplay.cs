using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GWK.Kart;

public class MinimapDisplay : MonoBehaviour
{
    [SerializeField] private RectTransform carPointPrefab;
    [SerializeField] private RectTransform minimapParent;
    [SerializeField] private Image minimapImage;
    [Space]
    [SerializeField] private float playerDotSize = 2;
    private MinimapTransform minimapTransform;
    private List<(BaseCar, RectTransform)> cars = new();
    public void SetMinimapTransform(MinimapTransform t) {
        minimapTransform = t;
        minimapTransform.SetUISize((transform as RectTransform).sizeDelta.x);
    }
    public void SetMinimapImage(Sprite img) {
        minimapImage.sprite = img;
        if (GameRulesManager.instance.currentTrack.settings.mirrorMode) minimapImage.transform.localScale = new(-1, 1, 1);
    }
    public void AddCars(BaseCar[] cars) {
        RectTransform rt;
        foreach (var car in cars) {
            rt = Instantiate(carPointPrefab, minimapParent);
            if (!car.IsBot) rt.localScale = new(playerDotSize, playerDotSize, 1);
            rt.GetComponent<Image>().color = car.Appearance.CarColor;
            this.cars.Add((car, rt));
        }
        
    }
    private void Update() {
        Vector3 carPosOnMap;
        foreach((BaseCar car, RectTransform dot) in cars) {
            carPosOnMap = new(car.Position.x, car.Position.z, -1);
            dot.localPosition = (carPosOnMap - minimapTransform.Offset) / minimapTransform.MinimapScale;
        }
    }
}
