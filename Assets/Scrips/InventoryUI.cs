using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Referencias")]
    public Transform fruitContainer;  // Panel donde aparecen los iconos
    public GameObject fruitSlotPrefab; // Prefab: cubo pequeño + TextMeshPro
    public TextMeshProUGUI goldText;

    void OnEnable() => Refresh();  // Se actualiza cada vez que se activa el panel

    public void Refresh()
    {
        foreach (Transform child in fruitContainer)
            Destroy(child.gameObject);

        var inv = GameManager.Instance.inventory;

        // Normales
        foreach (var entry in inv.normalFruits)
        {
            GameObject slot = Instantiate(fruitSlotPrefab, fruitContainer);
            var label = slot.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = $"{entry.fruit.fruitName} x{entry.count}";
        }

        // Oro
        foreach (var entry in inv.goldFruits)
        {
            GameObject slot = Instantiate(fruitSlotPrefab, fruitContainer);
            var label = slot.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = $"{entry.fruit.fruitName} (ORO) x{entry.count}";
        }

        if (goldText != null)
            goldText.text = $"Oro: {GameManager.Instance.playerData.gold}";
    }
    //public void Refresh()
    //{
    //    // Limpiar slots anteriores
    //    foreach (Transform child in fruitContainer)
    //        Destroy(child.gameObject);

    //    // Crear un slot por cada fruta
    //    foreach (var entry in GameManager.Instance.inventory.fruits)
    //    {
    //        GameObject slot = Instantiate(fruitSlotPrefab, fruitContainer);
    //        TextMeshProUGUI label = slot.GetComponentInChildren<TextMeshProUGUI>();
    //        if (label != null)
    //            label.text = $"{entry.fruit.fruitName} x{entry.count}";
    //    }

    //    // Actualizar oro
    //    if (goldText != null)
    //        goldText.text = $"Oro: {GameManager.Instance.playerData.gold}";
    //}
}