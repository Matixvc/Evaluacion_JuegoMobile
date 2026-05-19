using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("UI Referencias")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI inventoryText;

    private Camera _cam;

    void Start() => _cam = Camera.main;

    void Update()
    {
        UpdateHUD();
        HandleTap();
    }

    void UpdateHUD()
    {
        if (GameManager.Instance == null) return;

        if (scoreText != null)
            scoreText.text = $"Score: {GameManager.Instance.playerData.score}";

        if (goldText != null)
            goldText.text = $"Gold: {GameManager.Instance.playerData.gold}";

        if (inventoryText != null)
            inventoryText.text = $"Normal: {GameManager.Instance.inventory.TotalNormal}  Gold: {GameManager.Instance.inventory.TotalGold}";
    }

    void HandleTap()
    {
        if (!TryGetTapPosition(out Vector2 screenPos)) return;
        Ray ray = _cam.ScreenPointToRay(screenPos);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;
        FruitTapHandler.TryCollect(hit.collider);
    }

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