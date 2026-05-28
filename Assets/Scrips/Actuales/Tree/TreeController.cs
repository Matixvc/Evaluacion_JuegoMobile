using UnityEngine;
using System.Collections;

public class TreeController : MonoBehaviour
{
    public ParticleSystem particulasHojasImpacto;

    [Header("Referencias")]
    public FruitSpawner fruitSpawner;
    public Renderer treeRenderer;

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

        if (_shakeCoroutine != null) StopCoroutine(_shakeCoroutine);
        _shakeCoroutine = StartCoroutine(ShakeRoutine());

        if (particulasHojasImpacto != null) particulasHojasImpacto.Play();

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