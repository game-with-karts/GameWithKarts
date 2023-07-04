using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Race Settings", menuName = "Race Settings")]
public class RaceSettings : ScriptableObject
{
    public byte numberOfLaps;
    public RaceMode raceMode;
    public PlayerSpawning playerSpawning;
    public bool spawnBots;
    public bool mirrorMode;
    public bool useItems;
    public bool survivalMode;
    public bool trackFeatures;
    public ItemSettings itemSettings;

    public static RaceSettings Default {
        get {
            RaceSettings settings = ScriptableObject.CreateInstance<RaceSettings>();
            settings.numberOfLaps = 3;
            settings.raceMode = RaceMode.Regular;
            settings.playerSpawning = PlayerSpawning.BehindBots;
            settings.spawnBots = true;
            settings.mirrorMode = false;
            settings.useItems = true;
            settings.survivalMode = false;
            settings.itemSettings = ItemSettings.Default;
            return settings;
        }
    }
}