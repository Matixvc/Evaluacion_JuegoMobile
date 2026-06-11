using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance { get; private set; }

    [Header("Paneles de la UI")]
    public GameObject inventoryPanel; // Arrastra aquí tu panel de Inventario en el Inspector

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void ShowTree()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
        }
    }
}