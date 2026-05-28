using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Paneles de UI (Canvas Mobile)")]
    public GameObject panelPausa;
    public GameObject panelAjustes;

    public static bool juegoPausado = false;

    public void AlternarPausaConBoton()
    {
        if (juegoPausado) Reanudar();
        else Pausar();
    }

    public void Reanudar()
    {
        if (panelPausa != null) panelPausa.SetActive(false);
        if (panelAjustes != null) panelAjustes.SetActive(false);

        Time.timeScale = 1f;
        juegoPausado = false;
    }

    public void Pausar()
    {
        if (panelPausa != null) panelPausa.SetActive(true);

        Time.timeScale = 0f;
        juegoPausado = true;
    }

    public void AbrirAjustes()
    {
        if (panelPausa != null) panelPausa.SetActive(false);
        if (panelAjustes != null) panelAjustes.SetActive(true);
    }

    public void CerrarAjustes()
    {
        if (panelAjustes != null) panelAjustes.SetActive(false);
        if (panelPausa != null) panelPausa.SetActive(true);
    }

    public void VolverAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        juegoPausado = false;
        SceneManager.LoadScene("MainMenuScene");
    }
}