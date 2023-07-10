using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;
using System;

public class PlaylistEditor : MonoBehaviour
{
    [SerializeField] private TrackRulesEditor trackEditor;
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private RectTransform entriesParent;
    [Header("UI elements")]
    [SerializeField] private TMP_Dropdown trackSelectDrp;
    [SerializeField] private Button addTrackBtn;
    [SerializeField] private Button removeTrackBtn;
    [SerializeField] private Button clearPlaylistBtn;
    [SerializeField] private Button moveUpBtn;
    [SerializeField] private Button moveDownBtn;
    [SerializeField] private Button defaultSettingsBtn;
    [SerializeField] private Button applyToAllBtn;
    [Header("Entries Settings")]
    [SerializeField] private Color defaultColour = Color.white;
    [SerializeField] private Color selectedColour = new(.5f, 1, .75f);
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
        if (trackSelectDrp.options.Count == 0) {
            foreach (string name in trackNames) {
                TMP_Dropdown.OptionData data = new(name);
                trackSelectDrp.options.Add(data);
            }
        }
        if(playlist is not null) Clear();
        playlist = ScriptableObject.CreateInstance<Playlist>();
        selectedItem = -1;
        UpdateUIState();
    }

    public void SelectTrack(int idx) {
        // track scene indices start at 1, idx starts at 0
        playlist[selectedItem].sceneIdx = idx + 1;
        UpdateUIState();
    }

    public void SelectForEditing(int idx) {
        selectedItem = idx;
        UpdateUIState();
    }

    public void Add() {
        selectedItem = playlist.Length;
        playlist.AddTrack(new(1, RaceSettings.CloneSettings(defaultSettings)));

        GameObject entryObj = Instantiate(entryPrefab, entriesParent);
        PlaylistEditorTrackEntry entry = entryObj.GetComponent<PlaylistEditorTrackEntry>();
        Button entryBtn = entryObj.GetComponent<Button>();
        ((RectTransform)entry.transform).anchoredPosition = new(0, entry.height * -selectedItem);
        entriesParent.sizeDelta = new(0, (entries.Count + 1) * entry.height);
        entry.index = selectedItem;
        entry.editor = this;
        entries.Add(entry);
        UpdateUIState();
    }

    public void Remove() {
        if (selectedItem < 0 || selectedItem >= playlist.Length) return;
        playlist.RemoveTrackAt(selectedItem);

        Destroy(entries[selectedItem].gameObject);
        entries.RemoveAt(selectedItem);
        float height = entries.Count == 0 ? 0 : entries.Count * entries[0].height;
        entriesParent.sizeDelta = new(0, height);

        if (selectedItem >= playlist.Length)
            selectedItem--;
        
        UpdateUIState();
    }

    public void Clear() {
        playlist.Clear();

        foreach(var entry in entries) {
            Destroy(entry.gameObject);
        }
        entries = new();
        entriesParent.sizeDelta = new(0, 0);
        UpdateUIState();
    }

    public void MoveUp() {
        if (selectedItem <= 0) return;
        (playlist[selectedItem], playlist[selectedItem - 1]) = (playlist[selectedItem - 1], playlist[selectedItem]);
        selectedItem--;
        UpdateUIState();
    }

    public void MoveDown() {
        if (selectedItem == playlist.Length - 1 || selectedItem < 0) return;
        (playlist[selectedItem], playlist[selectedItem + 1]) = (playlist[selectedItem + 1], playlist[selectedItem]);
        selectedItem++;
        UpdateUIState();
    }

    public void DefaultTrackSettings() {
        playlist[selectedItem].settings = RaceSettings.CloneSettings(defaultSettings);
        UpdateUIState();
    }

    public void ApplyToAll() {
        if (selectedItem < 0) return;
        RaceSettings settings = playlist[selectedItem].settings;
        for (int i = 0; i < playlist.Length; i++) {
            if (i == selectedItem) continue;
            playlist[i].settings = settings;
        }
        UpdateUIState();
    }

    public void UpdateSettings() {
        if (playlist.Length > 0) trackEditor.UpdateRaceSettings(playlist[selectedItem].settings);
    }

    private void UpdateUIState () {
        if (playlist.Length == 0) {
            selectedItem = -1;
        } 
        else {
            trackSelectDrp.value = playlist[selectedItem].sceneIdx - 1;
            trackEditor.SetDisplayFrom(playlist[selectedItem].settings);
        }
        trackEditor.gameObject.SetActive(selectedItem >= 0);
        

        if (entries.Count == 0) return;
        for (int i = 0; i < entries.Count; i++) {
            (entries[i].transform as RectTransform).anchoredPosition = new(0, -i * entries[i].height);
            entries[i].IndexDisplay.text = (i + 1).ToString();
            entries[i].NameDisplay.text = trackNames[playlist[i].sceneIdx - 1];
            entries[i].RaceModeDisplay.text = "test";
            entries[i].index = i;
            entries[i].color = i == selectedItem ? selectedColour : defaultColour;
        }
    }
}