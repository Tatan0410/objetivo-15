using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelMenuPrincipal;
    public GameObject panelAcercaDe;

    void Start()
    {
        Time.timeScale = 1f;

        panelMenuPrincipal.SetActive(true);
        panelAcercaDe.SetActive(false);
    }

    public void Jugar()
    {
        SceneManager.LoadScene("Mapamundial");
    }

    public void AbrirAcercaDe()
    {
        panelMenuPrincipal.SetActive(false);
        panelAcercaDe.SetActive(true);
    }

    public void VolverAlMenu()
    {
        panelAcercaDe.SetActive(false);
        panelMenuPrincipal.SetActive(true);
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}
