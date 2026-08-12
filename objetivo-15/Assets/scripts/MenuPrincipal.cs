using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelMenuPrincipal;

    void Start()
    {
        Time.timeScale = 1f;
        if (panelMenuPrincipal != null)
            panelMenuPrincipal.SetActive(true);

        Transform panel = panelMenuPrincipal != null ? panelMenuPrincipal.transform : null;
        var bJugar = panel?.Find("BotonJugar")?.gameObject;
        if (bJugar != null)
            bJugar.GetComponent<Button>().onClick.AddListener(Jugar);

        var bSol = panel?.Find("BotonSoluciones")?.gameObject;
        if (bSol != null)
            bSol.GetComponent<Button>().onClick.AddListener(AbrirSoluciones);
    }

    public void Jugar()
    {
        SceneTransitionManager.CargarEscenaConFallback("Mapamundial");
    }

    public void AbrirSoluciones()
    {
        SceneTransitionManager.CargarEscenaConFallback("cutscene_soluciones");
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}
