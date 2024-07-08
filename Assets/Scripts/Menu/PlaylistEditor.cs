using UnityEngine;
using GWK.UI;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;
using System;

public class PlaylistEditor : MonoBehaviour
{
    [SerializeField] private TrackRulesEditor trackEditor;
    [Header("UI elements")]
    [SerializeField] private ScrollableList scrollableList;
    [Header("Defaults")]
    [SerializeField] private Playlist emptyPlaylist;
    [SerializeField] private RaceSettings defaultSettings;

    private Playlist playlist;
    public Playlist Playlist => playlist;
    private RaceSettings settings;
    private int selectedItem = -1;
    [SerializeField] private List<string> trackNames;
    private List<PlaylistEditorTrackEntry> entries = new();

    private void OnEnable() {
        scrollableList.Clear();
        playlist = ScriptableObject.CreateInstance<Playlist>();
    }

    public void Add() {
        selectedItem = playlist.Length;
        playlist.AddTrack(new(1, RaceSettings.CloneSettings(defaultSettings)));
    }

    public void Remove() {
        scrollableList.Remove();
    }

    public void Clear() {
        playlist.Clear();

        foreach(var entry in entries) {
            Destroy(entry.gameObject);
        }
        entries = new();
    }

    public void MoveUp() {
        scrollableList.MoveUp();
    }

    public void MoveDown() {
        scrollableList.MoveDown();
    }

    public void DefaultTrackSettings() {
        defaultSettings.CopyTo(playlist[selectedItem].settings);
    }

    public void ApplyToAll() {
        if (selectedItem < 0) return;
        RaceSettings settings = playlist[scrollableList.SelectedIndex].settings;
        for (int i = 0; i < playlist.Length; i++) {
            if (i == selectedItem) continue;
            settings.CopyTo(playlist[i].settings);
        }

    }

    public void UpdateSettings() {
        if (playlist.Length > 0) trackEditor.UpdateRaceSettings(playlist[scrollableList.SelectedIndex].settings);
    }


    private string GetRaceModeString(int idx) {
        RaceMode mode = playlist[idx].settings.raceMode;
        return mode switch {
            RaceMode.Regular => "Regular Race",
            RaceMode.Arcade => "Arcade",
            RaceMode.Domination => "Domination",
            RaceMode.HotPotato => "Hot Potato",
            RaceMode.LastManStanding => "Last Man Standing",
            _ => string.Empty
        };
    }
}