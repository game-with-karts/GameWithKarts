using UnityEngine;
using UnityEngine.Events;
using GWK.UI;

public class TrackRulesEditor : MonoBehaviour
{
    const int minLaps = 1;
    const int maxLaps = 99;
    [SerializeField] ChoiceBox raceModeInput;
    [SerializeField] NumberInputBox lapsInput;
    [SerializeField] CheckBox useItems;
    [SerializeField] CheckBox trackFeatures;
    [SerializeField] CheckBox mirrorMode;
    [SerializeField] RectTransform itemSettingsButton;
    [Space]
    [SerializeField] private ItemSettingsScreen itemSettings;

    void OnEnable() {
        // HACK: this should remove weird behaviour in playlist editor
        Vector2 pos = itemSettingsButton.anchoredPosition;
        pos.x = 0;
        itemSettingsButton.anchoredPosition = pos;
    }

    private void NumLapsUpdate(RaceSettings settings, int laps) {
        if (laps >= minLaps && laps <= maxLaps) {
            settings.numberOfLaps = (byte)laps;
        }
    }

    public void UpdateRaceSettings(RaceSettings settings) {
        NumLapsUpdate(settings, lapsInput.Value);
        settings.raceMode = (RaceMode)raceModeInput.Value;
        settings.useItems = useItems.Value;
        settings.trackFeatures = trackFeatures.Value;
        settings.mirrorMode = mirrorMode.Value;
        settings.itemsEnabled = itemSettings.Settings;
    }

    public void SetDisplayFrom(RaceSettings settings) {
        lapsInput.Value = settings.numberOfLaps;
        raceModeInput.Value = (int)settings.raceMode;
        useItems.Value = settings.useItems;
        trackFeatures.Value = settings.trackFeatures;
        mirrorMode.Value = settings.mirrorMode;
        itemSettings.Settings = settings.itemsEnabled;
    }
}
