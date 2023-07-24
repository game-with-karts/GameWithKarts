using System.Collections.Generic;

public static class GameRulesManager
{
    const int MAX_PLAYERS = 8;
    public static Playlist playlist = null;
    public static Track currentTrack = null;
    private static string playerName = "Test player";
    public static List<PlayerInfo> players;
    public static bool isPlaylistEmpty => playlist.Length == 0;

    public static Track GetNextTrack() {
        currentTrack = playlist.GetNextTrack();
        return currentTrack;
    }

    public static void SpawnPlayersForRace() {
        if (players is not null) return;
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

    public static void SetPlayerName(string name) => playerName = name;
}