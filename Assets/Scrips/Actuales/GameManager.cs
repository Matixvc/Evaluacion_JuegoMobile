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

    [Header("Carrera (Manzana Seleccionada)")]
    [Tooltip("Aquí se guardará la manzana elegida para la carrera actual.")]
    public FruitData selectedRunnerFruit;

    [Header("Configuración de Escenas")]
    public string carreraSceneName = "EscenaCarrera"; // Cambia por el nombre exacto de tu escena de carreras

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerData.Reset();
        LoadProgress();
    }

    public void LoadProgress()
    {
        SaveManager.LoadPlayer(playerData);
        SaveManager.LoadInventory(inventory, allFruits);
        Debug.Log("<color=green><b>[GameManager]</b></color> Progreso cargado con éxito.");
    }

    public void SaveProgress()
    {
        SaveManager.SavePlayer(playerData);
        SaveManager.SaveInventory(inventory);
        Debug.Log("<color=green><b>[GameManager]</b></color> Progreso guardado en el dispositivo Android.");
    }

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
        SaveProgress();

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

    /// <summary>
    /// Selecciona la manzana campeona, vende el resto automáticamente y carga la carrera.
    /// </summary>
    public void SelectFruitForRace(FruitData chosenFruit)
    {
        if (chosenFruit == null) return;

        // 1. Guardamos la manzana elegida en memoria para que la escena de carrera la lea
        selectedRunnerFruit = chosenFruit;
        Debug.Log($"<color=yellow><b>[GameManager]</b></color> Campeona elegida: {chosenFruit.fruitName} (Velocidad UI: {chosenFruit.velocidadResumen:F0}%)");

        // 2. Procesamos la venta masiva de las manzanas NO elegidas
        int oroGanadoTotal = 0;

        foreach (FruitData f in inventory.collectedFruits)
        {
            // Si no es la que elegimos para correr, se vende
            if (f != chosenFruit)
            {
                oroGanadoTotal += f.shopValue;
            }
        }

        // 3. Sumar el dinero al monedero del jugador
        playerData.gold += oroGanadoTotal;
        Debug.Log($"<color=gold><b>[Tienda]</b></color> Vendiste el resto de manzanas por +{oroGanadoTotal} de oro. Oro total: {playerData.gold}");

        // 4. Limpiamos el inventario temporal de recolección (ya que unas se vendieron y otra fue a correr)
        inventory.collectedFruits.Clear();

        // 5. Guardamos los cambios de oro e inventario en Android de inmediato
        SaveProgress();

        // 6. Cerramos la UI de la tienda por si acaso
        ResetHarvestRound();

        // 7. ¡SALTAMOS A LA CARRERA!
        SceneManager.LoadScene(carreraSceneName);
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