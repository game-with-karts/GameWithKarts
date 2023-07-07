using UnityEngine;

public class TrackSelectionScreen : MonoBehaviour
{
    private TrackSelector selector;
    public void GetSelector(TrackSelector selector) {
        this.selector = selector;
        print(GameRulesManager.instance);
    }
    
    public void SetActivePlaylist() {
        GameRulesManager.instance.SetPlaylist(selector.GetTrackAsPlaylist());
    }
}