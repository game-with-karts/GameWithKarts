using System.Collections.Generic;
using UnityEngine;

public static class GameRulesManager
{
    const int MAX_PLAYERS = 8;
    // TODO: not hard-coded colours
    private static readonly Color[] colors = new Color[MAX_PLAYERS] {
        new Color(1f, 0f, 0f),
        new Color(1f, 0.4f, 0f),
        new Color(1f, 1f, 0f),
        new Color(0f, 1f, 0f),
        new Color(0f, 1f, 1f),
        new Color(0f, 0f, 1f),
        new Color(0.4f, 0f, 1f),
        new Color(1f, 0f, 1f),
    };
    public static Playlist playlist = null;
    public static Track currentTrack = null;
    private static string playerName = "Player";
    public static List<PlayerInfo> players;
    public static bool isPlaylistEmpty => playlist.Length == 0;

    public static Track GetNextTrack() {
        currentTrack = playlist.GetNextTrack();
        return currentTrack;
    }

    public static void SpawnPlayersForRace() {
        if (players is not null) return;
        int numPlayers = 1;
        int numBots = MAX_PLAYERS - numPlayers;
        int colorIdx = 0;
        players = new();
        for (int i = 0; i < numPlayers; i++) {
            players.Add(new(playerName, true, colors[colorIdx++]));
        }
        for (int i = 0; i < numBots; i++) {
            players.Add(new($"Bot {i}", false, colors[colorIdx++]));
        }
    }

    public static void SetPlayerName(string name) => playerName = name;
}
