using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    private const string menuSceneName = "Menu 1";
    [SerializeField] private bool goToMenu = false;
    public void LoadLevel() {
        StartCoroutine(nameof(LoadLevelCoroutine));
    }

    private IEnumerator LoadLevelCoroutine() {
        Track t = null;
        if (GameRulesManager.instance.playlist is not null && !GameRulesManager.instance.isPlaylistEmpty) {
            t = GameRulesManager.instance.GetNextTrack();
            GameRulesManager.instance.SpawnPlayersForRace();
        }
            
        var loading = SceneManager.LoadSceneAsync(goToMenu ? menuSceneName : t.levelName);
        if (goToMenu) {
            GameRulesManager.instance.players = null;
        }
        while (!loading.isDone) {
            yield return new WaitForEndOfFrame();
        }
    }
}
