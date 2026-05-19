using UnityEngine;
using System.Collections;

public class TreeController : MonoBehaviour
{
    public ParticleSystem particulasHojasImpacto; 

    [Header("Referencias")]
    public FruitSpawner fruitSpawner;
    public Renderer treeRenderer;

    [Header("Config")]
    public float tapCooldown = 1f;
    private float _lastTapTime = -99f;
    private Camera _cam;

    [Header("Shake")]
    public float shakeDuration = 0.6f;
    public float shakeDecay = 2.5f;
    private Material _mat;

    private Coroutine _shakeCoroutine;
    void Start()
    {
        _cam = Camera.main;
        _mat = treeRenderer.material;
        _mat.SetFloat("_ShakeIntensity", 0f);
    }

    void Update()
    {
    

        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        Ray ray = _cam.ScreenPointToRay(touch.position);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;
        if (hit.collider.GetComponent<FruitObject>() != null) return;
        if (hit.collider.gameObject != gameObject) return;
        if (Time.time - _lastTapTime < tapCooldown) return;

        _lastTapTime = Time.time;
        OnTapped();
    }


    void OnTapped()
    {
        if (_shakeCoroutine != null) StopCoroutine(_shakeCoroutine);
        _shakeCoroutine = StartCoroutine(ShakeRoutine());

        if (particulasHojasImpacto != null) particulasHojasImpacto.Play();

        fruitSpawner.SpawnFruit();

#if UNITY_ANDROID && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }
    IEnumerator ShakeRoutine()
    {
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

    // Editor fallback (mouse)
    static bool TryGetTapPosition(out Vector2 screenPos)
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase != TouchPhase.Began) { screenPos = default; return false; }
            screenPos = touch.position;
            return true;
        }
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) { screenPos = Input.mousePosition; return true; }
#endif
        screenPos = default;
        return false;
    }

}
