using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance { get; private set; }

    [Header("Paneles")]
    public GameObject treePanel;
    public GameObject blenderPanel;
    public GameObject shopPanel;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start() => ShowTree();

    public void ShowTree()
    {
        treePanel.SetActive(true);
        blenderPanel.SetActive(false);
        shopPanel.SetActive(false);
    }

    public void ShowBlender()
    {
        treePanel.SetActive(false);
        blenderPanel.SetActive(true);
        shopPanel.SetActive(false);

        // Refresca UI al entrar

        FindFirstObjectByType<InventoryUI>()?.Refresh();
        //FindObjectOfType<InventoryUI>()?.Refresh();
    }

    public void ShowShop()
    {
        treePanel.SetActive(false);
        blenderPanel.SetActive(false);
        shopPanel.SetActive(true);

        // Refresca UI al entrar
        FindFirstObjectByType<ShopController>()?.Refresh();
        //FindObjectOfType<ShopController>()?.Refresh();
    }
}