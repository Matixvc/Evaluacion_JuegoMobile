using UnityEngine;

[CreateAssetMenu(fileName = "NewTree", menuName = "FruitShake/Tree Data")]
public class TreeData : ScriptableObject
{
    [Header("Identidad")]
    public string treeName;
    public GameObject prefab;

    [Header("Frutas que produce")]
    public FruitData[] possibleFruits; // asigna Normal, Rotten y Bonus de este árbol

    [Header("Desbloqueo")]
    public int unlockCost;
    public bool isUnlocked = false;
}