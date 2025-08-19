using UnityEngine;
using GWK.Kart;

public class BoostTankItem : IItem {
    public void Use(BaseCar parent, GameObject prefab) {
        parent.Drifting.AddBoost(50, BoostTier.Normal);
    }
}