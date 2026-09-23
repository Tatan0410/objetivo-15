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

        var bCred = panel?.Find("BotonCreditos")?.gameObject;
        if (bCred != null)
            bCred.GetComponent<Button>().onClick.AddListener(AbrirCreditos);

        var bSalir = panel?.Find("BotonSalir")?.gameObject;
        if (bSalir != null)
            bSalir.GetComponent<Button>().onClick.AddListener(Salir);

        if (panelMenuPrincipal != null)
            SeleccionUI.SeleccionarPrimero(panelMenuPrincipal);
    }

    public void Jugar()
    {
        SceneTransitionManager.CargarEscenaConFallback("Mapamundial");
    }

    public void AbrirSoluciones()
    {
        SceneTransitionManager.CargarEscenaConFallback("cutscene_soluciones");
    }

    public void AbrirCreditos()
    {
        CreditosManager.escenaRetorno = "menuprincipal";
        SceneTransitionManager.CargarEscenaConFallback("creditos");
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
