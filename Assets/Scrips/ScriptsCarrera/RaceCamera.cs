using UnityEngine;

public class RaceCamera : MonoBehaviour
{
    private Transform _target; // La manzana que vamos a seguir

    [Header("Configuración de Seguimiento")]
    [Tooltip("Distancia/Altura relativa desde la cámara hacia la manzana")]
    public Vector3 offset = new Vector3(0f, 5f, -7f);

    [Tooltip("Qué tan suave sigue la cámara al objeto (A menor número, más suave)")]
    public float smoothSpeed = 5f;

    private RaceManager _raceManager;

    void Start()
    {
        // Buscamos el RaceManager en la escena para saber cuál es la manzana del jugador
        _raceManager = Object.FindFirstObjectByType<RaceManager>();
    }

    void LateUpdate()
    {
        // 1. Si no tenemos objetivo aún, intentamos pedirle el jugador instanciado al RaceManager
        if (_target == null)
        {
            if (_raceManager != null && _raceManager.jugadorInstanciado != null)
            {
                _target = _raceManager.jugadorInstanciado.transform;
            }
            return; // Esperamos al siguiente fotograma si aún no aparece
        }

        // 2. Calculamos la posición ideal a la que debe ir la cámara
        Vector3 posicionDeseada = _target.position + offset;

        // 3. Interpolamos de forma fluida entre la posición actual y la ideal (Lerp)
        Vector3 posicionSuave = Vector3.Lerp(transform.position, posicionDeseada, smoothSpeed * Time.deltaTime);

        // 4. Aplicamos la posición
        transform.position = posicionSuave;

        // 5. Hacemos que la cámara siempre mire en dirección a la manzana campeona
        transform.LookAt(_target.position + Vector3.up * 0.5f);
    }
}