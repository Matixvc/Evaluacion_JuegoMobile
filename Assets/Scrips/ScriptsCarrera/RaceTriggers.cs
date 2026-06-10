using UnityEngine;

public class RaceTriggers : MonoBehaviour
{
    private RaceManager _raceManager;
    private bool _yaTermino = false; // Evita que se active dos veces si rebota

    void Start()
    {
        // Buscamos el RaceManager en la escena de la carrera
        _raceManager = Object.FindFirstObjectByType<RaceManager>();
    }

    // 1. DETECTAR SI CAE AL ABISMO (Toca el suelo con tag "Piso")
    private void OnCollisionEnter(Collision collision)
    {
        if (_yaTermino) return;

        if (collision.gameObject.CompareTag("Piso"))
        {
            _yaTermino = true;
            bool esJugador = gameObject.name.Contains("[JUGADOR]");

            if (_raceManager != null)
            {
                _raceManager.ProcesarCaida(gameObject, esJugador);
            }
        }
    }

    // 2. DETECTAR SI LLEGA A LA META (Box Collider invisible con "Is Trigger")
    private void OnTriggerEnter(Collider other)
    {
        if (_yaTermino) return;

        if (other.gameObject.CompareTag("Meta"))
        {
            _yaTermino = true;
            bool esJugador = gameObject.name.Contains("[JUGADOR]");

            if (_raceManager != null)
            {
                _raceManager.ProcesarMeta(esJugador);
            }
        }
    }
}