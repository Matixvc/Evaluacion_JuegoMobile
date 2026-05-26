using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FruitObject : MonoBehaviour
{
    [Header("Data")]
    public FruitData data;

    [Header("Configuraci�n")]
    public float destroyBelowY = -10f; // Por si cae de la isla al vac�o

    public bool isCollected { get; private set; }

    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        if (data == null)
        {
            Debug.LogError($"Falta asignar el FruitData en {gameObject.name}");
            return;
        }

        // Aplicamos la fricci�n de rotaci�n �nica de esta manzana
        if (_rb != null)
        {
            _rb.angularDamping = data.angularDrag;
        }
    }

    void Update()
    {
        if (transform.position.y < destroyBelowY)
        {
            Destroy(gameObject);
            return;
        }
    }

    // Llamado al tocar la manzana con el Touch
    public void Collect()
    {
        if (isCollected) return;
        isCollected = true;

        if (data == null) { Destroy(gameObject); return; }

        // Guardamos la manzana en la lista �nica del inventario
        GameManager.Instance.inventory.AddFruit(data);

        // Guardamos el progreso en el SaveManager autom�ticamente
        GameManager.Instance.SaveProgress();

        Destroy(gameObject);
    }
}