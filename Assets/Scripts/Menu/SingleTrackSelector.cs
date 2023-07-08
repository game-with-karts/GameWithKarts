using UnityEngine;
using UnityEngine.Events;

public class SingleTrackSelector : MonoBehaviour, ILevelSelector
{
    [SerializeField] private int trackIndex;
    [SerializeField] private RaceSettings settings;
    [SerializeField] private UnityEvent<ILevelSelector> onSelected;
    public UnityEvent<ILevelSelector> OnSelected { 
        get => onSelected;
        set => onSelected = value;
    }
    public Playlist GetPlaylist() {
        Track t = new Track {
            sceneIdx = trackIndex,
            settings = this.settings
        };
        Playlist playlist = ScriptableObject.CreateInstance<Playlist>();
        playlist.AddTrack(t);
        return playlist;
    }

    public void Select() {
        OnSelected.Invoke(this);
    }
}