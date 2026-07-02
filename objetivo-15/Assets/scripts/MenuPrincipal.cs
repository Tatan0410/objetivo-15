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
        SceneManager.LoadScene("Mapamundial");
    }

    public void AbrirSoluciones()
    {
        SceneManager.LoadScene("cutscene_soluciones");
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}
