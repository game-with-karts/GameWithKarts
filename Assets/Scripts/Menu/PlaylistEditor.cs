using UnityEngine;
using UnityEngine.UI;

public class PlaylistEditor : MonoBehaviour
{
    [SerializeField] private TrackRulesEditor trackEditor;
    [SerializeField] private Button addTrackBtn;
    [SerializeField] private Button removeTrackBtn;
    [SerializeField] private Button clearPlaylistBtn;
    [SerializeField] private Button moveUpBtn;
    [SerializeField] private Button moveDownBtn;
    [SerializeField] private Button defaultSettingsBtn;
    [SerializeField] private Button applyToAllBtn;
    [Space]
    [SerializeField] private Playlist emptyPlaylist;

    private Playlist playlist;
    public static int selectedItem = -1;

    private void OnEnable()
    {
        playlist = emptyPlaylist;
    }

    public void Add() {
        playlist.AddTrack(new());
    }

    public void Remove() {
        if (selectedItem < 0 || selectedItem >= playlist.Length) return;
        playlist.RemoveTrackAt(selectedItem);
    }

    public void Clear() {
        playlist.Clear();
    }

    public void MoveUp() {
        if (selectedItem == 0) return;
        (playlist[selectedItem], playlist[selectedItem - 1]) = (playlist[selectedItem - 1], playlist[selectedItem]);
    }

    public void MoveDown() {
        if (selectedItem == playlist.Length - 1) return;
        (playlist[selectedItem], playlist[selectedItem + 1]) = (playlist[selectedItem + 1], playlist[selectedItem]);
    }

    public void DefaultTrackSettings() {
        playlist[selectedItem].settings = RaceSettings.Default;
    }

    public void ApplyToAll() {
        if (selectedItem < 0) return;
        RaceSettings settings = playlist[selectedItem].settings;
        for (int i = 0; i < playlist.Length; i++) {
            if (i == selectedItem) continue;
            playlist[i].settings = settings;
        }
    }

    private void UpdateUIState () {

    }
}