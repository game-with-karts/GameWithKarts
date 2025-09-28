using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "Playlst", menuName = "Track Playlist")]
public class Playlist : ScriptableObject {
    [SerializeField] private List<Track> trackList = new();
    public PlaylistSettings settings = new();

    public int Length => trackList.Count;

    public bool LastTrack => trackList.Count == 0;

    public Track this[int idx] {
        get => trackList[idx];
        set => trackList[idx] = value;
    }

    public void AddTrack(Track track) => trackList.Add(track);
    public void RemoveTrackAt(int idx) => trackList.RemoveAt(idx);
    public void Clear() => trackList.Clear();
    public Track GetNextTrack() {
        Track track = trackList[0];
        trackList.RemoveAt(0);
        return track;
    }

    public static Playlist CopyFrom(Playlist from) {
        Playlist to = CreateInstance<Playlist>();
        foreach (var track in from.trackList) {
            to.trackList.Add(new Track {
                levelName = track.levelName,
                settings = track.settings
            });
        }
        return to;
    }

}

[Serializable]
public class PlaylistSettings {
    public PlayerSpawning playerSpawning;
    public bool spawnBots;
    public bool cupScoring;
}
