using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using GWK.Kart;

public class MinimapDisplay : MonoBehaviour
{
    [SerializeField] private RectTransform carPointPrefab;
    [SerializeField] private RectTransform fireballPrefab;
    [SerializeField] private RectTransform minimapParent;
    [SerializeField] private Image minimapImage;
    [Space]
    [SerializeField] private float playerDotSize = 2;
    private MinimapTransform minimapTransform;
    private List<(BaseCar, RectTransform)> cars = new();

    private static List<(Transform, RectTransform)> fireballs = new();

    public static void AddFireball(Transform fireball) => OnAddFireball?.Invoke(fireball);
    public static void RemoveFireball(Transform fireball) => OnRemoveFireball?.Invoke(fireball);

    private static Action<Transform> OnAddFireball;
    private static Action<Transform> OnRemoveFireball;

    void Start() {
        OnAddFireball += AddFireballInstance;
        OnRemoveFireball += RemoveFireballInstance;
    }

    void OnDestroy() {
        OnAddFireball -= AddFireballInstance;
        OnRemoveFireball -= RemoveFireballInstance;
        fireballs.Clear();
    }

    private void AddFireballInstance(Transform fireball) {
        RectTransform rt = Instantiate(fireballPrefab, minimapParent);
        rt.localScale = new(2, 2, 1);
        fireballs.Add((fireball, rt));
    }

    private void RemoveFireballInstance(Transform fireball) {
        (Transform, RectTransform) obj;
        try {
            obj = fireballs.Where(x => x.Item1 == fireball).First();
        } catch (InvalidOperationException) {
            return;
        }
        fireballs.Remove(obj);
        Destroy(obj.Item2.gameObject);
    }

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
        foreach((Transform fb, RectTransform rt) in fireballs) {
            carPosOnMap = new(fb.position.x, fb.position.z, -1);
            rt.localPosition = (carPosOnMap - minimapTransform.Offset) / minimapTransform.MinimapScale;
        }
    }
}
