using UnityEngine;

public class EnemigoVolador : MonoBehaviour
{
    [Header("Patrulla")]
    public float velocidad = 2f;
    public float distanciaPatrulla = 3f;

    [Header("Drop de plasticos")]
    public GameObject[] prefabsPlasticos;
    public int cantidadDrop = 1;
    [Range(0f, 1f)]
    public float probabilidadDrop = 0.5f;

    private Vector2 puntoInicio;
    private bool moviendoDerecha = true;
    private Rigidbody2D rb;
    private bool muerto = false;
    private bool congelado = false;
    private Vector3 posicionMuerte;
    private Color colorOriginal;
    private SpriteRenderer sr;
    private bool yaProcesadoEsteFrame = false;

    void Start()
    {
        puntoInicio = transform.position;
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) colorOriginal = sr.color;
    }

    void FixedUpdate()
    {
        yaProcesadoEsteFrame = false;
        if (muerto || congelado) return;
        Patrullar();
    }

    void Patrullar()
    {
        if (rb == null) return;
        float dir = moviendoDerecha ? 1f : -1f;
        rb.MovePosition(rb.position + new Vector2(dir * velocidad * Time.fixedDeltaTime, 0));

        if (moviendoDerecha &&
            transform.position.x >= puntoInicio.x + distanciaPatrulla)
            Voltear();
        else if (!moviendoDerecha &&
            transform.position.x <= puntoInicio.x - distanciaPatrulla)
            Voltear();
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

    void SoltarPlasticos()
    {
        if (Random.value > probabilidadDrop) return;
        if (prefabsPlasticos == null || prefabsPlasticos.Length == 0) return;

        posicionMuerte = transform.position;

        GameObject runner = new GameObject("PlasticoSpawnRunnerVolador");
        runner.AddComponent<PlasticoSpawnRunner>().Iniciar(
            prefabsPlasticos,
            cantidadDrop,
            posicionMuerte,
            0.5f
        );
    }

    void Morir()
    {
        muerto = true;
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        foreach (Collider2D col in GetComponents<Collider2D>())
            col.enabled = false;

        if (sr != null) sr.color = Color.gray;
        SoltarPlasticos();
        Destroy(gameObject, 1f);
    }

    private Collider2D ObtenerColliderCuerpo()
    {
        foreach (var c in GetComponents<Collider2D>())
            if (!c.isTrigger) return c;
        return null;
    }

    private void ManejarContactoJugador(Collider2D colJugador, GameObject jugador)
    {
        if (muerto || congelado || yaProcesadoEsteFrame) return;

        PlayerController pc = jugador.GetComponent<PlayerController>();
        if (pc != null && pc.EsInmortal())
        {
            Morir();
            yaProcesadoEsteFrame = true;
            return;
        }

        Rigidbody2D rbJug = jugador.GetComponent<Rigidbody2D>();
        if (rbJug == null) return;

        Collider2D cuerpo = ObtenerColliderCuerpo();
        float pieJugador = colJugador.bounds.min.y;
        float cabezaEnemigo;
        if (cuerpo != null)
            cabezaEnemigo = cuerpo.bounds.max.y;
        else
        {
            Collider2D trigger = null;
            foreach (var c in GetComponents<Collider2D>())
                if (c.isTrigger) { trigger = c; break; }
            if (trigger != null)
                cabezaEnemigo = trigger.bounds.max.y;
            else if (sr != null)
                cabezaEnemigo = sr.bounds.max.y;
            else
                return;
        }

        yaProcesadoEsteFrame = true;

        if (rbJug.velocity.y < -0.1f && pieJugador >= cabezaEnemigo - 0.1f)
        {
            rbJug.velocity = new Vector2(rbJug.velocity.x, 6f);
            Morir();
        }
        else
        {
            MuerteJugador muerte = jugador.GetComponent<MuerteJugador>();
            if (muerte != null)
                muerte.MorirPorEnemigo();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (muerto || congelado) return;
        if (!col.gameObject.CompareTag("Player")) return;
        ManejarContactoJugador(col.otherCollider, col.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (muerto || congelado) return;

        if (other.CompareTag("Player"))
        {
            ManejarContactoJugador(other, other.gameObject);
            return;
        }

        if (other.GetComponent<Enemigo>() != null || other.GetComponent<EnemigoVolador>() != null)
        {
            other.SendMessage("RecibirDanio", 1, SendMessageOptions.DontRequireReceiver);
            Morir();
        }
    }

    public void RecibirDanio(int cantidad)
    {
        if (muerto) return;
        Morir();
    }

    public void Congelar()
    {
        congelado = true;
        if (rb != null) rb.velocity = Vector2.zero;
        if (sr != null)
        {
            colorOriginal = sr.color;
            sr.color = new Color(0.5f, 0.8f, 1f);
        }
        Invoke("Restaurar", 5f);
    }

    void Restaurar()
    {
        congelado = false;
        if (sr != null) sr.color = colorOriginal;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
