using UnityEngine;

public interface ITargetable {
    public void MarkAsTarget(ItemProjectile projectile);
    public void ClearTarget();
    public Vector3 Position { get; }
}