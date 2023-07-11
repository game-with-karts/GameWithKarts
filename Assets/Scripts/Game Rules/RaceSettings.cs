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

    public static RaceSettings CloneSettings(RaceSettings from) {
        RaceSettings to = ScriptableObject.CreateInstance<RaceSettings>();
        to.numberOfLaps = from.numberOfLaps;
        to.raceMode = from.raceMode;
        to.playerSpawning = from.playerSpawning;
        to.spawnBots = from.spawnBots;
        to.mirrorMode = from.mirrorMode;
        to.useItems = from.useItems;
        to.survivalMode = from.survivalMode;
        to.trackFeatures = from.trackFeatures;
        to.itemSettings = from.itemSettings;
        return to;
    }
}