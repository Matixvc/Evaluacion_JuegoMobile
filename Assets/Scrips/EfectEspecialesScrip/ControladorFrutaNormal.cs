using UnityEngine;

public class ControladorFrutaNormal : MonoBehaviour
{
    [Header("Referencias de Efectos")]
    public ParticleSystem sistemaTrail;    // Arrastra aquí el Efecto_Trail
    public ParticleSystem sistemaImpacto;  // Arrastra aquí el Efecto_Impacto

    private bool haImpactado = false;


    // Se activa automáticamente cuando la fruta toca el suelo
    void OnCollisionEnter(Collision collision)
    {
        // Solo se activa si choca con el suelo y no ha impactado antes
        if (collision.gameObject.CompareTag("Suelo") && !haImpactado)
        {
            haImpactado = true;

            // 1. Detenemos el rastro de caída
            if (sistemaTrail != null) sistemaTrail.Stop();

            // 2. Activamos el estallido de impacto
            if (sistemaImpacto != null) sistemaImpacto.Play();

            Debug.Log("¡Fruta ha aterrizado!");
        }
    }
}
