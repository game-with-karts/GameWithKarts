using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;
using System.Collections.Generic;

public class TrackRulesEditor : MonoBehaviour
{
    const byte minLaps = 1;
    const byte maxLaps = 99;
    [SerializeField] TMP_Dropdown raceModeInput;
    [SerializeField] TMP_InputField lapsInput;
    [SerializeField] Toggle useItems;
    [SerializeField] Toggle trackFeatures;
    [SerializeField] Toggle mirrorMode;
    [SerializeField] Toggle survivalMode;
    [SerializeField] Toggle spawnBots;
    [SerializeField] TMP_Dropdown playerSpawningInput;

    private void NumLapsUpdate(RaceSettings settings, string lapsString) {
        if (!byte.TryParse(lapsString, out byte num)) return;

        if (num >= minLaps && num <= maxLaps) {
            settings.numberOfLaps = num;
        }
    }

    public void UpdateRaceSettings(RaceSettings settings) {

        NumLapsUpdate(settings, lapsInput.text);
        settings.raceMode = (RaceMode)raceModeInput.value;
        settings.playerSpawning = (PlayerSpawning)playerSpawningInput.value;
        settings.useItems = useItems.isOn;
        settings.trackFeatures = trackFeatures.isOn;
        settings.mirrorMode = mirrorMode.isOn;
        settings.survivalMode = survivalMode.isOn;
        settings.spawnBots = spawnBots.isOn;
        playerSpawningInput.enabled = spawnBots.isOn;
    }

    public void SetDisplayFrom(RaceSettings settings) {
        DisableAll();
        lapsInput.text = settings.numberOfLaps.ToString();
        raceModeInput.value = (int)settings.raceMode;
        useItems.isOn = settings.useItems;
        trackFeatures.isOn = settings.trackFeatures;
        mirrorMode.isOn = settings.mirrorMode;
        survivalMode.isOn = settings.survivalMode;
        spawnBots.isOn = settings.spawnBots;
        playerSpawningInput.value = (int)settings.playerSpawning;
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

    private void DisableOnValueChanged(Toggle obj) {
        obj.onValueChanged.SetPersistentListenerState(0, UnityEventCallState.Off);
    }

    private void DisableOnValueChanged(TMP_InputField obj) {
        obj.onValueChanged.SetPersistentListenerState(0, UnityEventCallState.Off);
    }

    private void DisableOnValueChanged(TMP_Dropdown obj) {
        obj.onValueChanged.SetPersistentListenerState(0, UnityEventCallState.Off);
    }

    private void EnableOnValueChanged(Toggle obj) {
        obj.onValueChanged.SetPersistentListenerState(0, UnityEventCallState.RuntimeOnly);
    }

    private void EnableOnValueChanged(TMP_InputField obj) {
        obj.onValueChanged.SetPersistentListenerState(0, UnityEventCallState.RuntimeOnly);
    }

    private void EnableOnValueChanged(TMP_Dropdown obj) {
        obj.onValueChanged.SetPersistentListenerState(0, UnityEventCallState.RuntimeOnly);
    }

}