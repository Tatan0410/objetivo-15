using UnityEngine;

public class ReiniciarProgreso : MonoBehaviour
{
    [Header("Confirmacion (opcional)")]
    public GameObject panelConfirmacion;

    public void AbrirConfirmacion()
    {
        if (panelConfirmacion != null)
            panelConfirmacion.SetActive(true);
    }

    public void CerrarConfirmacion()
    {
        if (panelConfirmacion != null)
            panelConfirmacion.SetActive(false);
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
        else
            Debug.LogWarning("No se encontro SelectorNivelesMapa para refrescar los botones.");
    }
}