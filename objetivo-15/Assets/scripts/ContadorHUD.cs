using UnityEngine;
using UnityEngine.UI;

public class ContadorHUD : MonoBehaviour
{
    public Image icono;
    public TMPro.TMP_Text texto;

    public void Actualizar(int valor)
    {
        if (texto != null)
            texto.text = valor.ToString();
    }

    public void AsignarIcono(Sprite sprite)
    {
        if (icono != null)
            icono.sprite = sprite;
    }
}
