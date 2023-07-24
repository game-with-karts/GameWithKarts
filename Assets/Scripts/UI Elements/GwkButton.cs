using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
public class GwkButton : MonoBehaviour
{
    public enum ClickMode {
        Default,
        Confirm,
        Back
    }

    [SerializeField] private Button button;
    [SerializeField] private EventTrigger trigger;
    [SerializeField] private bool playSoundOnHover = false;
    [Space]
    [SerializeField] private ClickMode clickMode;
    [Header("Events")]
    [SerializeField] private UnityEvent onClick;
    [SerializeField] private UnityEvent onHover;
    [SerializeField] private UnityEvent onHoverEnd;
    [Header("Animations")]
    [SerializeField] private GwkAnimationTarget hoverAnimation;
    [SerializeField] private GwkAnimationTarget hoverEndAnimation;
    public UnityEvent OnClick => onClick;
    public UnityEvent OnHover => onHover;
    public UnityEvent OnHoverEnd => onHoverEnd;

    public void Click() => onClick.Invoke();
    public void Hover() { 
        onHover.Invoke();
        hoverAnimation.Play();
        hoverEndAnimation.Stop();
    }
    public void EndHover() {
        onHoverEnd.Invoke();
        hoverEndAnimation.Play();
        hoverAnimation.Stop();
    }

    private void Awake() {
        if (playSoundOnHover)
            onHover.AddListener(UIAudioManager.OnHover);
        UnityAction call = clickMode switch {
            ClickMode.Default => UIAudioManager.OnConfirm,
            ClickMode.Confirm => UIAudioManager.OnConfirm,
            ClickMode.Back => UIAudioManager.OnBack,
            _ => () => {}
        };
        onClick.AddListener(call);
    }

    private void Start() {
        print($"{gameObject.name}\t{transform.localPosition}");
        hoverAnimation.SetTransform(transform as RectTransform);
        hoverEndAnimation.SetTransform(transform as RectTransform);
    }

    void Update() {
        hoverAnimation.Tick(Time.unscaledDeltaTime);
        hoverEndAnimation.Tick(Time.unscaledDeltaTime);
    }

    void OnDisable() {
        hoverAnimation.Revert();
    }
}