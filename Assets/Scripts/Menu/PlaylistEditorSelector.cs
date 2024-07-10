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

    public void Select() {
        if (editor.Playlist.Length == 0) {
            return;
        }
        GameRulesManager.playlist = editor.Playlist;
        onSelected.Invoke(this);
    }
}