using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    private const int menuSceneIdx = 1;
    [SerializeField] private bool goToMenu = false;
    public void LoadLevel() {
        StartCoroutine(nameof(LoadLevelCoroutine));
    }

    private IEnumerator LoadLevelCoroutine() {
        Track t = null;
        if (GameRulesManager.playlist is not null && !GameRulesManager.isPlaylistEmpty) {
            t = GameRulesManager.GetNextTrack();
            GameRulesManager.SpawnPlayersForRace();
        }
            
        var loading = SceneManager.LoadSceneAsync(goToMenu ? menuSceneIdx : t.sceneIdx + menuSceneIdx);
        while (!loading.isDone) {
            yield return new WaitForEndOfFrame();
        }
    }
}