using UnityEngine;

public class ReiniciarProgreso : MonoBehaviour
{
    [Header("Confirmacion (opcional)")]
    public GameObject panelConfirmacion;

    public void AbrirConfirmacion()
    {
        if (panelConfirmacion != null)
        {
            panelConfirmacion.SetActive(true);
            SeleccionUI.SeleccionarPrimero(panelConfirmacion);
            ConfigurarNavegacionConfirmacion();
        }
    }

    void ConfigurarNavegacionConfirmacion()
    {
        if (panelConfirmacion == null) return;
        var botones = panelConfirmacion.GetComponentsInChildren<UnityEngine.UI.Button>(true);
        if (botones.Length >= 2)
        {
            var nav0 = botones[0].navigation;
            var nav1 = botones[1].navigation;
            nav0.mode = UnityEngine.UI.Navigation.Mode.Explicit;
            nav1.mode = UnityEngine.UI.Navigation.Mode.Explicit;
            nav0.selectOnRight = botones[1];
            nav0.selectOnLeft = botones[1];
            nav1.selectOnRight = botones[0];
            nav1.selectOnLeft = botones[0];
            botones[0].navigation = nav0;
            botones[1].navigation = nav1;
        }
    }

    void Update()
    {
        if (panelConfirmacion != null && panelConfirmacion.activeSelf && Input.GetButtonDown("Cancel"))
            CerrarConfirmacion();
    }

    public void CerrarConfirmacion()
    {
        if (panelConfirmacion != null)
            panelConfirmacion.SetActive(false);
        SeleccionUI.LimpiarSeleccion();
        var selector = Object.FindFirstObjectByType<SelectorNivelesMapa>();
        if (selector != null)
            SeleccionUI.SeleccionarPrimero(selector.gameObject);
    }

    public void Reiniciar()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Progreso reiniciado (PlayerPrefs borrados).");

        CerrarConfirmacion();

        var selector = Object.FindFirstObjectByType<SelectorNivelesMapa>();
        if (selector != null)
            selector.RefrescarBotones();

        var manager = Object.FindFirstObjectByType<MapamundialEstadoManager>();
        if (manager != null)
            manager.AplicarMapa();
    }
}