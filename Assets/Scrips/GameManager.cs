using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("ScriptableObjects")]
    public InventoryData inventory;
    public PlayerData playerData;
    public TreeData[] allTrees;
    public FruitData[] allFruits;

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
        LoadProgress();
    }

    void OnApplicationPause(bool paused)
    {
        if (paused) SaveProgress();
    }

    void OnApplicationQuit() => SaveProgress();

    public void LoadProgress()
    {
        SaveManager.LoadPlayer(playerData);
        SaveManager.LoadInventory(inventory, allFruits);
    }

    public void SaveProgress()
    {
        SaveManager.SavePlayer(playerData);
        SaveManager.SaveInventory(inventory);
    }

    // ── Navegación ──────────────────────────────────────
    public void GoToTree() => SceneManager.LoadScene("TreeScene");
    public void GoToBlender() => SceneManager.LoadScene("BlenderScene");
    public void GoToShop() => SceneManager.LoadScene("ShopScene");

    // ── Fruta ───────────────────────────────────────────
    public void CollectFruit(FruitData fruit)
    {
        if (fruit == null) return;

        if (fruit.type == FruitType.Rotten)
        {
            playerData.AddScore(fruit.scoreValue);
            SaveProgress();
            return;
        }

        inventory.AddFruit(fruit);
        playerData.AddScore(fruit.scoreValue);
        SaveProgress();
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