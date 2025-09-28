using UnityEngine;
using UnityEngine.Events;
using System;

public class SingleTrackSelector : MonoBehaviour, ILevelSelector {
    [SerializeField] private string trackName;
    [SerializeField] private RaceSettingsContainer settings;
    [SerializeField] private PlaylistSettings playlistSettings = new() {
        playerSpawning = PlayerSpawning.BehindBots,
        spawnBots = true,
        cupScoring = false,
    };
    [SerializeField] private UnityEvent<ILevelSelector> onSelected;
    public UnityEvent<ILevelSelector> OnSelected { 
        get => onSelected;
        set => onSelected = value;
    }

    public void SetLevel(String name) {
        trackName = name;
    }

    public void Select() {
        Track t = new();
        t.levelName = trackName;
        if (settings == null) {
            t.settings = RaceSettings.CloneSettings(RaceSettings.DefaultRace);
        }
        else {
            t.settings = RaceSettings.CloneSettings(settings.settings);
        }
        Playlist playlist = ScriptableObject.CreateInstance<Playlist>();
        playlist.AddTrack(t);
        playlist.settings = playlistSettings;
        GameRulesManager.instance.playlist = playlist;
        OnSelected.Invoke(this);
    }
}
