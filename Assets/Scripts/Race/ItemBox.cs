using UnityEngine;
using UnityEngine.VFX;

public class ItemBox : MonoBehaviour {
    [SerializeField] private VisualEffect boxEffect;
    [SerializeField] private VisualEffect hitEffect;
    [SerializeField] private float cooldown;
    public bool IsActive { get; private set; }
    private Timer timer = new();

    private Vector3 targetBoxEffectScale = Vector3.one;

    void Awake() {
        IsActive = true;
        timer.Stop();
        timer.Reset();
    }
    void OnTriggerEnter(Collider other) {
        if (!IsActive) {
            return;
        }
        IItemInteractable interactable = other.GetComponent<IItemInteractable>();
        if (interactable is null) {
            return;
        }
        interactable.OnItemBox();
        timer.Start();
        boxEffect.Stop();
        hitEffect.Play();
        IsActive = false;
        targetBoxEffectScale = Vector3.zero;
    }

    void Update() {
        timer.Tick(Time.deltaTime);
        targetBoxEffectScale = IsActive ? Vector3.one : Vector3.zero;
        boxEffect.transform.localScale = Vector3.Lerp(boxEffect.transform.localScale, targetBoxEffectScale, 10 * Time.deltaTime);
        if (timer.running && timer.Time >= cooldown) {
            timer.Stop();
            timer.Reset();
            IsActive = true;
            boxEffect.Play();
        }
    }
}