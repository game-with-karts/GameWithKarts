using UnityEngine;

namespace GWK.UI {
    public class PlaylistEditorTrackList : MonoBehaviour {
        [SerializeField] private Button addBtn;
        [SerializeField] private ScrollableList list;
        [SerializeField] private Window window;
        [SerializeField] private PlaylistEditor editor;

        void OnEnable() {
            if (editor.Playlist is null || editor.Playlist.Length == 0) {
                window.SetFirstFocused(addBtn);
                return;
            }
            window.SetFirstFocused(list);
        }
    }
}