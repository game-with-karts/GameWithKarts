using UnityEngine;
using GWK.Kart;

public class ShieldItem : IItem {
    public void Use(BaseCar parent, GameObject prefab) {
        parent.Item.EnableShield();
    }
}
