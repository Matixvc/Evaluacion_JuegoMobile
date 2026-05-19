using UnityEngine;
public static class FruitTapHandler
{
    public static bool TryCollect(Collider collider)
    {
        if (collider == null) return false;
        FruitObject fruit = collider.GetComponentInParent<FruitObject>();
        if (fruit == null || fruit.data == null || fruit.isCollected) return false;
        if (GameManager.Instance == null) return false;
        collider.enabled = false;
        fruit.Collect();
        return true;
    }
}