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

    [Header("Parámetro X (Límite de Recolección)")]
    public int maxHarvestLimit = 5;
    private int _currentHarvestCount = 0;

    [Header("UI del Canvas (Referencia Dinámica)")]
    public GameObject panelShopUI;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerData.Reset();
        LoadProgress(); // Llama a la carga estática de abajo
    }

    // --- FUNCIONES DE GUARDADO ESTÁTICAS PARA ANDROID ---
    public void LoadProgress()
    {
        // Llamada directa sin usar .Instance ya que tus métodos son estáticos
        SaveManager.LoadPlayer(playerData);
        SaveManager.LoadInventory(inventory, allFruits);
        Debug.Log("<color=green><b>[GameManager]</b></color> Progreso cargado con éxito.");
    }

    public void SaveProgress()
    {
        // Llamada directa sin usar .Instance
        SaveManager.SavePlayer(playerData);
        SaveManager.SaveInventory(inventory);
        Debug.Log("<color=green><b>[GameManager]</b></color> Progreso guardado en el dispositivo Android.");
    }
    // ---------------------------------------------------

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentHarvestCount = 0;
        BuscarPanelTiendaEnEscena();
    }

    private void BuscarPanelTiendaEnEscena()
    {
        panelShopUI = GameObject.Find("PanelShopUI");

        if (panelShopUI != null)
        {
            Transform panelFondo = panelShopUI.transform.Find("FondoTienda") ?? panelShopUI.transform.GetChild(0);
            if (panelFondo != null) panelFondo.gameObject.SetActive(false);
        }
    }

    public bool IsHarvestLimitReached()
    {
        return _currentHarvestCount >= maxHarvestLimit;
    }

    public void OnFruitCollected()
    {
        _currentHarvestCount++;
        Debug.Log($"<color=cyan><b>[GameManager]</b></color> Recolectadas: {_currentHarvestCount} / {maxHarvestLimit}");

        if (_currentHarvestCount >= maxHarvestLimit)
        {
            EndHarvestAndShowShop();
        }
    }

    private void EndHarvestAndShowShop()
    {
        SaveProgress(); // Llama al guardado estático de arriba de forma segura

        if (panelShopUI == null) BuscarPanelTiendaEnEscena();

        if (panelShopUI != null)
        {
            Transform panelFondo = panelShopUI.transform.Find("FondoTienda") ?? panelShopUI.transform.GetChild(0);
            if (panelFondo != null)
            {
                panelFondo.gameObject.SetActive(true);

                if (panelFondo.TryGetComponent(out InventoryUI uiInventario))
                {
                    uiInventario.Refresh();
                }
            }
            else
            {
                panelShopUI.SetActive(true);
            }
        }
    }

    public void ResetHarvestRound()
    {
        _currentHarvestCount = 0;
        if (panelShopUI != null)
        {
            Transform panelFondo = panelShopUI.transform.Find("FondoTienda") ?? panelShopUI.transform.GetChild(0);
            if (panelFondo != null) panelFondo.gameObject.SetActive(false);
            else panelShopUI.SetActive(false);
        }
    }
}