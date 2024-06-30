using UnityEngine;

public abstract class CarComponent : MonoBehaviour
{
    protected BaseCar car;

    protected virtual void Awake() {
        car = GetComponent<BaseCar>();
    }

    public abstract void Init();
    public virtual void StartRace() {}
}
