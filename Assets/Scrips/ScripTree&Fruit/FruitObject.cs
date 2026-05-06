using UnityEngine;

public class FruitObject : MonoBehaviour
{
    public FruitData data;

    // Destruirse si cae muy abajo
    public float destroyBelowY = -10f;

    void Update()
    {
        if (transform.position.y < destroyBelowY)
            Destroy(gameObject);
    }
}