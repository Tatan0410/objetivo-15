using UnityEngine;
using System.Collections;

public class Enemigo : MonoBehaviour
{
    [Header("Patrulla")]
    public float velocidad = 2f;
    public float distanciaPatrulla = 3f;

    [Header("Detección")]
    public Transform detectorBorde;
    public LayerMask capaSuelo;

    [Header("Drop de plásticos")]
    public GameObject[] prefabsPlasticos;
    public int cantidadDrop = 1;
    [Range(0f, 1f)]
    public float probabilidadDrop = 0.5f;

    [Header("Persecucion")]
    public float rangoDeteccion = 4f;
    public float rangoDeteccionVertical = 1.5f;

    private Vector2 puntoInicio;
    private bool moviendoDerecha = true;
    private Rigidbody2D rb;
    private bool muerto = false;
    private bool congelado = false;
    private Transform jugador;
    private Color colorOriginal;
    private SpriteRenderer sr;

    private Vector3 posicionMuerte;
    private bool yaProcesadoEsteFrame = false;

    void Start()
    {
        puntoInicio = transform.position;
        rb = GetComponent<Rigidbody2D>();
        rb.mass = 1000f;
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) colorOriginal = sr.color;
        GameObject j = GameObject.FindGameObjectWithTag("Player");
        if (j != null) jugador = j.transform;
    }

    void FixedUpdate()
    {
        yaProcesadoEsteFrame = false;
        if (muerto || congelado) return;

        if (jugador == null)
        {
            GameObject j = GameObject.FindGameObjectWithTag("Player");
            if (j != null) jugador = j.transform;
        }

        float dist = jugador != null
            ? Vector2.Distance(transform.position, jugador.position)
            : float.MaxValue;

        if (dist <= rangoDeteccion && Mathf.Abs(transform.position.y - jugador.position.y) <= rangoDeteccionVertical)
            Perseguir();
        else
        {
            Patrullar();
            VerificarBorde();
        }
    }

    void Perseguir()
    {
        float dir = jugador.position.x > transform.position.x ? 1f : -1f;

        // Verifica si hay piso adelante con el detectorBorde
        if (detectorBorde != null)
        {
            bool haySuelo = Physics2D.OverlapCircle(
                detectorBorde.position, 0.1f, capaSuelo);
            if (!haySuelo)
            {
                rb.velocity = Vector2.zero;
                return;
            }
        }

        rb.velocity = new Vector2(dir * velocidad, rb.velocity.y);

        // Voltear visual hacia el jugador sin tocar puntoInicio
        if (dir > 0 && transform.localScale.x < 0)
            transform.localScale = new Vector3(
                -transform.localScale.x,
                transform.localScale.y,
                transform.localScale.z);
        else if (dir < 0 && transform.localScale.x > 0)
            transform.localScale = new Vector3(
                -transform.localScale.x,
                transform.localScale.y,
                transform.localScale.z);
    }

    void Patrullar()
    {
        float dir = moviendoDerecha ? 1f : -1f;
        rb.velocity = new Vector2(dir * velocidad, rb.velocity.y);

        if (moviendoDerecha &&
            transform.position.x >= puntoInicio.x + distanciaPatrulla)
            Voltear();
        else if (!moviendoDerecha &&
            transform.position.x <= puntoInicio.x - distanciaPatrulla)
            Voltear();
    }

    void VerificarBorde()
    {
        if (detectorBorde == null) return;
        bool haySuelo = Physics2D.OverlapCircle(
            detectorBorde.position, 0.1f, capaSuelo);
        if (!haySuelo) Voltear();

        Vector2 direccion = moviendoDerecha ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position, direccion, 0.6f, capaSuelo);
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

    void SoltarPlasticos()
    {
        if (Random.value > probabilidadDrop) return;
        if (prefabsPlasticos.Length == 0) return;

        // ✅ FIX: guardamos posición ANTES de destruir el objeto
        // y usamos un GameObject temporal para correr la coroutine
        posicionMuerte = transform.position;

        // Creamos un objeto vacío temporal que sobrevive al enemigo
        // y spawnea los plásticos después de 0.5s
        GameObject runner = new GameObject("PlasticoSpawnRunner");
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
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        foreach (Collider2D col in GetComponents<Collider2D>())
            col.enabled = false;

        GetComponent<SpriteRenderer>().color = Color.gray;
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
        Collider2D cuerpo = ObtenerColliderCuerpo();
        if (rbJug == null || cuerpo == null) return;

        float pieJugador = colJugador.bounds.min.y;
        float cabezaEnemigo = cuerpo.bounds.max.y;

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
        if (!other.CompareTag("Player")) return;
        ManejarContactoJugador(other, other.gameObject);
    }

    public void RecibirDanio(int cantidad)
    {
        Morir();
    }

    public void Congelar()
    {
        congelado = true;
        rb.velocity = Vector2.zero;
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaPatrulla);
        if (detectorBorde != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(detectorBorde.position, 0.1f);
        }
    }
}

// ✅ Clase auxiliar: objeto temporal que spawnea los plásticos con delay
// y luego se destruye solo — no depende del enemigo que ya murió
public class PlasticoSpawnRunner : MonoBehaviour
{
    public void Iniciar(GameObject[] prefabs, int cantidad, Vector3 posicion, float delay)
    {
        StartCoroutine(SpawnConDelay(prefabs, cantidad, posicion, delay));
    }

    IEnumerator SpawnConDelay(GameObject[] prefabs, int cantidad, Vector3 posicion, float delay)
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < cantidad; i++)
        {
            int random = Random.Range(0, prefabs.Length);
            // Spawnear ligeramente separado para que no se acumulen
            Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), 0.3f, 0);
            Instantiate(prefabs[random], posicion + offset, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}