using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "Race Settings", menuName = "Race Settings")]
public sealed class RaceSettingsContainer : ScriptableObject {
    public RaceSettings settings;
}

[Serializable]
public sealed class RaceSettings : ISerializationCallbackReceiver {
    public byte numberOfLaps;
    public RaceMode raceMode;
    public bool mirrorMode;
    public bool useItems;
    public bool trackFeatures;
    public Dictionary<ItemType, bool> itemsEnabled;
    public bool timeAttackMode;

    [SerializeField, HideInInspector] private List<ItemType> itemsEnabledKeys;
    [SerializeField, HideInInspector] private List<bool> itemsEnabledValues;

    public static readonly RaceSettings DefaultRace = new() {
        numberOfLaps = 3,
        raceMode = RaceMode.Regular,
        mirrorMode = false,
        useItems = true,
        trackFeatures = true,
        timeAttackMode = false,
        itemsEnabled = new Dictionary<ItemType, bool>(Enum.GetValues(typeof(ItemType)).Cast<ItemType>().ToDictionary(x => x, _ => true)),
    };

    void Awake() {
        itemsEnabled = new();
        foreach (ItemType it in Enum.GetValues(typeof(ItemType))) {
            itemsEnabled[it] = true;
        }
    }

    public static RaceSettings CloneSettings(RaceSettings from) {
        RaceSettings to = new();
        to.numberOfLaps = from.numberOfLaps;
        to.raceMode = from.raceMode;
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
        to.mirrorMode = mirrorMode;
        to.useItems = useItems;
        to.trackFeatures = trackFeatures;
        to.itemsEnabled = itemsEnabled.ToDictionary(e => e.Key, e => e.Value);
        to.timeAttackMode = timeAttackMode;
    }

    public override string ToString() {
        return JsonUtility.ToJson(this);
    }

    public void OnBeforeSerialize() {
        itemsEnabledKeys = new();
        itemsEnabledValues = new();

        if (itemsEnabled is null) {
            return;
        }
        foreach (var kv in itemsEnabled) {
            itemsEnabledKeys.Add(kv.Key);
            itemsEnabledValues.Add(kv.Value);
        }
    }

    public void OnAfterDeserialize() {
        if (itemsEnabledKeys.Count != itemsEnabledValues.Count) {
            throw new Exception("key and value counts are mismatched");
        }

        itemsEnabled = new();
        for (int i = 0; i < itemsEnabledKeys.Count; i++) {
            itemsEnabled[itemsEnabledKeys[i]] = itemsEnabledValues[i];
        }
    }
}


