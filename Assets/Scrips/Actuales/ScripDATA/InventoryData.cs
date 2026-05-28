using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "FruitShake/Inventory")]
public class InventoryData : ScriptableObject
{
    [Header("Escudería de Manzanas Únicas")]
    // Lista plana: guarda cada manzana de forma individual con sus propias estadísticas
    public List<FruitData> collectedFruits = new List<FruitData>();

    public void AddFruit(FruitData uniqueFruit)
    {
        if (uniqueFruit == null) return;
        collectedFruits.Add(uniqueFruit);
    }

    public void RemoveFruit(FruitData fruit)
    {
        if (collectedFruits.Contains(fruit))
            collectedFruits.Remove(fruit);
    }

    public void Clear() => collectedFruits.Clear();
    public bool HasFruits() => collectedFruits.Count > 0;
    public int TotalFruits => collectedFruits.Count;

    public int CalculateValue()
    {
        int total = 0;
        foreach (var fruit in collectedFruits)
        {
            if (fruit != null) total += fruit.shopValue;
        }
        return total;
    }
}