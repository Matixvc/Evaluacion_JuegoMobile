using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "FruitShake/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Economía y Progreso")]
    public int gold = 0;
    public int score = 0;

    [Header("Configuración de Carrera")]
    // ¡NUEVO!: Guarda cuál es la manzana que el jugador seleccionó para correr
    public FruitData equippedApple;

    public bool SpendGold(int amount)
    {
        if (gold < amount) return false;
        gold -= amount;
        return true;
    }

    public void AddGold(int amount) => gold += amount;
    public void AddScore(int amount) => score += amount;

    // ¡NUEVO!: Función para equipar la manzana más rápida en la tienda
    public void EquipApple(FruitData newApple)
    {
        equippedApple = newApple;
        Debug.Log($"Manzana equipada con éxito: {newApple.fruitName} (Angular Drag: {newApple.angularDrag})");
    }

    public void Reset()
    {
        gold = 0;
        score = 0;
        equippedApple = null; // También limpiamos la manzana al reiniciar
    }
}