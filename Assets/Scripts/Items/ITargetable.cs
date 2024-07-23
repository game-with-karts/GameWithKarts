using UnityEngine;

public interface ITargetable {
    public void MarkAsTarget(ItemProjectile projectile);
    public Vector3 Position { get; }
}