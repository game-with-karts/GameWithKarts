using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour {
    [SerializeField] private TMP_Text fpsText;

    void Awake() {
        SettingsMenu.OnSettingsUpdated += Toggle;
        Toggle();
    }

    void OnDestroy() {
        SettingsMenu.OnSettingsUpdated -= Toggle;
    }

    private void Toggle() {
        gameObject.SetActive(PlayerPrefs.GetInt(SettingsMenu.FPSCounterKey) == 1);
    }

    void Update() {
        float fps = 1f / Time.unscaledDeltaTime;
        fpsText.text = $"{fps:0.0} FPS";
    }
}