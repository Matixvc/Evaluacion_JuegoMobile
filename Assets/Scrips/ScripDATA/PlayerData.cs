using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "FruitShake/Player Data")]
public class PlayerData : ScriptableObject
{
    public int gold = 0;
    public int score = 0;

    public bool SpendGold(int amount)
    {
        if (gold < amount) return false;
        gold -= amount;
        return true;
    }

    public void AddGold(int amount) => gold += amount;
    public void AddScore(int amount) => score += amount;
    public void Reset() { gold = 0; score = 0; }
}
