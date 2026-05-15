using UnityEngine;
using System.Collections;

public class ControladorFrutaEspecial : MonoBehaviour
{
    [Header("Referencias de Efectos")]
    public ParticleSystem sistemaTrail;    // Arrastra aquí el Efecto_Trail
    public ParticleSystem sistemaSparkles; // Arrastra aquí el Efecto_Sparkles
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

            // 3. Opcional: Pequeña vibración (Haptic Feedback) para mobile
#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif

            Debug.Log("¡Fruta ha aterrizado!");
        }
    }
}
