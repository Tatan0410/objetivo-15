using UnityEngine;

public class Enemigo : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 5f;
    public float speed = 2f;

    [Header("Detección de bordes")]
    public LayerMask capaSuelo;
    public float distanciaBorde = 0.4f;
    public float alturaBorde = 0.3f;

    [Header("Drop de plasticos")]
    public GameObject[] prefabsPlasticos;
    public int cantidadDrop = 1;
    [Range(0f, 1f)]
    public float probabilidadDrop = 0.5f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool muerto = false;
    private bool congelado = false;
    private SpriteRenderer sr;
    private Color colorOriginal;
    private bool yaProcesadoEsteFrame = false;
    private float direccionVisual = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) colorOriginal = sr.color;
    }

    void Update()
    {
        if (player == null || muerto || congelado) return;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            movement = new Vector2(direction.x, 0f);
        }
        else
        {
            movement = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        yaProcesadoEsteFrame = false;
        if (rb == null || muerto || congelado) return;

        if (movement.x != 0f)
        {
            direccionVisual = movement.x > 0f ? 1f : -1f;

            if (!HaySueloAdelante(direccionVisual))
                movement = Vector2.zero;
        }

        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    bool HaySueloAdelante(float dir)
    {
        if (capaSuelo == 0) return true;

        Vector3 origen = transform.position + new Vector3(dir * distanciaBorde, -alturaBorde, 0f);
        RaycastHit2D hit = Physics2D.Raycast(origen, Vector2.down, alturaBorde + 0.1f, capaSuelo);
        return hit.collider != null;
    }

    void SoltarPlasticos()
    {
        if (Random.value > probabilidadDrop) return;
        if (prefabsPlasticos == null || prefabsPlasticos.Length == 0) return;

        GameObject runner = new GameObject("PlasticoSpawnRunnerEnemigo");
        runner.AddComponent<PlasticoSpawnRunner>().Iniciar(
            prefabsPlasticos,
            cantidadDrop,
            transform.position,
            0.5f
        );
    }

    void Morir()
    {
        if (muerto) return;
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

    public void RecibirDanio(int cantidad)
    {
        Morir();
    }

    public void Congelar()
    {
        if (muerto) return;
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

        string fuente = cuerpo != null ? "cuerpo" : "trigger/sr";
        Debug.Log($"[{gameObject.name}] pieJugador={pieJugador:F3} cabezaEnemigo={cabezaEnemigo:F3} diferencia={pieJugador - cabezaEnemigo:F3} fuente={fuente}");

        if (pieJugador >= cabezaEnemigo - 0.1f)
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
        if (!other.CompareTag("Player")) return;
        ManejarContactoJugador(other, other.gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (capaSuelo != 0)
        {
            Gizmos.color = Color.green;
            Vector3 origen = transform.position + new Vector3(distanciaBorde, -alturaBorde, 0f);
            Gizmos.DrawLine(origen, origen + Vector3.down * (alturaBorde + 0.1f));
            origen = transform.position + new Vector3(-distanciaBorde, -alturaBorde, 0f);
            Gizmos.DrawLine(origen, origen + Vector3.down * (alturaBorde + 0.1f));
        }
    }
}
