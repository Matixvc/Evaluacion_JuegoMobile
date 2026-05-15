using UnityEngine;

public class S : MonoBehaviour
{
    par
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            // 1. Detenemos el Trail porque ya no se mueve
            efectoTrail.Stop();

            // 2. Activamos el estallido de impacto
            efectoImpacto.Play();

            // 3. Dejamos el Aura (Sparkles) encendido para que brille en el suelo
        }
    }
}
