using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class RaceManager : MonoBehaviour
{
    [Header("Puntos de Salida")]
    public List<Transform> spawnPoints;

    [Header("Configuración de Rivales")]
    public GameObject manzanaNormalPrefab;

    [Header("Configuración por Script (Escala y Física)")]
    [Range(0.1f, 2f)]
    public float escalaManzana = 0.5f;
    public bool congelarEjeXAlInicio = false;

    [Header("Valores Razonables para Competidoras Normales")]
    public Vector2 rangoVelocidadNormal = new Vector2(12f, 16f);
    public Vector2 rangoDragNormal = new Vector2(0.15f, 0.35f);

    [Header("Fuerza de Salida")]
    public float impulsoInicial = 5f;

    [Header("Configuración de Escenas")]
    [Tooltip("Nombre exacto de la escena del árbol para regresar")]
    public string nombreEscenaArbol = "EscenaArbol";

    [HideInInspector]
    public GameObject jugadorInstanciado;

    private List<Rigidbody> _listaCorredores = new List<Rigidbody>();
    private bool _carreraFinalizada = false;

    void Start()
    {
        DesplegarCompetidores();
    }

    void DesplegarCompetidores()
    {
        if (spawnPoints == null || spawnPoints.Count < 2) return;
        if (manzanaNormalPrefab == null) return;

        // 1. INSTANCIAR JUGADOR
        FruitData manzanaJugadorData = GameManager.Instance != null ? GameManager.Instance.selectedRunnerFruit : null;

        if (manzanaJugadorData != null)
        {
            GameObject prefabJugador = manzanaJugadorData.prefab != null ? manzanaJugadorData.prefab : manzanaNormalPrefab;

            jugadorInstanciado = Instantiate(prefabJugador, spawnPoints[0].position, spawnPoints[0].rotation);
            jugadorInstanciado.name = "[JUGADOR] " + manzanaJugadorData.fruitName;
            jugadorInstanciado.transform.localScale = Vector3.one * escalaManzana;

            // 👉 ELIMINAR EL HIJO "SistemaParticulas"
            EliminarEfectoParticulas(jugadorInstanciado);

            // Le aseguramos que tenga el script de triggers pegado
            if (!jugadorInstanciado.GetComponent<RaceTriggers>()) jugadorInstanciado.AddComponent<RaceTriggers>();

            if (jugadorInstanciado.TryGetComponent(out FruitObject fObject)) fObject.InitializeFruit(manzanaJugadorData);
            if (jugadorInstanciado.TryGetComponent(out Rigidbody rbJugador))
            {
                PrepararFisicaInicial(rbJugador);
                _listaCorredores.Add(rbJugador);
            }
        }

        // 2. INSTANCIAR RIVALES
        for (int i = 1; i < spawnPoints.Count; i++)
        {
            GameObject rivalInstanciado = Instantiate(manzanaNormalPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
            rivalInstanciado.name = $"[RIVAL] Manzana Normal Bot {i}";
            rivalInstanciado.transform.localScale = Vector3.one * escalaManzana;

            // 👉 ELIMINAR EL HIJO "SistemaParticulas"
            EliminarEfectoParticulas(rivalInstanciado);

            // Le aseguramos que tenga el script de triggers pegado
            if (!rivalInstanciado.GetComponent<RaceTriggers>()) rivalInstanciado.AddComponent<RaceTriggers>();

            FruitData statsBot = ScriptableObject.CreateInstance<FruitData>();
            statsBot.fruitName = $"Rival Normal #{i}";
            statsBot.isGoldenFruit = false;
            statsBot.topSpeed = Random.Range(rangoVelocidadNormal.x, rangoVelocidadNormal.y);
            statsBot.angularDrag = Random.Range(rangoDragNormal.x, rangoDragNormal.y);

            if (rivalInstanciado.TryGetComponent(out FruitObject fObjectRival)) fObjectRival.InitializeFruit(statsBot);
            if (rivalInstanciado.TryGetComponent(out Rigidbody rbRival))
            {
                PrepararFisicaInicial(rbRival);
                _listaCorredores.Add(rbRival);
            }
        }

        StartCoroutine(CuentaRegresivaCarrera());
    }

    void PrepararFisicaInicial(Rigidbody rb)
    {
        rb.isKinematic = true;
        if (congelarEjeXAlInicio) rb.constraints = RigidbodyConstraints.FreezePositionX;
    }

    /// <summary>
    /// Busca y remueve el objeto de efectos si existe en el prefab instanciado.
    /// </summary>
    void EliminarEfectoParticulas(GameObject manzanaGo)
    {
        Transform hijoParticulas = manzanaGo.transform.Find("SistemaParticulas");
        if (hijoParticulas != null)
        {
            Destroy(hijoParticulas.gameObject);
        }
    }

    IEnumerator CuentaRegresivaCarrera()
    {
        yield return new WaitForSeconds(3f); // Cuenta de 3 segundos quieto

        for (int i = 0; i < _listaCorredores.Count; i++)
        {
            Rigidbody rb = _listaCorredores[i];
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddForce(spawnPoints[i].forward * impulsoInicial, ForceMode.VelocityChange);
            }
        }
    }

    // =================================================================
    // 💥 CONTROL DE EVENTOS: CAÍDAS Y METAS
    // =================================================================

    public void ProcesarCaida(GameObject manzana, bool esJugador)
    {
        if (_carreraFinalizada) return;

        if (esJugador)
        {
            StartCoroutine(FinalizarCarreraRutina(false, "¡Te caíste de la pista!"));
        }
        else
        {
            Debug.Log($"<color=white><b>[Mecánica]</b></color> {manzana.name} se eliminó por caer al vacío.");
            Destroy(manzana);
        }
    }

    public void ProcesarMeta(bool esJugador)
    {
        if (_carreraFinalizada) return;

        if (esJugador)
        {
            StartCoroutine(FinalizarCarreraRutina(true, "¡VICTORIA! Eres el rey de la rampa."));
        }
        else
        {
            StartCoroutine(FinalizarCarreraRutina(false, "¡DERROTA! Un rival cruzó la meta primero."));
        }
    }

    private IEnumerator FinalizarCarreraRutina(bool victoria, string mensaje)
    {
        _carreraFinalizada = true;

        // Congelamos el tiempo del juego para simular la pantalla fija de fin de juego
        Time.timeScale = 0.2f;

        if (victoria)
        {
            Debug.Log($"<color=green><b>{mensaje}</b></color> Ganas 5 monedas.");
            // 👉 LLAMADA OFICIAL AL MERCADO PERSISTENTE DE TU GAMEMANAGER
            if (GameManager.Instance != null) GameManager.Instance.ModificarOroDesdeCarrera(5);
        }
        else
        {
            Debug.Log($"<color=red><b>{mensaje}</b></color> Pierdes 3 monedas.");
            // 👉 LLAMADA OFICIAL AL MERCADO PERSISTENTE DE TU GAMEMANAGER
            if (GameManager.Instance != null) GameManager.Instance.ModificarOroDesdeCarrera(-3);
        }

        // Esperamos 3 segundos en tiempo real (ya que el timeScale está casi congelado)
        yield return new WaitForSecondsRealtime(3f);

        // Restauramos el tiempo original antes de cambiar de escena
        Time.timeScale = 1f;

        // Regresamos automáticamente al huerto a recolectar más manzanas
        SceneManager.LoadScene(nombreEscenaArbol);
    }
}