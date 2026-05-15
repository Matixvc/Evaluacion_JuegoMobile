using System;
using System.Collections.Generic;
using UnityEngine;

public static class SaveManager
{
    const string PlayerKey = "FruitShake_Player";
    const string InventoryKey = "FruitShake_Inventory";

    [Serializable]
    public class PlayerSave
    {
        public int gold;
        public int score;
    }

    [Serializable]
    public class InventorySave
    {
        public List<FruitSaveEntry> entries = new();
    }

    [Serializable]
    public class FruitSaveEntry
    {
        public string fruitName;
        public int count;
    }

    public static void SavePlayer(PlayerData data)
    {
        if (data == null) return;
        var save = new PlayerSave { gold = data.gold, score = data.score };
        PlayerPrefs.SetString(PlayerKey, JsonUtility.ToJson(save));
        PlayerPrefs.Save();
    }

    public static void LoadPlayer(PlayerData data)
    {
        if (data == null || !PlayerPrefs.HasKey(PlayerKey)) return;
        var save = JsonUtility.FromJson<PlayerSave>(PlayerPrefs.GetString(PlayerKey));
        data.gold = save.gold;
        data.score = save.score;
    }

    public static void SaveInventory(InventoryData inventory)
    {
        if (inventory == null) return;

        var save = new InventorySave();
        foreach (var entry in inventory.fruits)
        {
            if (entry.fruit == null) continue;
            save.entries.Add(new FruitSaveEntry
            {
                fruitName = entry.fruit.fruitName,
                count = entry.count
            });
        }

        PlayerPrefs.SetString(InventoryKey, JsonUtility.ToJson(save));
        PlayerPrefs.Save();
    }

    public static void LoadInventory(InventoryData inventory, FruitData[] catalog)
    {
        if (inventory == null) return;

        inventory.Clear();
        if (!PlayerPrefs.HasKey(InventoryKey) || catalog == null) return;

        var save = JsonUtility.FromJson<InventorySave>(PlayerPrefs.GetString(InventoryKey));
        foreach (var entry in save.entries)
        {
            FruitData fruit = FindFruit(catalog, entry.fruitName);
            if (fruit == null) continue;

            for (int i = 0; i < entry.count; i++)
                inventory.AddFruit(fruit);
        }
    }

    static FruitData FindFruit(FruitData[] catalog, string name)
    {
        foreach (var fruit in catalog)
        {
            if (fruit != null && fruit.fruitName == name)
                return fruit;
        }
        return null;
    }
}
