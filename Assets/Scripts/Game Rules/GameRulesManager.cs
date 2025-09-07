using System.Collections.Generic;
using UnityEngine;

public sealed class GameRulesManager
{
    const int MAX_PLAYERS = 8;
    // TODO: not hard-coded colours
    private readonly Color[] colors = new Color[MAX_PLAYERS] {
        new Color(1f, 0f, 0f),
        new Color(1f, 0.4f, 0f),
        new Color(1f, 1f, 0f),
        new Color(0f, 1f, 0f),
        new Color(0f, 1f, 1f),
        new Color(0f, 0f, 1f),
        new Color(0.4f, 0f, 1f),
        new Color(1f, 0f, 1f),
    };

    private static GameRulesManager _instance;
    public static GameRulesManager instance {
        get {
            if (_instance is null) {
                _instance = new();
            }
            return _instance;
        }
    }

    public Playlist playlist = null;
    public Track currentTrack = null;
    private string playerName = "Player";
    public List<PlayerInfo> players;
    public bool isPlaylistEmpty => playlist.Length == 0;

    public Track GetNextTrack() {
        currentTrack = playlist.GetNextTrack();
        return currentTrack;
    }

    public void SpawnPlayersForRace() {
        if (players is not null) return;
        int numPlayers = 1;
        int numBots = MAX_PLAYERS - numPlayers;
        int colorIdx = 0;
        players = new();
        for (int i = 0; i < numPlayers; i++) {
            playerName = "Player";
            if (PlayerPrefs.HasKey(Constants.USERNAME_KEY) && PlayerPrefs.GetString(Constants.USERNAME_KEY) != string.Empty) {
                playerName = PlayerPrefs.GetString(Constants.USERNAME_KEY);
            }
            players.Add(new(playerName, true, colors[colorIdx++]));
        }
        for (int i = 0; i < numBots; i++) {
            players.Add(new($"Bot {i}", false, colors[colorIdx++]));
        }
    }

    public void SetPlayerName(string name) => playerName = name;
}
