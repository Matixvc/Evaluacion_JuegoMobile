using UnityEngine;

[CreateAssetMenu(fileName = "NewFruit", menuName = "FruitShake/Fruit Data")]
public class FruitData : ScriptableObject
{
    [Header("Identidad")]
    public string fruitName;
    public GameObject prefab;
    public Sprite icon;

    [Header("Spawn (Probabilidad)")]
    [Range(0f, 1f)] public float spawnWeight = 0.5f; // ¡AQUÍ ESTÁ LA VARIABLE QUE FALTA!

    [Header("Estadísticas de Carrera")]
    public float topSpeed;        // Velocidad máxima que puede alcanzar
    public float angularDrag;     // Fricción de rotación (A menor valor, rueda MÁS RÁPIDO)
    public int shopValue;         // Cuánto oro vale si decides venderla en lugar de correr
    public int scoreValue;        // Puntos que otorga al recolectarla (por si tu GameManager aún lo pide)
}