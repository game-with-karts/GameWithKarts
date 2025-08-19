using UnityEngine;
using UnityEngine.InputSystem;
using System;
using GWK.Kart;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    [SerializeField] private GameObject pauseScreen;
    private PlayerInputActions inputs;
    private bool isPaused;
    public bool IsPaused => isPaused;
    public Action<bool> OnPause;
    private bool raceFinished;

    void Awake() {
        inputs = new();
        Init();
        inputs.UI.Pause.performed += TogglePause;
        inputs.UI.Pause.Enable();
    }

    void Update() {
        pauseScreen.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void SimpleTogglePause() {
        if (!raceFinished) {
            isPaused = !isPaused;
            OnPause?.Invoke(isPaused);
            Cursor.visible = isPaused;
        }
    }

    public void TogglePause(InputAction.CallbackContext ctx) {
        SimpleTogglePause();
    }

    public void ResetRace() {
        Init();
    }

    private void Init() {
        raceFinished = false;
        isPaused = false;
        Cursor.visible = false;
    }

    public void RaceEnd(BaseCar _) {
        raceFinished = true;
        Cursor.visible = true;
    }

    private void OnDestroy() {
        Cursor.visible = true;
        Time.timeScale = 1;
        inputs.UI.Disable();
        inputs.UI.Pause.Disable();
        inputs.UI.Pause.performed -= TogglePause;
    }
}