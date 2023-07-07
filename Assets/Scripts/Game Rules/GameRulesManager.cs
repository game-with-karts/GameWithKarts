using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameRulesManager : MonoBehaviour
{
    public static GameRulesManager instance = null;
    private Playlist playlist;
    public Track currentTrack { get; private set; }
    public bool isPlaylistEmpty => playlist.Length == 0;
    void OnEnable() {
        if (instance is null) instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        playlist = ScriptableObject.CreateInstance<Playlist>();
        currentTrack = null;
    }

    public void SetPlaylist(Playlist playlist) {
        this.playlist = playlist;
    }

    public Track GetNextTrack() {
        currentTrack = playlist.GetNextTrack();
        return currentTrack;
    }

}