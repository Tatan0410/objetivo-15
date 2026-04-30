using UnityEngine;

public class CamaraSeguidora : MonoBehaviour
{
    [Header("Objetivo a seguir")]
    public Transform jugador;

    [Header("Ajustes de seguimiento")]
    public float suavizado = 0.1f;        // qué tan fluido sigue
    public Vector2 offset = new Vector2(1.5f, 1f); // adelanta la cámara

    [Header("Límites del nivel (opcional)")]
    public float limiteIzquierdo = -10f;
    public float limiteDerecho = 50f;
    public float limiteAbajo = -5f;
    public float limiteArriba = 10f;
    public bool usarLimites = true;

    private Vector3 velocidad = Vector3.zero;

    void LateUpdate()
    {
        if (jugador == null) return;

        // Posición destino siguiendo al jugador
        Vector3 destino = new Vector3(
            jugador.position.x + offset.x,
            jugador.position.y + offset.y,
            transform.position.z   // Z fijo en -10
        );

        // Limitar dentro del nivel
        if (usarLimites)
        {
            destino.x = Mathf.Clamp(destino.x, limiteIzquierdo, limiteDerecho);
            destino.y = Mathf.Clamp(destino.y, limiteAbajo, limiteArriba);
        }

        // Movimiento suave
        transform.position = Vector3.SmoothDamp(
            transform.position, destino, ref velocidad, suavizado
        );
    }
}