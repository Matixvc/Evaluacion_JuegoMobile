using UnityEngine;

public class RakeController : MonoBehaviour
{
    [Header("Config")]
    public float dragSpeed = 15f;

    private Camera _cam;
    private bool _isDragging;
    private Vector3 _offset;
    private float _zDepth;

    void Start() => _cam = Camera.main;

    void Update()
    {
        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began) TryStartDrag(touch.position);
        if (touch.phase == TouchPhase.Moved && _isDragging) DragRake(touch.position);
        if (touch.phase == TouchPhase.Ended) _isDragging = false;
    }

    void TryStartDrag(Vector2 screenPos)
    {
        Ray ray = _cam.ScreenPointToRay(screenPos);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;
        if (hit.collider.gameObject != gameObject) return;

        _isDragging = true;
        _zDepth = _cam.WorldToScreenPoint(transform.position).z;
        _offset = transform.position - GetWorldPoint(screenPos);
    }

    void DragRake(Vector2 screenPos)
    {
        transform.position = Vector3.Lerp(
            transform.position,
            GetWorldPoint(screenPos) + _offset,
            Time.deltaTime * dragSpeed
        );
    }

    Vector3 GetWorldPoint(Vector2 screenPos)
    {
        Vector3 pos = new Vector3(screenPos.x, screenPos.y, _zDepth);
        return _cam.ScreenToWorldPoint(pos);
    }

    void OnTriggerEnter(Collider other)
    {
        FruitObject fruit = other.GetComponent<FruitObject>();
        if (fruit == null) return;
        if (fruit.data.type != FruitType.Rotten) return;
        Destroy(fruit.gameObject);
    }
}
