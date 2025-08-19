using System.Collections.Generic;
using System;

[Serializable]
public class ItemSettings
{
    public List<ItemSetting> settings = new();

    public ItemSetting GetItem(ItemID item) => settings.Find(i => i.id == item);

    public static ItemSettings Default 
    {
        get 
        {
            ItemSettings settings = new();
            ItemSetting temp = new();
            temp.power = ItemPower.Normal;
            temp.weight = 1;
            foreach (ItemID id in (ItemID[])Enum.GetValues(typeof(ItemID))) 
            {
                temp.id = id;
                settings.settings.Add(temp);
            }
            return settings;
        }
    }
}