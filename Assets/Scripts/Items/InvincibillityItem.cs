using UnityEngine;
using GWK.Kart;

public class InvincibilityItem : IItem {
    public void Use(BaseCar parent, GameObject prefab) {
        parent.Item.EnableInvincibility();
    }
}
