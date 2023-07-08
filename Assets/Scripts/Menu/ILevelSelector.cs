using UnityEngine.Events;

public interface ILevelSelector {
    public UnityEvent<ILevelSelector> OnSelected { get; set; }
    public Playlist GetPlaylist();
    public void Select();
}