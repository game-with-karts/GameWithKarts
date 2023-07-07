using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameRulesManager : MonoBehaviour
{
    const int MAX_PLAYERS = 8;
    public static GameRulesManager instance = null;
    private Playlist playlist;
    public Track currentTrack { get; private set; }
    private string playerName;
    public List<PlayerInfo> players;
    public bool isPlaylistEmpty => playlist.Length == 0;
    void OnEnable() {
        if (instance is null) instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        playlist = ScriptableObject.CreateInstance<Playlist>();
        currentTrack = null;
        playerName = "Test player";
    }

    public void SetPlaylist(Playlist playlist) {
        this.playlist = playlist;
    }

    public Track GetNextTrack() {
        currentTrack = playlist.GetNextTrack();
        return currentTrack;
    }

    public void SpawnPlayersForRace() {
        int numPlayers = 1;
        int numBots = currentTrack.settings.spawnBots ? MAX_PLAYERS - numPlayers : 0;
        players = new();
        for (int i = 0; i < numPlayers; i++) {
            players.Add(new(playerName, true));
        }
        for (int i = 0; i < numBots; i++) {
            players.Add(new($"Bot{i}", false));
        }
    }

    public void SetPlayerName(string name) => playerName = name;

}