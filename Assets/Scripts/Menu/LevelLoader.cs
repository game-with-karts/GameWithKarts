using UnityEngine;


public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Playlist playlist;

    public void SetPlaylist(Playlist playlist) => this.playlist = playlist;
    public void LoadLevel() {
        Playlist playlistCopy = ScriptableObject.CreateInstance<Playlist>();
        for (int i = 0; i < playlist.Length; i++) {
            playlistCopy.AddTrack(playlist[i]);
        }
        GameObject info = new("Level Info");
        GameRulesManager gameInfo = info.AddComponent<GameRulesManager>();
        gameInfo.SetPlaylist(playlistCopy);
        gameInfo.LoadLevel();
    }
}