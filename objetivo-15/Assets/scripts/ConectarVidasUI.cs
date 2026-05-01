using UnityEngine;
using TMPro;

public class ConectarVidasUI : MonoBehaviour
{
    void Start()
    {
        TMP_Text texto = GetComponent<TMP_Text>();
        if (VidasManager.instancia != null)
            VidasManager.instancia.AsignarTexto(texto);
    }
}