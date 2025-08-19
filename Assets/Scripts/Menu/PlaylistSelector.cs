using UnityEngine;
using UnityEngine.Events;

public class PlaylistSelector : MonoBehaviour, ILevelSelector
{
    [SerializeField] private Playlist playlist;
    [SerializeField] private UnityEvent<ILevelSelector> onSelected;
    public UnityEvent<ILevelSelector> OnSelected { 
        get => onSelected; 
        set => onSelected = value; 
    }
    public void Select() {
        GameRulesManager.playlist = Playlist.CopyFrom(playlist);
        OnSelected.Invoke(this);
    }
}