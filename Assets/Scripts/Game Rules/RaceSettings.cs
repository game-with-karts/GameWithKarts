using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "Race Settings", menuName = "Race Settings")]
public class RaceSettings : ScriptableObject
{
    public byte numberOfLaps;
    public RaceMode raceMode;
    public PlayerSpawning playerSpawning;
    public bool spawnBots;
    public bool mirrorMode;
    public bool useItems;
    public bool trackFeatures;
    public Dictionary<ItemType, bool> itemsEnabled;

    public byte NumberOfLaps => numberOfLaps;
    public RaceMode RaceMode => raceMode;

    public bool timeAttackMode;

    void Awake() {
        itemsEnabled = new();
        foreach (ItemType it in Enum.GetValues(typeof(ItemType))) {
            itemsEnabled[it] = true;
        }
    }

    public static RaceSettings CloneSettings(RaceSettings from) {
        RaceSettings to = CreateInstance<RaceSettings>();
        to.numberOfLaps = from.numberOfLaps;
        to.raceMode = from.raceMode;
        to.playerSpawning = from.playerSpawning;
        to.spawnBots = from.spawnBots;
        to.mirrorMode = from.mirrorMode;
        to.useItems = from.useItems;
        to.trackFeatures = from.trackFeatures;
        if (from.itemsEnabled == null) {
            to.itemsEnabled = new();
        }
        else {
            to.itemsEnabled = from.itemsEnabled.ToDictionary(e => e.Key, e => e.Value);
        }
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
        to.trackFeatures = trackFeatures;
        to.itemsEnabled = itemsEnabled.ToDictionary(e => e.Key, e => e.Value);
        to.timeAttackMode = timeAttackMode;
    }
}
