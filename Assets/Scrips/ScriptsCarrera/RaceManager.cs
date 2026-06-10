using UnityEngine;
using System.Collections.Generic;
using System.Collections; // Necesario para usar Corrutinas (IEnumerator)

public class RaceManager : MonoBehaviour
{
    [Header("Puntos de Salida")]
    [Tooltip("Arrastra aquí los Transforms de las rampas de salida (Rampa 1, Rampa 2, etc.)")]
    public List<Transform> spawnPoints;

    [Header("Configuración de Rivales")]
    [Tooltip("Arrastra aquí el PREFAB de tu Manzana Normal (el que tiene el script FruitObject)")]
    public GameObject manzanaNormalPrefab;

    [Header("Configuración por Script (Escala y Física)")]
    [Range(0.1f, 2f)]
    [Tooltip("Tamaño relativo que se le aplicará a todas las manzanas en la pista de forma automática")]
    public float escalaManzana = 0.5f;

    [Tooltip("Si está activo, congela el movimiento horizontal (X) para que no se caigan de lado en la salida")]
    public bool congelarEjeXAlInicio = false;

    [Header("Valores Razonables para Competidoras Normales (X = Mínimo, Y = Máximo)")]
    public Vector2 rangoVelocidadNormal = new Vector2(12f, 16f);
    public Vector2 rangoDragNormal = new Vector2(0.15f, 0.35f);

    [Header("Fuerza de Salida")]
    [Tooltip("Pequeño impulso inicial hacia adelante para asegurar que empiecen a rodar por la rampa Bézier")]
    public float impulsoInicial = 5f;

    [HideInInspector]
    public GameObject jugadorInstanciado;

    // Lista interna para manejar los Rigidbodies de todos los corredores
    private List<Rigidbody> _listaCorredores = new List<Rigidbody>();

    void Start()
    {
        DesplegarCompetidores();
    }

    void DesplegarCompetidores()
    {
        if (spawnPoints == null || spawnPoints.Count < 2)
        {
            Debug.LogError("<b>[RaceManager]</b> Por favor asigna al menos 2 o más SpawnPoints en el Inspector.");
            return;
        }

        if (manzanaNormalPrefab == null)
        {
            Debug.LogError("<b>[RaceManager]</b> No has asignado el prefab de la manzana normal en el Inspector.");
            return;
        }

        // -------------------------------------------------------------
        // 1. INSTANCIAR A LA CAMPEONA DEL JUGADOR
        // -------------------------------------------------------------
        FruitData manzanaJugadorData = GameManager.Instance != null ? GameManager.Instance.selectedRunnerFruit : null;

        if (manzanaJugadorData != null)
        {
            GameObject prefabJugador = manzanaJugadorData.prefab != null ? manzanaJugadorData.prefab : manzanaNormalPrefab;

            jugadorInstanciado = Instantiate(prefabJugador, spawnPoints[0].position, spawnPoints[0].rotation);
            jugadorInstanciado.name = "[JUGADOR] " + manzanaJugadorData.fruitName;

            // 👉 1. APLICAR ESCALA POR SCRIPT
            jugadorInstanciado.transform.localScale = Vector3.one * escalaManzana;

            // 👉 2. ELIMINAR EL HIJO "SistemaParticulas"
            EliminarEfectoParticulas(jugadorInstanciado);

            if (jugadorInstanciado.TryGetComponent(out FruitObject fObject))
            {
                fObject.InitializeFruit(manzanaJugadorData);
            }

            // 👉 3. PREPARAR RIGIDBODY EN ESTADO "QUIETO"
            if (jugadorInstanciado.TryGetComponent(out Rigidbody rbJugador))
            {
                PrepararFisicaInicial(rbJugador);
                _listaCorredores.Add(rbJugador); // Guardamos para la largada
            }

            Debug.Log($"<color=green><b>[Carrera]</b></color> Jugador listo: {manzanaJugadorData.fruitName}");
        }

        // -------------------------------------------------------------
        // 2. GENERAR RIVALES (Solo manzanas normales con stats aleatorias)
        // -------------------------------------------------------------
        for (int i = 1; i < spawnPoints.Count; i++)
        {
            GameObject rivalInstanciado = Instantiate(manzanaNormalPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
            rivalInstanciado.name = $"[RIVAL] Manzana Normal Bot {i}";

            // 👉 1. APLICAR ESCALA POR SCRIPT
            rivalInstanciado.transform.localScale = Vector3.one * escalaManzana;

            // 👉 2. ELIMINAR EL HIJO "SistemaParticulas"
            EliminarEfectoParticulas(rivalInstanciado);

            FruitData statsBot = ScriptableObject.CreateInstance<FruitData>();
            statsBot.fruitName = $"Rival Normal #{i}";
            statsBot.isGoldenFruit = false;

            statsBot.topSpeed = Random.Range(rangoVelocidadNormal.x, rangoVelocidadNormal.y);
            statsBot.angularDrag = Random.Range(rangoDragNormal.x, rangoDragNormal.y);

            if (rivalInstanciado.TryGetComponent(out FruitObject fObjectRival))
            {
                fObjectRival.InitializeFruit(statsBot);
            }

            // 👉 3. PREPARAR RIGIDBODY EN ESTADO "QUIETO"
            if (rivalInstanciado.TryGetComponent(out Rigidbody rbRival))
            {
                PrepararFisicaInicial(rbRival);
                _listaCorredores.Add(rbRival); // Guardamos para la largada
            }
        }

        // -------------------------------------------------------------
        // 3. INICIAR CUENTA REGRESIVA DE 3 SEGUNDOS
        // -------------------------------------------------------------
        StartCoroutine(CuentaRegresivaCarrera());
    }

    /// <summary>
    /// Busca y destruye permanentemente el objeto hijo llamado SistemaParticulas
    /// </summary>
    void EliminarEfectoParticulas(GameObject manzanaGo)
    {
        Transform hijoParticulas = manzanaGo.transform.Find("SistemaParticulas");
        if (hijoParticulas != null)
        {
            // Lo destruimos de inmediato para que no consuma recursos ni genere efectos
            Destroy(hijoParticulas.gameObject);
        }
    }

    /// <summary>
    /// Deja la manzana flotando quieta en el aire cancelando la gravedad al arrancar
    /// </summary>
    void PrepararFisicaInicial(Rigidbody rb)
    {
        rb.isKinematic = true; // Activar kinematic congela la física por completo

        if (congelarEjeXAlInicio)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionX;
        }
    }

    /// <summary>
    /// Corrutina que espera 3 segundos reales y da la orden de salida
    /// </summary>
    IEnumerator CuentaRegresivaCarrera()
    {
        Debug.Log("<color=yellow><b>[CARRERA]</b> 3...</color>");
        yield return new WaitForSeconds(1f);

        Debug.Log("<color=yellow><b>[CARRERA]</b> 2...</color>");
        yield return new WaitForSeconds(1f);

        Debug.Log("<color=yellow><b>[CARRERA]</b> 1...</color>");
        yield return new WaitForSeconds(1f);

        Debug.Log("<color=cyan><b>[CARRERA]</b> ¡¡LARGADA!! 🏁</color>");

        // Liberamos las físicas de todos al mismo tiempo
        for (int i = 0; i < _listaCorredores.Count; i++)
        {
            Rigidbody rb = _listaCorredores[i];
            if (rb != null)
            {
                rb.isKinematic = false; // Desactivamos kinematic para devolver el control a Unity

                // Le aplicamos el empujón inicial usando la rotación de su respectiva rampa
                rb.AddForce(spawnPoints[i].forward * impulsoInicial, ForceMode.VelocityChange);
            }
        }
    }
}