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
        Init();
    }

    void LateUpdate() {
        pauseScreen.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void TogglePause(InputAction.CallbackContext ctx) {
        if (ctx.canceled && !raceFinished) {
            isPaused = !isPaused;
            OnPause?.Invoke(isPaused);
            Cursor.visible = isPaused;
        }
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
}