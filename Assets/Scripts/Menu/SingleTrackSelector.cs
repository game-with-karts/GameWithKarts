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

    public void SetLevel(String name) {
        trackName = name;
    }

    public void Select() {
        Track t = new Track {
            levelName = trackName,
            settings = RaceSettings.CloneSettings(settings)
        };
        Playlist playlist = ScriptableObject.CreateInstance<Playlist>();
        playlist.AddTrack(t);
        GameRulesManager.instance.playlist = playlist;
        OnSelected.Invoke(this);
    }
}
