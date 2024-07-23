using GWK.Kart;
using UnityEngine;

public class LaserDiscItem : IItem {
    public void Use(BaseCar parent, GameObject prefab) {
        float mul = parent.Item.LookingBackwards ? -1 : 1;
        GameObject obj = GameObject.Instantiate(prefab, parent.transform.position + parent.transform.forward * 4f * mul, Quaternion.identity);
        LaserDiscProjectile projectile = obj.GetComponent<LaserDiscProjectile>();
        projectile.RB.velocity = 80 * parent.transform.forward * mul;
        projectile.isAntigrav = parent.Movement.IsAntigrav;
        projectile.localUp = parent.Movement.LocalUp;
        projectile.SetParentCar(parent);
    }
}