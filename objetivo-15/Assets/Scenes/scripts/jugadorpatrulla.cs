using UnityEngine;

public class jugadorpatrulla : MonoBehaviour
{
    [Header("Patrulla")]
    public float velocidad = 2f;
    public float distanciaPatrulla = 3f;

    private Vector2 puntoInicio;
    private bool moviendoDerecha = true;

    void Start()
    {
        puntoInicio = transform.position;
    }

    void Update()
    {
        Patrullar();
    }

    void Patrullar()
    {
        float dir = moviendoDerecha ? 1f : -1f;

        if (HayTrampaAdelante(dir))
        {
            moviendoDerecha = !moviendoDerecha;
            return;
        }

        if (moviendoDerecha)
        {
            transform.Translate(Vector2.right * velocidad * Time.deltaTime);
            if (transform.position.x >= puntoInicio.x + distanciaPatrulla)
                moviendoDerecha = false;
        }
        else
        {
            transform.Translate(Vector2.left * velocidad * Time.deltaTime);
            if (transform.position.x <= puntoInicio.x - distanciaPatrulla)
                moviendoDerecha = true;
        }
    }

    static bool EsTrampa(GameObject go)
    {
        if (go == null) return false;
        return go.GetComponent<TrampaHot>() != null
            || go.GetComponent<DañoEnemigo>() != null;
    }

    bool HayTrampaAdelante(float dir)
    {
        Collider2D col = GetComponent<Collider2D>();
        Vector2 origen = col != null ? col.bounds.center : (Vector2)transform.position;
        Vector2 size = col != null ? col.bounds.size : new Vector2(0.8f, 0.8f);
        float distancia = (col != null ? col.bounds.extents.x : 0.4f) + 0.4f;

        RaycastHit2D[] hits = Physics2D.BoxCastAll(origen, size, 0f, Vector2.right * dir, distancia);
        foreach (RaycastHit2D h in hits)
        {
            if (h.collider == null || h.collider.gameObject == gameObject) continue;
            if (EsTrampa(h.collider.gameObject)) return true;
        }
        return false;
    }

    // ← OnTriggerEnter2D porque el jugador tiene Is Trigger activado
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            MuerteJugador muerte = col.GetComponent<MuerteJugador>();
            if (muerte != null)
                muerte.MorirPorEnemigo();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 pos = Application.isPlaying ? puntoInicio : (Vector2)transform.position;
        Gizmos.DrawLine(
            new Vector2(pos.x - distanciaPatrulla, pos.y),
            new Vector2(pos.x + distanciaPatrulla, pos.y)
        );
    }
}