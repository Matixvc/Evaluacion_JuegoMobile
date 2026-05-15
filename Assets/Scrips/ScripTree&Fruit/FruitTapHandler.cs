using UnityEngine;

public static class FruitTapHandler
{
    public static bool TryCollect(Collider collider)
    {
        if (collider == null) return false;

        FruitObject fruit = collider.GetComponentInParent<FruitObject>();
        if (fruit == null || fruit.data == null || fruit.isCollected) return false;
        if (GameManager.Instance == null) return false;

        fruit.isCollected = true;
        collider.enabled = false;

        GameManager.Instance.CollectFruit(fruit.data);
        Object.Destroy(fruit.gameObject);
        return true;
    }
}
