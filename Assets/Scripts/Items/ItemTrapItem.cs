using GWK.Kart;
using UnityEngine;

public class ItemTrapItem : IItem {
    public void Use(BaseCar parent, GameObject prefab) {
        GameObject obj = GameObject.Instantiate(prefab, parent.Item.ItemSpawnpoint);
        obj.transform.rotation = parent.transform.rotation;
        obj.transform.parent = null;
    }
}