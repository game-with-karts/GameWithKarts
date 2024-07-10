using UnityEngine;
using UnityEngine.UI;
using GWK.UI;
using TMPro;

public class PlaylistEditor : MonoBehaviour
{
    [Header("Rules Editor")]
    [SerializeField] private TrackRulesEditor trackEditor;
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
    [SerializeField] private RaceSettings defaultSettings;

    private Playlist playlist;
    public Playlist Playlist => playlist;
    private int selectedItem => scrollableList.SelectedIndex;

    private void OnEnable() {
        scrollableList.Clear();
        playlist = ScriptableObject.CreateInstance<Playlist>();
        trackList.SetActive(true);
    }

    private bool isAdding;
    public void SetAddingState(bool isAdding) => this.isAdding = isAdding;

    public void Add(int sceneIdx) {
        if (!isAdding) {
            playlist[selectedItem].sceneIdx = sceneIdx;
            trackEditor.SetDisplayFrom(playlist[selectedItem].settings);
        }
        else {
            Track track = new(sceneIdx, RaceSettings.CloneSettings(defaultSettings));
            playlist.AddTrack(track);
            scrollableList.AddTrack(track);
            trackEditor.SetDisplayFrom(track.settings);
        }
        (ruleEditorTrackName.text, ruleEditorThumbnail.sprite) = scrollableList.GetAssetsAtIndex(sceneIdx);
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

    public void SelectForEditing(int idx) {
        trackList.SetActive(false);
        (ruleEditorTrackName.text, ruleEditorThumbnail.sprite) = scrollableList.GetAssetsAtIndex(playlist[idx].sceneIdx);
        trackEditor.SetDisplayFrom(playlist[idx].settings);
        rulesEditor.SetActive(true);
    }

    public void UpdateSettings() {
        if (playlist.Length > 0) trackEditor.UpdateRaceSettings(playlist[scrollableList.SelectedIndex].settings);
    }

    public static string GetRaceModeString(RaceMode mode) {
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