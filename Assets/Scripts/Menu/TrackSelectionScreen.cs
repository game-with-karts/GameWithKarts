using UnityEngine;

public class TrackSelectionScreen : MonoBehaviour
{
    private ILevelSelector selector;
    public void GetSelector(ILevelSelector selector) {
        this.selector = selector;
        GameRulesManager.instance.SetPlaylist(selector.GetPlaylist());
        print(GameRulesManager.instance);
    }
}