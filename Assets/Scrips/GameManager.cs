using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("ScriptableObjects")]
    public InventoryData inventory;
    public PlayerData playerData;
    public TreeData[] allTrees;

    [Header("Estado actual")]
    public TreeData currentTree;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ── Navegación ──────────────────────────────────────
    public void GoToTree() => SceneManager.LoadScene("TreeScene");
    public void GoToBlender() => SceneManager.LoadScene("BlenderScene");
    public void GoToShop() => SceneManager.LoadScene("ShopScene");

    // ── Fruta ───────────────────────────────────────────
    public void CollectFruit(FruitData fruit)
    {
        if (fruit.type == FruitType.Rotten)
        {
            playerData.AddScore(fruit.scoreValue); // scoreValue = -1
            return;
        }
        inventory.AddFruit(fruit);
        playerData.AddScore(fruit.scoreValue);
    }

    // ── Tienda ──────────────────────────────────────────
    public bool UnlockTree(TreeData tree)
    {
        if (!playerData.SpendGold(tree.unlockCost)) return false;
        tree.isUnlocked = true;
        return true;
    }

    public void SelectTree(TreeData tree) => currentTree = tree;
}