using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelMenuPrincipal;

    void Start()
    {
        Time.timeScale = 1f;
        panelMenuPrincipal.SetActive(true);
    }

    public void Jugar()
    {
        if (SceneTransitionManager.instancia != null)
            SceneTransitionManager.instancia.CargarEscena("Mapamundial");
    }

    public void AbrirSoluciones()
    {
        if (SceneTransitionManager.instancia != null)
            SceneTransitionManager.instancia.CargarEscena("cutscene_soluciones");
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}
