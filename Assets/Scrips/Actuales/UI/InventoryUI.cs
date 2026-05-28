using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public Transform fruitContainer;   // El panel con el 'Grid Layout Group'
    public GameObject fruitSlotPrefab; // Tu prefab de la manzana (SlotManzanaUI)
    public TextMeshProUGUI goldText;

    void OnEnable()
    {
        Refresh(); // Se actualiza automáticamente al abrir la tienda
    }

    public void Refresh()
    {
        // 1. Limpiar los slots visuales anteriores para que no se dupliquen
        foreach (Transform child in fruitContainer)
        {
            Destroy(child.gameObject);
        }

        var inv = GameManager.Instance.inventory;

        // 2. Crear un slot visual por cada manzana recolectada
        foreach (var entry in inv.collectedFruits)
        {
            if (entry == null) continue;

            // Instanciamos el cuadrito en el panel
            GameObject slot = Instantiate(fruitSlotPrefab, fruitContainer);

            // CORRECCIÓN: En lugar de buscar un componente de texto genérico,
            // llamamos al script especializado del prefab que creamos paso a paso.
            if (slot.TryGetComponent(out FruitSlotUI slotScript))
            {
                slotScript.SetupSlot(entry); // Le pasa la manzana para que acomode Nombre y Stats por separado
            }
            else
            {
                // Alerta por si acaso el prefab no tiene el script pegado
                Debug.LogWarning($"El prefab asignado en InventoryUI no tiene el componente FruitSlotUI en su objeto raíz.");
            }
        }

        // 3. Actualizar el marcador de oro del jugador en la pantalla
        if (goldText != null)
        {
            goldText.text = $"Oro: {GameManager.Instance.playerData.gold}";
        }
    }
}