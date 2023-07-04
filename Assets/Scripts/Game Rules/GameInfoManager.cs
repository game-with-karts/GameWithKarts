using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameRulesManager : MonoBehaviour
{
    public static GameRulesManager instance;
    private Playlist playlist;
    public RaceSettings settings { get; private set; }
    public bool isEmpty => playlist.Length == 0;
    void OnEnable() {
        if (instance is null) instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void SetPlaylist(Playlist playlist) {
        this.playlist = playlist;
    }

    public void LoadLevel() {
        StartCoroutine(nameof(_load));
    }

    private IEnumerator _load() {
        Track track = playlist.GetNextTrack();
        settings = track.settings;
        var load = SceneManager.LoadSceneAsync(track.sceneIdx);
        while (!load.isDone) {
            yield return new WaitForEndOfFrame();
        }
    }

}