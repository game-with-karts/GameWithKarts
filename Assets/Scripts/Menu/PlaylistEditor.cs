using UnityEngine;
using UnityEngine.UI;
using GWK.UI;
using TMPro;

public class PlaylistEditor : MonoBehaviour
{
    [Header("Rules Editor")]
    [SerializeField] private TrackRulesEditor trackEditor;
    [SerializeField] private PlaylistRulesEditor playlistEditor;
    [SerializeField] private Image ruleEditorThumbnail;
    [SerializeField] private TMP_Text ruleEditorTrackName;
    [Header("UI elements")]
    [SerializeField] private ScrollableList scrollableList;
    [Header("Screens")]
    [SerializeField] private GameObject trackList;
    [SerializeField] private GameObject rulesEditor;
    [SerializeField] private GameObject trackSelector;
    [Header("Defaults")]
    [SerializeField] private Playlist emptyPlaylist;
    [SerializeField] private RaceSettingsContainer defaultSettings;

    private Playlist playlist;
    public Playlist Playlist => playlist;
    private int selectedItem => scrollableList.SelectedIndex;

    private void OnEnable() {
        scrollableList.Clear();
        playlist = ScriptableObject.CreateInstance<Playlist>();
        playlist.settings = new() {
            playerSpawning = PlayerSpawning.BehindBots,
            spawnBots = true,
            cupScoring = false,
        };
        playlistEditor.SetDisplayFrom(playlist.settings);
        trackList.SetActive(true);
    }

    private bool isAdding;
    public void SetAddingState(bool isAdding) => this.isAdding = isAdding;

    public void Add(string levelName) {
        if (!isAdding) {
            playlist[selectedItem].levelName = levelName;
            trackEditor.SetDisplayFrom(playlist[selectedItem].settings);
        }
        else {
            Track track = new(levelName, RaceSettings.CloneSettings(defaultSettings.settings));
            playlist.AddTrack(track);
            scrollableList.AddTrack(track);
            trackEditor.SetDisplayFrom(track.settings);
        }
        ruleEditorTrackName.text = levelName;
        ruleEditorThumbnail.sprite = scrollableList.GetThumbnail(levelName);
        trackSelector.SetActive(false);
        rulesEditor.SetActive(true);
        SoundManager.OnConfirmUI();
    }

    public void Remove() {
        if (playlist.Length == 0) {
            return;
        }
        playlist.RemoveTrackAt(selectedItem);
        scrollableList.Remove();
    }

    public void Clear() {
        playlist.Clear();
        scrollableList.Clear();
    }

    public void MoveUp() {
        if (selectedItem == 0) {
            return;
        }
        (playlist[selectedItem], playlist[selectedItem - 1]) = (playlist[selectedItem - 1], playlist[selectedItem]);
        scrollableList.MoveUp();
    }

    public void MoveDown() {
        if (selectedItem == playlist.Length - 1) {
            return;
        }
        (playlist[selectedItem], playlist[selectedItem + 1]) = (playlist[selectedItem + 1], playlist[selectedItem]);
        scrollableList.MoveDown();
    }

    public void RefreshTracks() {
        scrollableList.UpdateAllTracks(playlist);
    }

    public void DefaultTrackSettings() {
        defaultSettings.settings.CopyTo(playlist[selectedItem].settings);
    }

    public void ApplyToAll() {
        if (selectedItem < 0) return;
        RaceSettings settings = playlist[scrollableList.SelectedIndex].settings;
        for (int i = 0; i < playlist.Length; i++) {
            if (i == selectedItem) continue;
            settings.CopyTo(playlist[i].settings);
        }
    }

    public void SelectForEditing(int idx) {
        trackList.SetActive(false);
        (ruleEditorTrackName.text, ruleEditorThumbnail.sprite) = (playlist[idx].levelName, scrollableList.GetThumbnail(playlist[idx].levelName));
        trackEditor.SetDisplayFrom(playlist[idx].settings);
        rulesEditor.SetActive(true);
    }

    public void UpdateSettings() {
        if (playlist.Length > 0) trackEditor.UpdateRaceSettings(playlist[scrollableList.SelectedIndex].settings);
        playlistEditor.UpdateRaceSettings(playlist.settings);
    }

    public static string GetRaceModeString(RaceMode mode) => mode switch {
        RaceMode.Regular => "Regular Race",
        RaceMode.Survival => "Survival",
        RaceMode.Domination => "Domination",
        RaceMode.HotPotato => "Hot Potato",
        RaceMode.LastManStanding => "Last Man Standing",
        _ => string.Empty
    };
}
