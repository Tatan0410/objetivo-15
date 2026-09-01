using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class NodoNivelMapa
{
    [Header("Identidad")]
    public int numeroNivel;
    public string nombreEscenaCutscene;

    [Header("UI")]
    public Button boton;
    public string nombreBoton;
}

public class SelectorNivelesMapa : MonoBehaviour
{
    [Header("Nodos del mapa")]
    public NodoNivelMapa[] nodos;

    [Header("Mensaje de bloqueado")]
    public GameObject panelBloqueado;
    public float duracionMensaje = 2f;

    void Start()
    {
        ConfigurarBotones();
    }

    void ConfigurarBotones()
    {
        int nivelDesbloqueado = PlayerPrefs.GetInt("NivelDesbloqueado", 0);

        foreach (NodoNivelMapa nodo in nodos)
        {
            if (nodo.boton == null)
            {
                Debug.LogWarning("Boton no asignado para nivel " + nodo.numeroNivel);
                continue;
            }

            bool desbloqueado = nodo.numeroNivel <= nivelDesbloqueado + 1;

            nodo.boton.interactable = desbloqueado;

            nodo.boton.onClick.RemoveAllListeners();
            int numeroCapturado = nodo.numeroNivel;
            nodo.boton.onClick.AddListener(() => IntentarEntrarNivel(numeroCapturado));
        }

        SeleccionarNodoActual();
    }

    void SeleccionarNodoActual()
    {
        int nivelDesbloqueado = PlayerPrefs.GetInt("NivelDesbloqueado", 0);

        // Preparar la navegacion entre todos los nodos del mapa (no solo uno).
        // Usamos el Canvas como contenedor para que todos los botones queden enlazados.
        var canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
            SeleccionUI.PrepararNavegacion(canvas.gameObject);

        NodoNivelMapa actual = System.Array.Find(nodos,
            n => n.numeroNivel == nivelDesbloqueado + 1 && n.boton != null && n.boton.interactable);
        if (actual != null)
        {
            SeleccionUI.SeleccionarPrimero(actual.boton.gameObject);
            return;
        }

        foreach (NodoNivelMapa nodo in nodos)
        {
            if (nodo.boton != null && nodo.boton.interactable)
            {
                SeleccionUI.SeleccionarPrimero(nodo.boton.gameObject);
                return;
            }
        }
    }

    void IntentarEntrarNivel(int numeroNivel)
    {
        int nivelDesbloqueado = PlayerPrefs.GetInt("NivelDesbloqueado", 0);

        if (numeroNivel > nivelDesbloqueado + 1)
        {
            MostrarMensajeBloqueado();
            return;
        }

        NodoNivelMapa nodo = System.Array.Find(nodos, n => n.numeroNivel == numeroNivel);
        if (nodo == null)
        {
            Debug.LogWarning("Nodo no encontrado para nivel " + numeroNivel);
            return;
        }

        PlayerPrefs.SetInt("NodoActual", numeroNivel - 1);
        PlayerPrefs.Save();

        if (GameManager.instancia != null)
            GameManager.instancia.ResetearCheckpoint();

        SceneTransitionManager.CargarEscenaConFallback(nodo.nombreEscenaCutscene);
    }

    void MostrarMensajeBloqueado()
    {
        if (panelBloqueado == null) return;

        panelBloqueado.SetActive(true);
        StartCoroutine(OcultarMensajeDespues());
    }

    IEnumerator OcultarMensajeDespues()
    {
        yield return new WaitForSeconds(duracionMensaje);
        if (panelBloqueado != null)
            panelBloqueado.SetActive(false);
    }

    public void RefrescarBotones()
    {
        ConfigurarBotones();
    }
}
