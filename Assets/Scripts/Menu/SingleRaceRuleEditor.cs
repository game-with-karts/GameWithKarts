using UnityEngine;

public class SingleRaceRuleEditor : MonoBehaviour {
    [SerializeField] TrackRulesEditor editor;
    [SerializeField] PlaylistRulesEditor playlistEditor;

    void OnEnable() {
        editor.SetDisplayFrom(GameRulesManager.instance.playlist[0].settings);
        playlistEditor.SetDisplayFrom(GameRulesManager.instance.playlist.settings);
    }

    public void UpdateSettings() {
        editor.UpdateRaceSettings(GameRulesManager.instance.playlist[0].settings);
        playlistEditor.UpdateRaceSettings(GameRulesManager.instance.playlist.settings);
    }
}
