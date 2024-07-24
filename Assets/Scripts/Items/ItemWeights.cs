using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Weights", menuName = "Item Weights")]
public class ItemWeights : ScriptableObject {
    public ItemWeightsRecord[] records = new ItemWeightsRecord[] {
        new(ItemType.BoostTank),
        new(ItemType.Missile),
        new(ItemType.LaserDisc),
        new(ItemType.Freezer),
        new(ItemType.SpikeTrap),
    };
}

[Serializable]
public class ItemWeightsRecord {
    public ItemType itemType { get; private set; }
    public int firstPlace = 0;
    public int secondPlace = 0;
    public int thirdPlace = 0;
    public int fourthPlace = 0;
    public int fifthPlace = 0;
    public int sixthPlace = 0;
    public int seventhPlace = 0;
    public int eighthPlace = 0;

    public ItemWeightsRecord(ItemType type) {
        itemType = type;
    }

    public string Name => itemType.ToString();
    public int ItemType => (int)itemType;

    public int GetPlacementWeight(int placement) => placement switch {
        1 => firstPlace,
        2 => secondPlace,
        3 => thirdPlace,
        4 => fourthPlace,
        5 => fifthPlace,
        6 => sixthPlace,
        7 => seventhPlace,
        8 => eighthPlace,
        _ => 0,
    };
}