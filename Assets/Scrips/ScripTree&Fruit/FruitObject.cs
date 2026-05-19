using UnityEngine;

public class FruitObject : MonoBehaviour
{
    [Header("Data")]
    public FruitData data;

    [Header("Lifetime (configurar en Inspector o via FruitData)")]
    public float lifetimeNormal = 10f;
    public float lifetimeGold = 5f;
    public float lifetimeRotten = 7f;
    public float destroyBelowY = -10f;

    public bool isCollected { get; private set; }

    private bool _onGround = false;
    private float _groundTimer = 0f;
    private float _lifetime = 10f;

    void Start()
    {
        if (data == null) return;
        _lifetime = data.type switch
        {
            FruitType.Bonus => lifetimeGold,
            FruitType.Rotten => lifetimeRotten,
            _ => lifetimeNormal
        };
    }

    void Update()
    {
        if (transform.position.y < destroyBelowY)
        { 
            Destroy(gameObject);
            return;
        }

        if (!_onGround) return;

        _groundTimer += Time.deltaTime;
        if (_groundTimer >= _lifetime)
            Expire();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_onGround) return; // ya procesado
        if (!collision.gameObject.CompareTag("Suelo")) return;
        _onGround = true;
        _groundTimer = 0f;
    }
    // Llamado al tocar la fruta
    public void Collect()
    {
        if (isCollected) return;
        isCollected = true;

        if (data == null) { Destroy(gameObject); return; }

        switch (data.type)
        {
            case FruitType.Normal:
                GameManager.Instance.inventory.AddFruit(data);
                GameManager.Instance.playerData.AddScore(data.scoreValue);
                break;

            case FruitType.Bonus:
                GameManager.Instance.inventory.AddGoldFruit(data);
                GameManager.Instance.playerData.AddScore(data.scoreValue);
                break;

            case FruitType.Rotten:
                // 0 puntos al tocar — solo destruir
                break;
        }

        GameManager.Instance.SaveProgress();
        Destroy(gameObject);
    }

    // Llamado al expirar sin ser tocada
    void Expire()
    {
        if (isCollected) return;

        if (data != null && data.type == FruitType.Rotten)
        {
            GameManager.Instance?.playerData.AddScore(data.scoreValue); // scoreValue negativo
            GameManager.Instance?.SaveProgress();
        }

        Destroy(gameObject);
    }
}