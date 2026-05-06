using UnityEngine;

public enum FruitType { Normal, Rotten, Bonus }

[CreateAssetMenu(fileName = "NewFruit", menuName = "FruitShake/Fruit Data")]
public class FruitData : ScriptableObject
{
    [Header("Identidad")]
    public string fruitName;
    public GameObject prefab;
    public FruitType type;

    [Header("Spawn")]
    [Range(0f, 1f)] public float spawnWeight = 0.5f;

    [Header("Valores")]
    public int scoreValue;    // Normal: +1, Rotten: -1, Bonus: +3
    public int smoothieValue; // cuánto oro vale en batido
}