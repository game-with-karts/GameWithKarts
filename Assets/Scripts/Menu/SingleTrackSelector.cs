using UnityEngine;
using UnityEngine.Events;
using System;

public class SingleTrackSelector : MonoBehaviour, ILevelSelector
{
    [SerializeField] private string trackName;
    [SerializeField] private RaceSettings settings;
    [SerializeField] private UnityEvent<ILevelSelector> onSelected;
    public UnityEvent<ILevelSelector> OnSelected { 
        get => onSelected;
        set => onSelected = value;
    }
    private Playlist GetTrackAsPlaylist() {
        Track t = new Track {
            levelName = trackName,
            settings = RaceSettings.CloneSettings(settings)
        };
        Playlist playlist = ScriptableObject.CreateInstance<Playlist>();
        playlist.AddTrack(t);
        return playlist;
    }

    public void SetLevel(String name) {
        trackName = name;
        Debug.Log($"Level set: {trackName}");
    }

    public void Select() {
        GameRulesManager.playlist = GetTrackAsPlaylist();
        OnSelected.Invoke(this);
    }
}
