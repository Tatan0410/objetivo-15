using UnityEngine;
using UnityEngine.EventSystems;

public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum TipoAccion
    {
        Izquierda,
        Derecha,
        Saltar,
        Disparo,
        Ataque,
        CambioBala
    }

    [Tooltip("Accion que ejecuta este boton (llamada directa a ControlTouch).")]
    public TipoAccion accion;

    public void OnPointerDown(PointerEventData eventData)
    {
        ControlTouch c = ControlTouch.instancia;
        if (c == null) return;

        switch (accion)
        {
            case TipoAccion.Izquierda: c.ApretarIzquierda(); break;
            case TipoAccion.Derecha: c.ApretarDerecha(); break;
            case TipoAccion.Saltar: c.ApretarSaltar(); break;
            case TipoAccion.Disparo: c.ApretarDisparo(); break;
            case TipoAccion.Ataque: c.ApretarAtaque(); break;
            case TipoAccion.CambioBala: c.CambiarBala(); break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ControlTouch c = ControlTouch.instancia;
        if (c == null) return;

        switch (accion)
        {
            case TipoAccion.Izquierda: c.SoltarIzquierda(); break;
            case TipoAccion.Derecha: c.SoltarDerecha(); break;
        }
    }
}