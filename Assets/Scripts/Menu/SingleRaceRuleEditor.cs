using UnityEngine;

public class SingleRaceRuleEditor : MonoBehaviour {
    [SerializeField] TrackRulesEditor editor;

    void OnEnable() {
        editor.SetDisplayFrom(GameRulesManager.instance.playlist[0].settings);
    }

    public void UpdateSettings() {
        editor.UpdateRaceSettings(GameRulesManager.instance.playlist[0].settings);
    }
}
