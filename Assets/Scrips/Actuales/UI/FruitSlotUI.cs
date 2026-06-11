using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FruitSlotUI : MonoBehaviour
{
    [Header("Referencias UI del Prefab")]
    public TextMeshProUGUI nombreText;
    public TextMeshProUGUI statsText;  // El objeto 'TextoStats'
    public Image iconoFruta;

    [Header("Interactividad")]
    [Tooltip("Arrastra aquí el componente Button de tu prefab (puede ser el fondo de la casilla o un botón exclusivo)")]
    public Button selectButton;

    // Variable interna para recordar qué manzana específica maneja esta ranura
    private FruitData _currentFruitData;

    public void SetupSlot(FruitData data)
    {
        if (data == null) return;

        // Guardamos la referencia de los datos únicos de esta manzana
        _currentFruitData = data;

        // 1. NOMBRE (Tipo de manzana)
        if (data.isGoldenFruit)
        {
            nombreText.text = "Manzana de Oro";
            nombreText.color = new Color(1f, 0.84f, 0f); // Color dorado estético
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

        // 4. CONFIGURACIÓN DEL BOTÓN POR CÓDIGO
        if (selectButton != null)
        {
            // Limpiamos listeners antiguos para evitar que se acumulen clics si la UI se refresca
            selectButton.onClick.RemoveAllListeners();

            // Le asignamos la función que se ejecutará al tocar la pantalla en Android
            selectButton.onClick.AddListener(OnSlotClicked);
        }
    }

    private void OnSlotClicked()
    {
        // Al presionar el botón, le enviamos la manzana elegida al GameManager
        // Él se encargará de guardar la campeona, vender las otras y cargar la pista de carrera
        if (_currentFruitData != null)
        {
            GameManager.Instance.SelectFruitForRace(_currentFruitData);
        }
    }
}