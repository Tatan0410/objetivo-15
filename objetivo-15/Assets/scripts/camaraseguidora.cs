using UnityEngine;

public class CamaraSeguidora : MonoBehaviour
{
    [Header("Objetivo a seguir")]
    public Transform jugador;

    [Header("Ajustes de seguimiento")]
    public float suavizado = 0.1f;
    public Vector2 offset = new Vector2(1.5f, 1f);

    [Header("L�mites del nivel (opcional)")]
    public float limiteIzquierdo = -10f;
    public float limiteDerecho = 50f;
    public float limiteAbajo = -5f;
    public float limiteArriba = 10f;
    public bool usarLimites = true;

    private Vector3 velocidad = Vector3.zero;
    private Vector3? posicionForzada = null;
    private Camera cam;
    private float tamanoOriginal;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null) tamanoOriginal = cam.orthographicSize;
    }

    public void ForzarPosicion(Vector3 posicion)
    {
        posicionForzada = posicion;
    }

    public void RestaurarPosicion()
    {
        posicionForzada = null;
    }

    public void ForzarZoom(float factor)
    {
        if (cam != null) cam.orthographicSize = tamanoOriginal * factor;
    }

    public void RestaurarZoom()
    {
        if (cam != null) cam.orthographicSize = tamanoOriginal;
    }

    void LateUpdate()
    {
        if (jugador == null) return;

        Vector3 destino;

        if (posicionForzada.HasValue)
        {
            destino = new Vector3(
                posicionForzada.Value.x,
                posicionForzada.Value.y,
                transform.position.z
            );
        }
        else
        {
            destino = new Vector3(
                jugador.position.x + offset.x,
                jugador.position.y + offset.y,
                transform.position.z
            );
        }

        if (usarLimites)
        {
            destino.x = Mathf.Clamp(destino.x, limiteIzquierdo, limiteDerecho);
            destino.y = Mathf.Clamp(destino.y, limiteAbajo, limiteArriba);
        }

        transform.position = Vector3.SmoothDamp(
            transform.position, destino, ref velocidad, suavizado
        );
    }
}