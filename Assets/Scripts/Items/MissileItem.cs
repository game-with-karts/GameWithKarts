using GWK.Kart;
using UnityEngine;

public class MissileItem : IItem {
    public void Use(BaseCar parent, GameObject prefab) {
        GameObject obj = GameObject.Instantiate(prefab, parent.transform.position + parent.transform.forward * 4f, Quaternion.identity);
        MissileProjectile projectile = obj.GetComponent<MissileProjectile>();
        projectile.RB.velocity = 80 * parent.transform.forward;
        projectile.isAntigrav = parent.Movement.IsAntigrav;
        projectile.localUp = parent.Movement.LocalUp;
        projectile.SetParentCar(parent);
        projectile.SetTarget(parent.Item.target);
    }
}