using UnityEngine;

public class MuerteJugador : MonoBehaviour
{
    [Header("Límite de caída")]
    public float limiteY = -20f;

    private bool muriendo = false;
    private Rigidbody2D rb;
    private PlayerController pc;
    private Vector3 posicionRespawn;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pc = GetComponent<PlayerController>();
        posicionRespawn = transform.position;
    }

    void Update()
    {
        if (muriendo) return;
        if (transform.position.y < limiteY)
            Morir();
    }

    public void MorirPorEnemigo()
    {
        if (muriendo) return;
        Morir();
    }

    public void GuardarRespawn(Vector3 pos)
    {
        posicionRespawn = pos;
    }

    void Morir()
    {
        if (muriendo) return;
        muriendo = true;

        if (pc != null) pc.DesactivarControl();
        if (rb != null) rb.velocity = Vector2.zero;

        if (VidasManager.instancia != null)
            VidasManager.instancia.PerderVida();

        if (this == null) return;

        if (VidasManager.instancia != null && VidasManager.instancia.vidasActuales > 0)
        {
            if (pc != null)
            {
                Vector3 destino = posicionRespawn;
                Debug.Log("[MuerteJugador] posicionRespawn=" + posicionRespawn + " destino=" + destino);
                pc.posicionRespawn = destino;
                pc.respawnPendiente = true;
            }
        }

        muriendo = false;
    }
}