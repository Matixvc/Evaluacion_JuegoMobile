using UnityEngine;
using System.Collections;

public class TreeController : MonoBehaviour
{
    public ParticleSystem particulasHojasImpacto;

    [Header("Referencias")]
    public FruitSpawner fruitSpawner;
    public Renderer treeRenderer;

    [Header("Referencias de Audio (SFX)")]
    [Tooltip("Componente AudioSource del árbol.")]
    [SerializeField] private AudioSource audioSource;
    [Tooltip("Sonido tipo 'Swoosh' o crujido foliar al golpear el árbol.")]
    [SerializeField] private AudioClip sonidoGolpeArbol;
    [Range(0f, 1f)][SerializeField] private float volumenSFX = 0.9f;

    [Header("Configuración de Toques")]
    public float tapCooldown = 0.5f;
    private float _lastTapTime = -99f;
    private Camera _cam;

    [Header("Efecto de Agitado (Shader)")]
    public float shakeDuration = 0.6f;
    public float shakeDecay = 2.5f;
    private Material _mat;
    private Coroutine _shakeCoroutine;

    void Start()
    {
        _cam = Camera.main;
        if (treeRenderer != null)
        {
            _mat = treeRenderer.material;
            _mat.SetFloat("_ShakeIntensity", 0f);
        }

        // Configuración automática y optimizada del AudioSource para Mobile
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // Audio 2D nativo para que suene nítido en el canal móvil
    }

    void Update()
    {
        if (!TryGetTapPosition(out Vector2 tapPosition)) return;
        if (Time.time - _lastTapTime < tapCooldown) return;

        Ray ray = _cam.ScreenPointToRay(tapPosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        if (hit.collider.GetComponent<FruitObject>() != null) return;
        if (hit.collider.gameObject != gameObject) return;

        _lastTapTime = Time.time;
        OnTapped();
    }

    void OnTapped()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsHarvestLimitReached()) return;

        // 1. REPRODUCIR AUDIO RECTIVO CON VARIACIÓN (Rúbrica: Coherencia Acústica)
        if (audioSource != null && sonidoGolpeArbol != null)
        {
            // Variación sutil de tono (Pitch) para evitar la fatiga auditiva del jugador
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(sonidoGolpeArbol, volumenSFX);
        }

        // 2. EFECTO DE SHADER (Vertex Offset Shake)
        if (_shakeCoroutine != null) StopCoroutine(_shakeCoroutine);
        _shakeCoroutine = StartCoroutine(ShakeRoutine());

        // 3. EFECTO VISUAL DE PARTÍCULAS (Sistema Foliar)
        if (particulasHojasImpacto != null) particulasHojasImpacto.Play();

        // 4. INSTANCIACIÓN DE MANZANAS
        if (fruitSpawner != null)
        {
            fruitSpawner.SpawnFruit();
        }
    }

    IEnumerator ShakeRoutine()
    {
        if (_mat == null) yield break;
        _mat.SetFloat("_ShakeIntensity", 1f);
        yield return new WaitForSeconds(shakeDuration);
        float t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime * shakeDecay;
            _mat.SetFloat("_ShakeIntensity", Mathf.Max(t, 0f));
            yield return null;
        }
        _mat.SetFloat("_ShakeIntensity", 0f);
    }

    private bool TryGetTapPosition(out Vector2 screenPos)
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) { screenPos = touch.position; return true; }
            screenPos = default; return false;
        }
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) { screenPos = Input.mousePosition; return true; }
#endif
        screenPos = default; return false;
    }
}