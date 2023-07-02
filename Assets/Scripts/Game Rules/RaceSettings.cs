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

    public static RaceSettings Default => new RaceSettings
    {
        numberOfLaps = 3,
        raceMode = RaceMode.Regular,
        playerSpawning = PlayerSpawning.BehindBots,
        spawnBots = true,
        mirrorMode = false,
        useItems = true,
        survivalMode = false,
        itemSettings = ItemSettings.Default
    };
}