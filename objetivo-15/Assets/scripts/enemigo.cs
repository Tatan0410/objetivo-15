using UnityEngine;

public class Enemigo : MonoBehaviour
{
    [Header("Patrulla")]
    public float velocidad = 2f;
    public float distanciaPatrulla = 3f;

    [Header("Detección")]
    public Transform detectorBorde;
    public LayerMask capaSuelo;

    private Vector2 puntoInicio;
    private bool moviendoDerecha = true;
    private Rigidbody2D rb;
    private bool muerto = false;

    void Start()
    {
        puntoInicio = transform.position;
        rb = GetComponent<Rigidbody2D>();
        rb.mass = 1000f;
    }

    void FixedUpdate()
    {
        if (muerto) return;
        Patrullar();
        VerificarBorde();
    }

    void Patrullar()
    {
        float dir = moviendoDerecha ? 1f : -1f;
        rb.velocity = new Vector2(dir * velocidad, rb.velocity.y);
        if (moviendoDerecha && transform.position.x >= puntoInicio.x + distanciaPatrulla)
            Voltear();
        else if (!moviendoDerecha && transform.position.x <= puntoInicio.x - distanciaPatrulla)
            Voltear();
    }

    void VerificarBorde()
    {
        if (detectorBorde == null) return;
        bool haySuelo = Physics2D.OverlapCircle(detectorBorde.position, 0.1f, capaSuelo);
        if (!haySuelo) Voltear();
        Vector2 direccion = moviendoDerecha ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direccion, 0.6f, capaSuelo);
        if (hit.collider != null) Voltear();
    }

    void Voltear()
    {
        moviendoDerecha = !moviendoDerecha;
        transform.localScale = new Vector3(
            -transform.localScale.x,
            transform.localScale.y,
            transform.localScale.z);
        puntoInicio = transform.position;
    }

    void Morir()
    {
        muerto = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        // Desactiva todos los colliders
        foreach (Collider2D col in GetComponents<Collider2D>())
            col.enabled = false;
        // Cambia color para indicar que está muerto
        GetComponent<SpriteRenderer>().color = Color.gray;
        Destroy(gameObject, 1f);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (muerto) return;
        if (!col.gameObject.CompareTag("Player")) return;
        if (VidasManager.instancia != null)
            VidasManager.instancia.PerderVida();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (muerto) return;
        if (!col.CompareTag("Player")) return;
        Rigidbody2D rbJugador = col.GetComponent<Rigidbody2D>();
        if (rbJugador != null && rbJugador.velocity.y < -0.1f)
        {
            rbJugador.velocity = new Vector2(rbJugador.velocity.x, 6f);
            Morir();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaPatrulla);
        if (detectorBorde != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(detectorBorde.position, 0.1f);
        }
    }
}