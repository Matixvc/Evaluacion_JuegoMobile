using UnityEngine;

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
        Time.timeScale = 0f;
    }

    public void ClosePause()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void GoToGranja()
    {
        ClosePause();
        PanelManager.Instance.ShowBlender();
    }

    public void GoToInventario()
    {
        ClosePause();
        PanelManager.Instance.ShowTree();
    }

    public void Salir() => Application.Quit();
}