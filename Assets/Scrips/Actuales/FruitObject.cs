using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FruitObject : MonoBehaviour
{
    [Header("Datos de la Manzana")]
    public FruitData data; // El ScriptableObject con las stats

    private Rigidbody _rb;
    private bool _hasTouchedGround = false;
    private bool _isInitialized = false;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Método público para inicializar la canica en la carrera.
    /// </summary>
    public void InitializeFruit(FruitData uniqueData)
    {
        data = uniqueData;

        if (data != null && _rb != null)
        {
            _rb.angularDamping = data.angularDrag;
            _isInitialized = true;
        }
    }

    void Start()
    {
        if (data != null && !_isInitialized)
        {
            InitializeFruit(data);
        }
    }

    /// <summary>
    /// ¡RECOLECCIÓN DIRECTA POR TOUCH! 
    /// Al tocar la manzana en el celular, se procesa su clon, va al inventario y se destruye.
    /// </summary>
    private void OnMouseDown()
    {
        // Si el GameManager existe y aún no alcanzamos el límite de la ronda...
        if (GameManager.Instance != null && !GameManager.Instance.IsHarvestLimitReached())
        {
            if (data != null)
            {
                // 1. Creamos el clon único con sus estadísticas aleatorias para la tienda
                FruitData uniqueClone = data.CreateUniqueClone();
                GameManager.Instance.inventory.collectedFruits.Add(uniqueClone);
            }

            // 2. Le sumamos +1 al contador de recolección
            GameManager.Instance.OnFruitCollected();

            // 3. La eliminamos de la escena porque ya está en la canasta
            Destroy(gameObject);

            Debug.Log("<color=cyan><b>[Recolección]</b></color> Manzana guardada exitosamente en el inventario.");
        }
    }

    void FixedUpdate()
    {
        // El límite de velocidad solo actúa si la manzana fue enviada a correr formalmente
        if (!_isInitialized || data == null) return;

        float velocidadActual = _rb.linearVelocity.magnitude;

        if (velocidadActual > data.topSpeed)
        {
            _rb.linearVelocity = _rb.linearVelocity.normalized * data.topSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            if (!_hasTouchedGround)
            {
                _hasTouchedGround = true;

                string nombre = data != null ? data.fruitName : "Desconocida";
                float drag = data != null ? data.angularDrag : 0f;

                Debug.Log($"La manzana {nombre} tocó la pista Bézier con un drag de {drag}");
            }
        }
    }
}