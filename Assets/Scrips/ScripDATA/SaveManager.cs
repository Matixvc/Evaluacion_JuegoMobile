using System;
using System.Collections.Generic;
using UnityEngine;

public static class SaveManager
{
    const string PlayerKey = "FruitShake_Player";
    const string InventoryKey = "FruitShake_Inventory";

    [Serializable] public class PlayerSave { public int gold; public int score; }

    [Serializable]
    public class InventorySave
    {
        public List<FruitSaveEntry> normalEntries = new();
        public List<FruitSaveEntry> goldEntries = new();
    }

    [Serializable]
    public class FruitSaveEntry { public string fruitName; public int count; }

    // ── Player ──────────────────────────────────────────
    public static void SavePlayer(PlayerData data)
    {
        if (data == null) return;
        PlayerPrefs.SetString(PlayerKey, JsonUtility.ToJson(new PlayerSave { gold = data.gold, score = data.score }));
        PlayerPrefs.Save();
    }

    public static void LoadPlayer(PlayerData data)
    {
        if (data == null || !PlayerPrefs.HasKey(PlayerKey)) return;
        var save = JsonUtility.FromJson<PlayerSave>(PlayerPrefs.GetString(PlayerKey));
        data.gold = save.gold;
        data.score = save.score;
    }

    // ── Inventory ───────────────────────────────────────
    public static void SaveInventory(InventoryData inventory)
    {
        if (inventory == null) return;
        var save = new InventorySave();

        foreach (var e in inventory.normalFruits)
            if (e.fruit != null) save.normalEntries.Add(new FruitSaveEntry { fruitName = e.fruit.fruitName, count = e.count });

        foreach (var e in inventory.goldFruits)
            if (e.fruit != null) save.goldEntries.Add(new FruitSaveEntry { fruitName = e.fruit.fruitName, count = e.count });

        PlayerPrefs.SetString(InventoryKey, JsonUtility.ToJson(save));
        PlayerPrefs.Save();
    }

    public static void LoadInventory(InventoryData inventory, FruitData[] catalog)
    {
        if (inventory == null) return;
        inventory.Clear();
        if (!PlayerPrefs.HasKey(InventoryKey) || catalog == null) return;

        var save = JsonUtility.FromJson<InventorySave>(PlayerPrefs.GetString(InventoryKey));

        foreach (var e in save.normalEntries)
        {
            FruitData fruit = Find(catalog, e.fruitName);
            if (fruit == null) continue;
            for (int i = 0; i < e.count; i++) inventory.AddFruit(fruit);
        }

        foreach (var e in save.goldEntries)
        {
            FruitData fruit = Find(catalog, e.fruitName);
            if (fruit == null) continue;
            for (int i = 0; i < e.count; i++) inventory.AddGoldFruit(fruit);
        }
    }

    static FruitData Find(FruitData[] catalog, string name)
    {
        foreach (var f in catalog) if (f != null && f.fruitName == name) return f;
        return null;
    }
}