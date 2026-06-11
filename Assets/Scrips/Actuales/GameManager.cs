using System.Collections.Generic;
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

<<<<<<< HEAD
    [Header("Carrera (Manzana Seleccionada)")]
    [Tooltip("Aquí se guardará la manzana elegida para la carrera actual.")]
    public FruitData selectedRunnerFruit;

    [Header("Configuración de Escenas")]
    public string carreraSceneName = "EscenaCarrera"; // Cambia por el nombre exacto de tu escena de carreras
=======
>>>>>>> parent of e4358f2 (UltimoAvanzes)
    void Awake()
    {
        // 1. Control de Singleton estricto
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

<<<<<<< HEAD
        // 2. ECONOMÍA E INVENTARIO: Limpieza absoluta de raíz
        // Si no existe el registro del jugador, limpiamos TODO (Billetera e Inventario viejo)
        if (!PlayerPrefs.HasKey("FruitShake_Player") || PlayerPrefs.GetInt("PrimeraVezJugando", 1) == 1)
        {
            // Limpieza de datos de jugador
            playerData.Reset();
            playerData.gold = 0;
            playerData.score = 0;

            // 🔥 LA CLAVE: Limpiamos la canasta de frutas física para que no arrastre fantasmas
            if (inventory != null)
            {
                inventory.collectedFruits.Clear();
            }

            PlayerPrefs.SetInt("PrimeraVezJugando", 0);

            // Forzamos al SaveManager a sobreescribir el disco con 0 monedas y 0 manzanas
            SaveManager.SavePlayer(playerData);
            SaveManager.SaveInventory(inventory);
            PlayerPrefs.Save();

            Debug.Log("<color=orange><b>[Economía]</b></color> ¡Billetera E INVENTARIO inicializados en 0 de forma absoluta!");
        }
        else
        {
            // Si ya hay un juego guardado real, cargamos de forma normal
            LoadProgress();
        }
=======
        playerData.Reset();
        LoadProgress(); // Llama a la carga estática de abajo
>>>>>>> parent of e4358f2 (UltimoAvanzes)
    }

    // --- FUNCIONES DE GUARDADO ESTÁTICAS PARA ANDROID ---
    public void LoadProgress()
    {
<<<<<<< HEAD
        // Corregido: Quitamos el .gameObject para que no tire error en la clase estática
        if (PlayerPrefs.HasKey("FruitShake_Player"))
        {
            SaveManager.LoadPlayer(playerData);
            SaveManager.LoadInventory(inventory, allFruits);
            Debug.Log($"<color=green><b>[GameManager]</b></color> Progreso cargado. Oro actual en memoria: {playerData.gold}");
        }
=======
        // Llamada directa sin usar .Instance ya que tus métodos son estáticos
        SaveManager.LoadPlayer(playerData);
        SaveManager.LoadInventory(inventory, allFruits);
        Debug.Log("<color=green><b>[GameManager]</b></color> Progreso cargado con éxito.");
>>>>>>> parent of e4358f2 (UltimoAvanzes)
    }

    public void SaveProgress()
    {
<<<<<<< HEAD
        if (playerData != null && inventory != null)
        {
            SaveManager.SavePlayer(playerData);
            SaveManager.SaveInventory(inventory);
            PlayerPrefs.Save(); // Asegura que se guarde físicamente en el disco
            Debug.Log($"<color=green><b>[GameManager]</b></color> Progreso guardado con éxito. Oro respaldado: {playerData.gold}");
        }
=======
        // Llamada directa sin usar .Instance
        SaveManager.SavePlayer(playerData);
        SaveManager.SaveInventory(inventory);
        Debug.Log("<color=green><b>[GameManager]</b></color> Progreso guardado en el dispositivo Android.");
>>>>>>> parent of e4358f2 (UltimoAvanzes)
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

<<<<<<< HEAD
    /// <summary>
    /// Selecciona la manzana campeona, vende el resto con valores fijos y duros (1 y 3) para evitar bugs del inspector.
    /// </summary>
    public void SelectFruitForRace(FruitData chosenFruit)
    {
        if (chosenFruit == null) return;

        // 1. Guardamos la manzana elegida en memoria para la carrera
        selectedRunnerFruit = chosenFruit;
        Debug.Log($"<color=yellow><b>[GameManager]</b></color> Campeona elegida: {chosenFruit.fruitName}");

        int oroGanadoTotal = 0;
        int manzanasContadas = 0;

        // 2. Filtro de seguridad estricto: máximo de manzanas legal del juego (5)
        List<FruitData> canastaRealYLimitada = new List<FruitData>();

        foreach (FruitData f in inventory.collectedFruits)
        {
            if (f != null)
            {
                canastaRealYLimitada.Add(f);
                manzanasContadas++;

                if (manzanasContadas >= maxHarvestLimit)
                {
                    break; // Freno de mano: no entran más de 5 manzanas al cálculo
                }
            }
        }

        Debug.Log($"<color=white><b>[Seguridad Tienda]</b></color> Procesando venta de {canastaRealYLimitada.Count} manzanas en canasta.");

        // 3. Procesamos la venta usando VALORES HARDCODED (Duros) para evitar que los ScriptableObjects traigan números corruptos
        for (int i = 0; i < canastaRealYLimitada.Count; i++)
        {
            FruitData frutaEnCanasta = canastaRealYLimitada[i];

            // Si es la manzana elegida para correr, nos saltamos su venta
            if (frutaEnCanasta == chosenFruit)
            {
                continue;
            }

            // 🌟 MATEMÁTICA BLINDADA: Evaluamos por su tipo o nombre, no por su variable corrupta
            if (frutaEnCanasta.isGoldenFruit || frutaEnCanasta.fruitName.Contains("Oro") || frutaEnCanasta.fruitName.Contains("Golden"))
            {
                oroGanadoTotal += 3; // Forzamos que la de oro valga 3
            }
            else
            {
                oroGanadoTotal += 1; // Forzamos que cualquier otra valga 1
            }
        }

        // 4. Inyectamos el dinero controlando que no use valores viejos de memoria
        playerData.gold += oroGanadoTotal;
        Debug.Log($"<color=gold><b>[Tienda]</b></color> Sumado por venta legal: +{oroGanadoTotal} de oro. Oro total actual: {playerData.gold}");

        // 5. Vaciamos por completo el inventario original
        inventory.collectedFruits.Clear();

        // 6. Guardamos los progresos de forma limpia
        SaveProgress();

        // 7. Cerramos UI y vamos a la carrera
        ResetHarvestRound();
        SceneManager.LoadScene(carreraSceneName);
    }

    /// <summary>
    /// Método público para modificar el dinero desde los Triggers de la Carrera al ganar (+5) o perder (-3).
    /// </summary>
    public void ModificarOroDesdeCarrera(int cantidad)
    {
        if (playerData != null)
        {
            playerData.gold += cantidad;

            // Evitamos que el oro baje de 0 si el jugador acumula muchas derrotas
            if (playerData.gold < 0) playerData.gold = 0;

            Debug.Log($"<color=orange><b>[Bucle de Carrera]</b></color> Transacción de carrera aplicada ({cantidad}). Nuevo total: {playerData.gold} de oro.");
            SaveProgress();
        }
    }

=======
>>>>>>> parent of e4358f2 (UltimoAvanzes)
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