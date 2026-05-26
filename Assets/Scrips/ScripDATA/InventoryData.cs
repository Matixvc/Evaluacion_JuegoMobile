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

    [Header("Inventario de Escudería")]
    public List<FruitEntry> collectedFruits = new(); // Lista única para todas tus manzanas

    // Añadir manzana al inventario
    public void AddFruit(FruitData fruit)
    {
        var entry = collectedFruits.FirstOrDefault(e => e.fruit == fruit);
        if (entry != null) entry.count++;
        else collectedFruits.Add(new FruitEntry { fruit = fruit, count = 1 });
    }

    // Quitar manzana del inventario
    public void RemoveFruit(FruitData fruit)
    {
        var entry = collectedFruits.FirstOrDefault(e => e.fruit == fruit);
        if (entry == null) return;
        entry.count--;
        if (entry.count <= 0) collectedFruits.Remove(entry);
    }

    // Utilidades
    public void Clear() => collectedFruits.Clear();

    public bool HasFruits() => collectedFruits.Count > 0;

    public int TotalFruits => collectedFruits.Sum(e => e.count);

    // Calcula el valor total de oro si decides vender tus manzanas en la Tienda
    public int CalculateValue()
    {
        int total = 0;
        foreach (var entry in collectedFruits)
        {
            if (entry.fruit != null)
            {
                total += entry.fruit.shopValue * entry.count;
            }
        }
        return total;
    }
}