using System;
using System.Collections.Generic;
using UnityEngine;

public static class SaveManager
{
    const string PlayerKey = "FruitShake_Player";
    const string InventoryKey = "FruitShake_Inventory";

    [Serializable]
    public class PlayerSave { public int gold; public int score; }

    [Serializable]
    public class InventorySave
    {
        public List<FruitSaveEntry> entries = new List<FruitSaveEntry>();
    }

    [Serializable]
    public class FruitSaveEntry
    {
        public string fruitName;
        public float topSpeed;
        public float angularDrag;
        public int shopValue;
    }

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

        // Guardamos las estadísticas exactas e individuales de cada manzana en la lista plana
        foreach (var fruit in inventory.collectedFruits)
        {
            if (fruit != null)
            {
                save.entries.Add(new FruitSaveEntry
                {
                    fruitName = fruit.fruitName,
                    topSpeed = fruit.topSpeed,
                    angularDrag = fruit.angularDrag,
                    shopValue = fruit.shopValue
                });
            }
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

        foreach (var e in save.entries)
        {
            FruitData baseFruit = Find(catalog, e.fruitName);
            if (baseFruit == null) continue;

            // Reconstruimos la manzana única con los dotes físicos exactos que tenía guardados
            FruitData uniqueFruit = ScriptableObject.Instantiate(baseFruit);
            uniqueFruit.name = baseFruit.name;
            uniqueFruit.topSpeed = e.topSpeed;
            uniqueFruit.angularDrag = e.angularDrag;
            uniqueFruit.shopValue = e.shopValue;

            inventory.AddFruit(uniqueFruit);
        }
    }

    static FruitData Find(FruitData[] catalog, string name)
    {
        foreach (var f in catalog) if (f != null && f.fruitName == name) return f;
        return null;
    }
}