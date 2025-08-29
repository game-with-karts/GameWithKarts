using UnityEngine;
using GWK.Kart;
using System.Linq;
public class FireballItem : IItem {
    public void Use(BaseCar parent, GameObject prefab) {
        GameObject obj = GameObject.Instantiate(prefab, parent.Item.ItemSpawnpoint.position + parent.transform.forward * 4f + parent.transform.up * 3f, parent.transform.rotation);
        FireballProjectile fireball = obj.GetComponent<FireballProjectile>();
        fireball.SetParentCar(parent);
        fireball.SetNextPoint(parent.Path.CurrentPathPoint);
    }
}
