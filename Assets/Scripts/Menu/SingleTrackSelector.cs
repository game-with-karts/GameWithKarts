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
    private Playlist GetTrackAsPlaylist() {
        Track t = new Track {
            sceneIdx = trackIndex,
            settings = RaceSettings.CloneSettings(this.settings)
        };
        Playlist playlist = ScriptableObject.CreateInstance<Playlist>();
        playlist.AddTrack(t);
        return playlist;
    }

    public void Select() {
        GameRulesManager.instance.SetPlaylist(GetTrackAsPlaylist());
        OnSelected.Invoke(this);
    }
}