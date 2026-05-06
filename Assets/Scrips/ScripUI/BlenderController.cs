using UnityEngine;
using TMPro;

public class BlenderController : MonoBehaviour
{
    [Header("Config SHAKE")]
    public float shakeThreshold = 2.5f;
    public int shakesRequired = 5;
    public float shakeCooldown = 0.3f;

    [Header("UI")]
    public TextMeshProUGUI shakesText;
    public GameObject blendButton; // botón para vender

    private int _shakeCount = 0;
    private float _lastShakeTime = -99f;
    private bool _isReady = false;

    void Update()
    {
        if (_isReady) return;
        if (!GameManager.Instance.inventory.HasFruits()) return;

        // Detectar SHAKE con acelerómetro
        float acceleration = Input.acceleration.magnitude;
        if (acceleration < shakeThreshold) return;
        if (Time.time - _lastShakeTime < shakeCooldown) return;

        _lastShakeTime = Time.time;
        _shakeCount++;
        UpdateUI();

        if (_shakeCount >= shakesRequired)
            SmoothieReady();
    }

    void UpdateUI()
    {
        if (shakesText != null)
            shakesText.text = $"Shake: {_shakeCount}/{shakesRequired}";
    }

    void SmoothieReady()
    {
        _isReady = true;
        if (blendButton != null)
            blendButton.SetActive(true);
    }

    // Llamado por el botón Vender
    public void SellSmoothie()
    {
        int gold = GameManager.Instance.inventory.CalculateValue();
        GameManager.Instance.playerData.AddGold(gold);
        GameManager.Instance.inventory.Clear();

        _shakeCount = 0;
        _isReady = false;

        if (blendButton != null)
            blendButton.SetActive(false);

        UpdateUI();
    }

    public void ResetBlender()
    {
        _shakeCount = 0;
        _isReady = false;
        if (blendButton != null) blendButton.SetActive(false);
        UpdateUI();
    }
}