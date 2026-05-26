using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;

    public void Toggle()
    {
        if (pausePanel.activeSelf) ClosePause();
        else OpenPause();
    }

    public void OpenPause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // Congela las físicas y el movimiento de las manzanas
    }

    public void ClosePause()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // Devuelve el tiempo a la normalidad
    }

    public void GoToGranja()
    {
        Time.timeScale = 1f; // ¡IMPORTANTE!: Siempre devuelve el tiempo a 1 antes de cambiar de escena

        // MEJORA: Guardamos el inventario y el oro actual antes de irnos
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveProgress();
        }

        SceneManager.LoadScene("Granja");
    }

    public void GoToInventario()
    {
        ClosePause(); // Esto ya devuelve el Time.timeScale a 1f automáticamente

        if (PanelManager.Instance != null)
        {
            PanelManager.Instance.ShowTree();
        }
    }

    public void Salir()
    {
        // MEJORA: Si el jugador cierra la app directo desde la pausa, respaldamos sus datos
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveProgress();
        }

        Application.Quit();
    }
}