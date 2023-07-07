using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    [SerializeField] private GameObject pauseScreen;
    private bool isPaused;
    public bool IsPaused => isPaused;
    public Action<bool> OnPause;
    private bool raceFinished;

    void Awake() {
        isPaused = false;
        raceFinished = false;
    }

    void LateUpdate() {
        pauseScreen.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void TogglePause(InputAction.CallbackContext ctx) {
        if (ctx.canceled && !raceFinished) {
            isPaused = !isPaused;
            OnPause?.Invoke(isPaused);
        }
    }

    public void ResetRace() {
        raceFinished = false;
        isPaused = false;
    }

    public void RaceEnd(BaseCar _) => raceFinished = true;
}