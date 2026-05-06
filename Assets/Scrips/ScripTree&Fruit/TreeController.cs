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
        animator.SetTrigger("Shake");

#if UNITY_ANDROID && !UNITY_EDITOR
        Handheld.Vibrate();
#endif

        fruitSpawner.SpawnFruit();
    }
}