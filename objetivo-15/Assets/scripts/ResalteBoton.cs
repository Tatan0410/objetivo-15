using UnityEngine;
using UnityEngine.EventSystems;

public class ResalteBoton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private const float FactorEscala = 1.10f;

    private Vector3 escalaBase;
    private bool inicializado = false;

    public void OnSelect(BaseEventData eventData)
    {
        AsegurarBase();
        transform.localScale = escalaBase * FactorEscala;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Restaurar();
    }

    void OnDisable()
    {
        Restaurar();
    }

    void AsegurarBase()
    {
        if (!inicializado)
        {
            escalaBase = transform.localScale;
            inicializado = true;
        }
    }

    void Restaurar()
    {
        if (!inicializado) return;
        transform.localScale = escalaBase;
    }
}
