using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class TrackRulesEditor : MonoBehaviour
{
    const byte minLaps = 1;
    const byte maxLaps = 99;
    [SerializeField] private RaceSettings settings;
    [SerializeField] TMP_Dropdown raceModeInput;
    [SerializeField] TMP_InputField lapsInput;
    [SerializeField] Toggle useItems;
    [SerializeField] Toggle trackFeatures;
    [SerializeField] Toggle mirrorMode;
    [SerializeField] Toggle survivalMode;
    [SerializeField] Toggle spawnBots;
    [SerializeField] TMP_Dropdown playerSpawningInput;

    private void NumLapsUpdate(string lapsString) {
        if (!byte.TryParse(lapsString, out byte num)) return;

        if (num >= minLaps && num <= maxLaps) {
            settings.numberOfLaps = num;
        }
    }

    public void UpdateRaceSettings() {
        NumLapsUpdate(lapsInput.text);
        settings.raceMode = (RaceMode)raceModeInput.value;
        settings.playerSpawning = (PlayerSpawning)playerSpawningInput.value;
        settings.useItems = useItems.isOn;
        settings.trackFeatures = trackFeatures.isOn;
        settings.mirrorMode = mirrorMode.isOn;
        settings.survivalMode = survivalMode.isOn;
        settings.spawnBots = spawnBots.isOn;
        playerSpawningInput.enabled = spawnBots.isOn;
    }

    public RaceSettings GetSettings() => settings;
}