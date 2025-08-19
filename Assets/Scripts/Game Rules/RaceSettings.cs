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

    public bool timeAttackMode;

    public static RaceSettings CloneSettings(RaceSettings from) {
        RaceSettings to = CreateInstance<RaceSettings>();
        to.numberOfLaps = from.numberOfLaps;
        to.raceMode = from.raceMode;
        to.playerSpawning = from.playerSpawning;
        to.spawnBots = from.spawnBots;
        to.mirrorMode = from.mirrorMode;
        to.useItems = from.useItems;
        to.survivalMode = from.survivalMode;
        to.trackFeatures = from.trackFeatures;
        to.itemSettings = from.itemSettings;
        to.timeAttackMode = from.timeAttackMode;
        return to;
    }

    public void CopyTo(RaceSettings to) {
        to.numberOfLaps = numberOfLaps;
        to.raceMode = raceMode;
        to.playerSpawning = playerSpawning;
        to.spawnBots = spawnBots;
        to.mirrorMode = mirrorMode;
        to.useItems = useItems;
        to.survivalMode = survivalMode;
        to.trackFeatures = trackFeatures;
        to.itemSettings = itemSettings;
        to.timeAttackMode = timeAttackMode;
    }
}