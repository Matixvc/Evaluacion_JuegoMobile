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

    public List<FruitEntry> fruits = new();

    public void AddFruit(FruitData fruit)
    {
        var entry = fruits.FirstOrDefault(e => e.fruit == fruit);
        if (entry != null) entry.count++;
        else fruits.Add(new FruitEntry { fruit = fruit, count = 1 });
    }

    public void RemoveFruit(FruitData fruit)
    {
        var entry = fruits.FirstOrDefault(e => e.fruit == fruit);
        if (entry == null) return;
        entry.count--;
        if (entry.count <= 0) fruits.Remove(entry);
    }


    public void Clear() => fruits.Clear();

    public bool HasFruits() => fruits.Count > 0;

    public int CalculateValue()
    {
        int total = 0;
        foreach (var entry in fruits)
            total += entry.fruit.smoothieValue * entry.count;
        return total;
    }
}