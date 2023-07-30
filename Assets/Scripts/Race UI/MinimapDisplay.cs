using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MinimapDisplay : MonoBehaviour
{
    [SerializeField] private RectTransform carPointPrefab;
    [SerializeField] private RectTransform minimapParent;
    [SerializeField] private Image minimapImage;
    [Space]
    [SerializeField] private float playerDotSize = 2;
    private MinimapTransform minimapTransform;
    private List<(Transform, RectTransform)> cars = new();
    public void SetMinimapTransform(MinimapTransform t) {
        minimapTransform = t;
        minimapTransform.SetUISize((transform as RectTransform).sizeDelta.x);
    }
    public void SetMinimapImage(Sprite img) {
        minimapImage.sprite = img;
        if (GameRulesManager.currentTrack.settings.mirrorMode) minimapImage.transform.localScale = new(-1, 1, 1);
    }
    public void AddCars(BaseCar[] cars) {
        RectTransform rt;
        foreach (var car in cars) {
            rt = Instantiate(carPointPrefab, minimapParent);
            if (!car.IsBot) rt.localScale = new(playerDotSize, playerDotSize, 1);
            this.cars.Add((car.transform, rt));
        }
        
    }
    private void Update() {
        Vector3 carPosOnMap;
        foreach((Transform car, RectTransform dot) in cars) {
            carPosOnMap = new(car.position.x, car.position.z, -1);
            dot.localPosition = (carPosOnMap - minimapTransform.Offset) / minimapTransform.MinimapScale;
        }
    }
}