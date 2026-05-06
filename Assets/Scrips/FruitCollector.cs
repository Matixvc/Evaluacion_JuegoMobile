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
        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        Ray ray = _cam.ScreenPointToRay(touch.position);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        FruitObject fruit = hit.collider.GetComponent<FruitObject>();
        if (fruit == null) return;

        OnFruitTapped(fruit);
    }

    void OnFruitTapped(FruitObject fruit)
    {
        GameManager.Instance.CollectFruit(fruit.data);
        UpdateScoreUI();
        Destroy(fruit.gameObject);
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {GameManager.Instance.playerData.score}";
    }
}