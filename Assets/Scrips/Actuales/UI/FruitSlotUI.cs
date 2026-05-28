using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FruitSlotUI : MonoBehaviour
{
    [Header("Referencias UI del Prefab")]
    public TextMeshProUGUI nombreText;
    public TextMeshProUGUI statsText;  // El objeto 'TextoStats'
    public Image iconoFruta;

    public void SetupSlot(FruitData data)
    {
        if (data == null) return;

        // 1. NOMBRE (Tipo de manzana)
        if (data.isGoldenFruit)
        {
            nombreText.text = "Manzana de Oro";
            nombreText.color = new Color(1f, 0.84f, 0f);
        }
        else
        {
            nombreText.text = "Manzana Normal";
            nombreText.color = Color.black;
        }

        // 2. MOSTRAR EL RESUMEN DEFINITIVO
        // Mostramos el porcentaje entero sin decimales (:F0) para que sea súper legible
        statsText.text = $"Velocidad: {data.velocidadResumen:F0}%";

        // 3. ICONO
        if (data.icon != null && iconoFruta != null)
        {
            iconoFruta.sprite = data.icon;
        }
    }
}