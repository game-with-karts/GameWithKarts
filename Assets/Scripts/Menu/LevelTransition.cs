using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private UnityEvent onTransitionEnd;
    public UnityEvent OnTransitionEnd => onTransitionEnd;

    private bool isAnimated = false;
    private Material mat;
    private float startTime;

    private const string progressName = "_Progress";

    public void StartTransition() {
        gameObject.SetActive(true);
        SoundManager.StopMusic();
        startTime = Time.time;
        mat = image.material;
        mat.SetFloat(progressName, 0);
        isAnimated = true;
    }

    private void Update() {
        if (!isAnimated) return;
        float t = (Time.time - startTime) / duration;
        if (t >= 1) {
            isAnimated = false;
            onTransitionEnd.Invoke();
        }
        mat.SetFloat(progressName, t);
    }
}