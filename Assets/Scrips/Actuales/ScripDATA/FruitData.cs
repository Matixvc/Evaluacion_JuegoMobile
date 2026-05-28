using UnityEngine;

[CreateAssetMenu(fileName = "NewFruit", menuName = "FruitShake/Fruit Data")]
public class FruitData : ScriptableObject
{
    [Header("Identidad Base")]
    public string fruitName;
    public GameObject prefab;
    public Sprite icon;

    [Tooltip("Marca esta casilla SOLO en el ScriptableObject de la manzana de oro.")]
    public bool isGoldenFruit = false;

    [Header("Spawn (Probabilidad de aparición)")]
    [Range(0f, 1f)] public float spawnWeight = 0.5f;

    [Header("Estadísticas Base Actuales (Calculadas al clonar)")]
    public float topSpeed;
    public float angularDrag;
    public int shopValue;
    public int scoreValue;

    [Tooltip("Este es el resumen de velocidad (0 a 100) que se mostrará en la UI.")]
    public float velocidadResumen;

    [Header("Configuración de Rangos para la Carrera")]
    public float minAngularDrag = 0.05f;
    public float maxAngularDrag = 0.2f;
    [Space(5)]
    public float minTopSpeed = 5f;
    public float maxTopSpeed = 22f; // Subimos a 22 para abarcar el máximo de la de Oro

    public FruitData CreateUniqueClone()
    {
        FruitData uniqueFruit = Instantiate(this);
        uniqueFruit.name = this.name;

        // 1. Asignar las físicas aleatorias dentro de sus rangos editables
        uniqueFruit.angularDrag = Random.Range(minAngularDrag, maxAngularDrag);
        uniqueFruit.topSpeed = Random.Range(minTopSpeed, maxTopSpeed);
        uniqueFruit.scoreValue = this.scoreValue;

        // 2. CALCULAR EL RESUMEN DE VELOCIDAD (Fórmula de 0 a 100)
        // Evaluamos qué tan buena es la velocidad (0 = peor, 1 = mejor)
        float factorVelocidad = Mathf.InverseLerp(minTopSpeed, maxTopSpeed, uniqueFruit.topSpeed);

        // Evaluamos qué tan bueno es el drag. Como MENOR drag es MEJOR, invertimos el orden en el Lerp
        float factorDrag = Mathf.InverseLerp(maxAngularDrag, minAngularDrag, uniqueFruit.angularDrag);

        // Promediamos ambos factores y lo escalamos de 0 a 100
        float resultadoPromedio = (factorVelocidad + factorDrag) / 2f;
        uniqueFruit.velocidadResumen = Mathf.Clamp(resultadoPromedio * 100f, 1f, 100f);


        // 3. ECONOMÍA: El valor comercial ahora depende directamente de su stat de resumen
        float multiplicadorOro = isGoldenFruit ? 3.0f : 1.5f;
        uniqueFruit.shopValue = Mathf.RoundToInt(uniqueFruit.velocidadResumen * multiplicadorOro);

        return uniqueFruit;
    }
}