using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public Transform fruitContainer;   // El panel con un componente 'Grid Layout Group' para ordenar los slots
    public GameObject fruitSlotPrefab; // Tu prefab de la manzana en la UI (Texto + Icono si tiene)
    public TextMeshProUGUI goldText;

    void OnEnable()
    {
        Refresh(); // Se actualiza automáticamente cada vez que abres la pantalla del inventario
    }

    public void Refresh()
    {
        // 1. Limpiar los slots visuales de la cosecha anterior para que no se dupliquen
        foreach (Transform child in fruitContainer)
        {
            Destroy(child.gameObject);
        }

        var inv = GameManager.Instance.inventory;

        // 2. Crear un slot visual por cada tipo de manzana de carrera que tengamos guardada
        foreach (var entry in inv.collectedFruits)
        {
            if (entry.fruit == null) continue;

            // Instanciamos el cuadrito en el panel
            GameObject slot = Instantiate(fruitSlotPrefab, fruitContainer);

            // Buscamos el componente de texto en el prefab
            TextMeshProUGUI label = slot.GetComponentInChildren<TextMeshProUGUI>();

            if (label != null)
            {
                // Muestra el nombre de la manzana, su fricción/velocidad si quieres, y la cantidad
                label.text = $"{entry.fruit.fruitName}\n(Drag: {entry.fruit.angularDrag}) x{entry.count}";
            }
        }

        // 3. Actualizar el marcador de oro del jugador en la pantalla
        if (goldText != null)
        {
            goldText.text = $"Oro: {GameManager.Instance.playerData.gold}";
        }
    }
}