using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    [SerializeField] private GameObject pauseScreen;
    private bool isPaused;
    public bool IsPaused => isPaused;
    private bool raceFinished;

    void Awake() {
        if (instance is null) instance = this;
        else Destroy(this);

        isPaused = false;
        raceFinished = false;
    }

    void Update() {
        pauseScreen.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void TogglePause(InputAction.CallbackContext ctx) {
        if (ctx.canceled && !raceFinished) isPaused = !isPaused;
    }

    public void ResetRace() {
        raceFinished = false;
        isPaused = false;
    }

    public void RaceEnd() => raceFinished = true;
}