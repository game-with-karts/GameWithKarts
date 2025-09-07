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
    [SerializeField] CheckBox spawnBots;
    [SerializeField] ChoiceBox playerSpawningInput;
    [Space]
    [SerializeField] private ItemSettingsScreen itemSettings;

    private void NumLapsUpdate(RaceSettings settings, int laps) {
        if (laps >= minLaps && laps <= maxLaps) {
            settings.numberOfLaps = (byte)laps;
        }
    }

    public void UpdateRaceSettings(RaceSettings settings) {
        NumLapsUpdate(settings, lapsInput.Value);
        settings.raceMode = (RaceMode)raceModeInput.Value;
        settings.playerSpawning = (PlayerSpawning)playerSpawningInput.Value;
        settings.useItems = useItems.Value;
        settings.trackFeatures = trackFeatures.Value;
        settings.mirrorMode = mirrorMode.Value;
        settings.spawnBots = spawnBots.Value;
        playerSpawningInput.enabled = spawnBots.Value;
        settings.itemsEnabled = itemSettings.Settings;
    }

    public void SetDisplayFrom(RaceSettings settings) {
        lapsInput.Value = settings.NumberOfLaps;
        raceModeInput.Value = (int)settings.RaceMode;
        useItems.Value = settings.useItems;
        trackFeatures.Value = settings.trackFeatures;
        mirrorMode.Value = settings.mirrorMode;
        spawnBots.Value = settings.spawnBots;
        playerSpawningInput.Value = (int)settings.playerSpawning;
        itemSettings.Settings = settings.itemsEnabled;
    }
}
