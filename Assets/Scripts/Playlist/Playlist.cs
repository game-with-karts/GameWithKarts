using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Playlst", menuName = "Track Playlist")]
public class Playlist : ScriptableObject
{
    [SerializeField] private List<Track> trackList = new();
    public int Length => trackList.Count;

    public Track this[int idx] {
        get {
            return trackList[idx];
        }
        set {
            trackList[idx] = value;
        }
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
        Playlist to = ScriptableObject.CreateInstance<Playlist>();
        to.trackList = from.trackList;
        return to;
    }

}