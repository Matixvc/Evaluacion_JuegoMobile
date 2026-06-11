using UnityEngine;
using TMPro;

public class FruitCollector : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI scoreText;

    private Camera _cam;

    void Start() => _cam = Camera.main;

    void Update()
    {
        if (!TryGetTapPosition(out Vector2 screenPos)) return;

        Ray ray = _cam.ScreenPointToRay(screenPos);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        if (!FruitTapHandler.TryCollect(hit.collider)) return;
        UpdateScoreUI();
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

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {GameManager.Instance.playerData.score}";
    }
}