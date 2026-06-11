using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FruitObject : MonoBehaviour
{
    [Header("Data")]
    public FruitData data;

    [Header("Configuración")]
    public float destroyBelowY = -10f; // Por si cae de la isla al vacío

    [Header("Referencias de Efectos (Partículas)")]
    [Tooltip("Efecto de rastro mientras cae (se detiene al chocar).")]
    public ParticleSystem sistemaTrail;
    [Tooltip("Efecto de explosión de hojas/polvo al impactar el suelo.")]
    public ParticleSystem sistemaImpacto;
    [Tooltip("Brillos extra (solo para la fruta de oro).")]
    public ParticleSystem sistemaSparkles;

    public bool isCollected { get; private set; }

    private Rigidbody _rb;
    private Camera _cam;
    private bool _haImpactadoSuelo = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _cam = Camera.main; // Guardamos la cámara principal para el Touch

        if (data == null)
        {
            Debug.LogError($"Falta asignar el FruitData en {gameObject.name}");
            return;
        }

        // Aplicamos la fricción de rotación única de esta manzana
        if (_rb != null)
        {
            // Nota: En versiones nuevas de Unity se usa angularDamping, en anteriores angularDrag.
            // Dejamos angularDamping que es el que tenías en tu script original.
            _rb.angularDamping = data.angularDrag;
        }

        // Si es fruta normal y por error tiene sparkles en el prefab, los apagamos
        if (!data.isGoldenFruit && sistemaSparkles != null)
        {
            sistemaSparkles.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // 1. Destruir si cae al vacío
        if (transform.position.y < destroyBelowY)
        {
            Destroy(gameObject);
            return;
        }

        // 2. DETECCIÓN DE TOUCH: Si el jugador toca la pantalla
        if (TryGetTapPosition(out Vector2 tapPosition))
        {
            Ray ray = _cam.ScreenPointToRay(tapPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Si el rayo del dedo chocó exactamente con esta manzana
                if (hit.collider.gameObject == gameObject)
                {
                    Collect();
                }
            }
        }
    }

    // LÓGICA DE DETECCIÓN DE SUELO (Para activar tus partículas)
    void OnCollisionEnter(Collision collision)
    {
        // Recuerda ponerle el Tag "Suelo" al piso de tu escena
        if (collision.gameObject.CompareTag("Suelo") && !_haImpactadoSuelo)
        {
            _haImpactadoSuelo = true;

            if (sistemaTrail != null) sistemaTrail.Stop();
            if (sistemaImpacto != null) sistemaImpacto.Play();

            // Si es de oro, vibra el celular Android al caer al suelo
            if (data != null && data.isGoldenFruit)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Handheld.Vibrate();
#endif
                Debug.Log($"🌟 ¡Fruta de Oro {data.fruitName} aterrizó!");
            }
        }
    }

    // Llamado al tocar la manzana con el Touch
    public void Collect()
    {
        // Si ya alcanzamos las 5 manzanas de la ronda, bloqueamos la recolección
        if (GameManager.Instance != null && GameManager.Instance.IsHarvestLimitReached())
        {
            return;
        }

        if (isCollected) return;
        isCollected = true;

        if (data == null) { Destroy(gameObject); return; }

        if (GameManager.Instance != null && GameManager.Instance.inventory != null)
        {
            // Guardamos la manzana en el inventario
            GameManager.Instance.inventory.AddFruit(data);

            // Sumamos el puntaje/score del jugador
            GameManager.Instance.playerData.AddScore(data.scoreValue);

            // Avisamos al GameManager para que cuente la manzana (+1 de 5)
            GameManager.Instance.OnFruitCollected();
        }

        Destroy(gameObject);
    }

    // Lector de posición táctil exclusivo para Android / Editor
    private bool TryGetTapPosition(out Vector2 screenPos)
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                screenPos = touch.position;
                return true;
            }
            screenPos = default;
            return false;
        }
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            screenPos = Input.mousePosition;
            return true;
        }
#endif
        screenPos = default;
        return false;
    }
}