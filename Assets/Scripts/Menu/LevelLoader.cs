using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    private const int menuSceneIdx = 0;
    [SerializeField] private bool goToMenu = false;
    public void LoadLevel() {
        StartCoroutine(nameof(LoadLevelCoroutine));
    }

    private IEnumerator LoadLevelCoroutine() {
        Track t = null;
        if (!GameRulesManager.instance.isPlaylistEmpty) {
            t = GameRulesManager.instance.GetNextTrack();
            GameRulesManager.instance.SpawnPlayersForRace();
        }
            
        var loading = SceneManager.LoadSceneAsync(goToMenu ? menuSceneIdx : t.sceneIdx);
        while (!loading.isDone) {
            yield return new WaitForEndOfFrame();
        }
    }
}