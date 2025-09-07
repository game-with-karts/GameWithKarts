using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public sealed class ItemSettingsScreen : MonoBehaviour {
    private SingleItemSettings[] items;
    public SingleItemSettings[] Items {
        get {
            if (items is null) {
                items = GetComponentsInChildren<SingleItemSettings>();
            }
            return items;
        }
    }

    private Dictionary<ItemType, bool> settings;
    public Dictionary<ItemType, bool> Settings {
        get {
            settings = new();
            foreach (var item in Items) {
                settings[item.Type] = item.IsOn;
            }
            return settings;
        }
        set {
            settings = value;
            foreach (var item in Items) {
                if (value.ContainsKey(item.Type)) {
                    item.Init(settings[item.Type]);
                    continue;
                }
                item.Init(true);
            }
        }
    }
}
