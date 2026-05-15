using UnityEngine;

public class TreeController : MonoBehaviour
{
    [Header("Referencias")]
    public FruitSpawner fruitSpawner;
    public Animator animator;

    [Header("Config")]
    public float tapCooldown = 1f;

    private float _lastTapTime = -99f;
    private Camera _cam;

    void Start() => _cam = Camera.main;

    void Update()
    {
        if (!TryGetTapPosition(out Vector2 screenPos)) return;

        Ray ray = _cam.ScreenPointToRay(screenPos);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        if (FruitTapHandler.TryCollect(hit.collider))
            return;

        if (hit.collider.gameObject == gameObject || hit.collider.CompareTag("Arbol"))
        {
            if (Time.time - _lastTapTime >= tapCooldown)
            {
                _lastTapTime = Time.time;
                OnTapped();
            }
        }
    }

    static bool TryGetTapPosition(out Vector2 screenPos)
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase != TouchPhase.Began)
            {
                screenPos = default;
                return false;
            }
            screenPos = touch.position;
            return true;
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

    void OnTapped()
    {
        animator.SetTrigger("Shake");
        fruitSpawner.SpawnFruit();
#if UNITY_ANDROID && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }
}
