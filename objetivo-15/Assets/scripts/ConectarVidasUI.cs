using UnityEngine;
using UnityEngine.UI;

public class ConectarVidasUI : MonoBehaviour
{
    void Start()
    {
        Image[] images = GetComponentsInChildren<Image>();
        if (images.Length > 0 && VidasManager.instancia != null)
            VidasManager.instancia.AsignarCorazones(images);
    }
}
