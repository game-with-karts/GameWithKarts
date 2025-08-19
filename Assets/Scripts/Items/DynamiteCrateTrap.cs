using UnityEngine;

public class DynamiteCrateTrap : ItemTrap {
    [SerializeField] private GameObject explosion;
    public override void SelfDestruct() {
        GameObject explosionObj = Instantiate(explosion, transform);
        explosionObj.transform.parent = null;
        base.SelfDestruct();
    }
}