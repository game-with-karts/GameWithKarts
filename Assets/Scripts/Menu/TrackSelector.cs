using UnityEngine;
using UnityEngine.Events;

public class TrackSelector : MonoBehaviour
{
    [SerializeField] private int trackIndex;
    [SerializeField] private RaceSettings settings;
    [SerializeField] private UnityEvent<TrackSelector> OnTrackSelected;
    public int TrackIndex => trackIndex;
    public RaceSettings Settings => settings;

    public Playlist GetTrackAsPlaylist() {
        Track t = new Track {
            sceneIdx = trackIndex,
            settings = this.settings
        };
        Playlist playlist = ScriptableObject.CreateInstance<Playlist>();
        playlist.AddTrack(t);
        return playlist;
    }

    public void Select() {
        OnTrackSelected.Invoke(this);
    }
}