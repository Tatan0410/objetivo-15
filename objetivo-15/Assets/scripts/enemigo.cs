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

    private Vector2 puntoInicio;
    private bool moviendoDerecha = true;
    private Rigidbody2D rb;
    private bool muerto = false;

    // Guardamos la posición de muerte para el spawn,
    // ya que el enemigo se destruye antes de que corra el delay
    private Vector3 posicionMuerte;

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

    void OnCollisionEnter2D(Collision2D col)
    {
        if (muerto) return;
        if (!col.gameObject.CompareTag("Player")) return;

        MuerteJugador muerte = col.gameObject.GetComponent<MuerteJugador>();
        if (muerte != null)
            muerte.MorirPorEnemigo();
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
        else
        {
            MuerteJugador muerte = col.GetComponent<MuerteJugador>();
            if (muerte != null)
                muerte.MorirPorEnemigo();
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