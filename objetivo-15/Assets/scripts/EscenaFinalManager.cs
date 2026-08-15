using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EscenaFinalManager : MonoBehaviour
{
    [Header("Imagen a mostrar")]
    public Image imagen;

    [Header("Etiqueta guia (opcional)")]
    public TMP_Text etiqueta;

    void Start()
    {
        if (imagen == null) return;

        if (imagen.sprite != null)
        {
            imagen.color = Color.white;
            if (etiqueta != null)
                etiqueta.gameObject.SetActive(false);
        }
        else
        {
            imagen.color = new Color(1f, 1f, 1f, 0.12f);
            if (etiqueta != null)
                etiqueta.gameObject.SetActive(true);
        }
    }

    public void Continuar()
    {
        SceneTransitionManager.CargarEscenaConFallback("creditos");
    }
}