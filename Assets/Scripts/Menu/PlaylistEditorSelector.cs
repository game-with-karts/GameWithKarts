using UnityEngine;
using UnityEngine.Events;

public class PlaylistEditorSelector : MonoBehaviour, ILevelSelector
{
    [SerializeField] private PlaylistEditor editor;
    [SerializeField] private UnityEvent<ILevelSelector> onSelected;
    public UnityEvent<ILevelSelector> OnSelected { 
        get => onSelected;
        set => onSelected = value;
    }

    public Playlist GetPlaylist()
    {
        return editor.Playlist;
    }

    public void Select()
    {
        GameRulesManager.instance.SetPlaylist(GetPlaylist());
        onSelected.Invoke(this);
    }
}