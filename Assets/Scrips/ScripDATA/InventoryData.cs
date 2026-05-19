using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Inventory", menuName = "FruitShake/Inventory")]
public class InventoryData : ScriptableObject
{
    [System.Serializable]
    public class FruitEntry
    {
        public FruitData fruit;
        public int count;
    }

    [Header("Inventario separado")]
    public List<FruitEntry> normalFruits = new();
    public List<FruitEntry> goldFruits = new();
    //public List<FruitEntry> fruits = new();
    // ── Normal ──────────────────────────────────────────
    public void AddFruit(FruitData fruit) => AddToList(normalFruits, fruit);
    public void RemoveFruit(FruitData fruit) => RemoveFromList(normalFruits, fruit);
    // ── Gold ────────────────────────────────────────────
    public void AddGoldFruit(FruitData fruit) => AddToList(goldFruits, fruit);
    public void RemoveGoldFruit(FruitData fruit) => RemoveFromList(goldFruits, fruit);
    // ── Utilidades ──────────────────────────────────────
    public void Clear() { normalFruits.Clear(); goldFruits.Clear(); }

    public bool HasFruits() => normalFruits.Count > 0 || goldFruits.Count > 0;

    public int TotalNormal => normalFruits.Sum(e => e.count);
    public int TotalGold => goldFruits.Sum(e => e.count);

    public int CalculateValue()
    {
        int total = 0;
        foreach (var e in normalFruits) total += e.fruit.smoothieValue * e.count;
        foreach (var e in goldFruits) total += e.fruit.smoothieValue * e.count;
        return total;
    }

    // ── Privados ────────────────────────────────────────
    void AddToList(List<FruitEntry> list, FruitData fruit)
    {
        var entry = list.FirstOrDefault(e => e.fruit == fruit);
        if (entry != null) entry.count++;
        else list.Add(new FruitEntry { fruit = fruit, count = 1 });
    }

    void RemoveFromList(List<FruitEntry> list, FruitData fruit)
    {
        var entry = list.FirstOrDefault(e => e.fruit == fruit);
        if (entry == null) return;
        entry.count--;
        if (entry.count <= 0) list.Remove(entry);
    }

    //public void AddFruit(FruitData fruit)
    //{
    //    var entry = fruits.FirstOrDefault(e => e.fruit == fruit);
    //    if (entry != null) entry.count++;
    //    else fruits.Add(new FruitEntry { fruit = fruit, count = 1 });
    //}

    //public void RemoveFruit(FruitData fruit)
    //{
    //    var entry = fruits.FirstOrDefault(e => e.fruit == fruit);
    //    if (entry == null) return;
    //    entry.count--;
    //    if (entry.count <= 0) fruits.Remove(entry);
    //}


    //public void Clear() => fruits.Clear();

    //public bool HasFruits() => fruits.Count > 0;

    //public int CalculateValue()
    //{
    //    int total = 0;
    //    foreach (var entry in fruits)
    //        total += entry.fruit.smoothieValue * entry.count;
    //    return total;
    //}
}