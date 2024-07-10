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
    [SerializeField] CheckBox survivalMode;
    [SerializeField] CheckBox spawnBots;
    [SerializeField] ChoiceBox playerSpawningInput;

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
        settings.survivalMode = survivalMode.Value;
        settings.spawnBots = spawnBots.Value;
        playerSpawningInput.enabled = spawnBots.Value;
    }

    public void SetDisplayFrom(RaceSettings settings) {
        DisableAll();
        lapsInput.Value = settings.numberOfLaps;
        raceModeInput.Value = (int)settings.raceMode;
        useItems.Value = settings.useItems;
        trackFeatures.Value = settings.trackFeatures;
        mirrorMode.Value = settings.mirrorMode;
        survivalMode.Value = settings.survivalMode;
        spawnBots.Value = settings.spawnBots;
        playerSpawningInput.Value = (int)settings.playerSpawning;
        EnableAll();
    }

    private void DisableAll() {
        DisableOnValueChanged(raceModeInput);
        DisableOnValueChanged(lapsInput);
        DisableOnValueChanged(useItems);
        DisableOnValueChanged(trackFeatures);
        DisableOnValueChanged(mirrorMode);
        DisableOnValueChanged(survivalMode);
        DisableOnValueChanged(spawnBots);
        DisableOnValueChanged(playerSpawningInput);
    }

    private void EnableAll() {
        EnableOnValueChanged(raceModeInput);
        EnableOnValueChanged(lapsInput);
        EnableOnValueChanged(useItems);
        EnableOnValueChanged(trackFeatures);
        EnableOnValueChanged(mirrorMode);
        EnableOnValueChanged(survivalMode);
        EnableOnValueChanged(spawnBots);
        EnableOnValueChanged(playerSpawningInput);
    }

    private void DisableOnValueChanged(CheckBox obj) {
        obj.OnValueChanged.SetPersistentListenerState(0, UnityEventCallState.Off);
    }

    private void DisableOnValueChanged(NumberInputBox obj) {
        obj.OnValueChanged.SetPersistentListenerState(0, UnityEventCallState.Off);
    }

    private void DisableOnValueChanged(ChoiceBox obj) {
        obj.OnValueChanged.SetPersistentListenerState(0, UnityEventCallState.Off);
    }

    private void EnableOnValueChanged(CheckBox obj) {
        obj.OnValueChanged.SetPersistentListenerState(0, UnityEventCallState.RuntimeOnly);
    }

    private void EnableOnValueChanged(NumberInputBox obj) {
        obj.OnValueChanged.SetPersistentListenerState(0, UnityEventCallState.RuntimeOnly);
    }

    private void EnableOnValueChanged(ChoiceBox obj) {
        obj.OnValueChanged.SetPersistentListenerState(0, UnityEventCallState.RuntimeOnly);
    }

}