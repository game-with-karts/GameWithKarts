using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Item Weights", menuName = "Item Weights")]
public class ItemWeights : ScriptableObject {
    public List<ItemWeightsRecord> records = new() {
        new(ItemType.BoostTank),
        new(ItemType.Missile),
        new(ItemType.LaserDisc),
        new(ItemType.SpikeTrap),
        new(ItemType.Freezer),
    };

    void Awake() {
        Debug.Log("ItemWeights Awake");
        // records = InitList();
    }

    void OnEnable() {
        Debug.Log("ItemWeights OnEnable");
        ReconstructRecordsList(records);
    }

    public List<ItemWeightsRecord> InitList() {
        IEnumerable<ItemType> allTypes = Enum.GetValues(typeof(ItemType)).Cast<ItemType>();
        List<ItemWeightsRecord> newRecords = new(allTypes.Count());

        foreach (ItemType type in allTypes) {
            newRecords.Add(new ItemWeightsRecord(type));
        }

        return newRecords;
    }

    public void ReconstructRecordsList(List<ItemWeightsRecord> records) {
        List<ItemWeightsRecord> newRecords = InitList();
        foreach (ItemWeightsRecord record in records) {
            ItemWeightsRecord newRecord = newRecords.Where(nr => nr.itemType == record.itemType).Single();
            int idx = newRecords.IndexOf(newRecord);
            newRecord.placeWeights = record.placeWeights;
            newRecords[idx] = newRecord;
        }

        records = newRecords;
    }
}

[Serializable]
public class ItemWeightsRecord {
    public ItemType itemType { get; private set; }
    public int[] placeWeights;

    public ItemWeightsRecord(ItemType type) {
        itemType = type;
        placeWeights = new int[8] {
            1, 1, 1, 1,
            1, 1, 1, 1,
        };
    }

    public string Name => itemType.ToString();
    public int GetPlacementWeight(int placement) => placeWeights[placement - 1];
}