using UnityEngine;

// Cuervo decorativo (NPC): patrulla puntos de vuelo y, cada cierto
// tiempo aleatorio, desciende a un punto de aterrizaje, camina un tramo
// corto, come unos segundos y vuelve a volar.
// No tiene colliders ni fisicas: el jugador no interactua con el.
//
// Animator: usa un parametro int llamado "estado":
//   0 = vuelavuela   (volando, descendiendo, despegando)
//   1 = caminandocuervo
//   2 = comiendokbron
public class CuervoNPC : MonoBehaviour
{
    private enum Estado { Volando, Descendiendo, Caminando, Comiendo, Despegando }

    private const string PARAM_ESTADO = "estado";
    private const int ANIM_VOLAR = 0;
    private const int ANIM_CAMINAR = 1;
    private const int ANIM_COMER = 2;

    [Header("Puntos de vuelo (arriba)")]
    [SerializeField] private Transform[] puntosVuelo;

    [Header("Puntos de aterrizaje (en el suelo)")]
    [SerializeField] private Transform[] puntosAterrizaje;

    [Header("Velocidades")]
    [SerializeField] private float velocidadVuelo = 3f;
    [SerializeField] private float velocidadCaminar = 1.2f;

    [Header("Distancias")]
    [SerializeField] private float distanciaMinima = 0.1f;
    [SerializeField] private float caminataMin = 1f;
    [SerializeField] private float caminataMax = 3f;

    [Header("Cada cuanto decide bajar a comer (segundos)")]
    [SerializeField] private float tiempoDescensoMin = 8f;
    [SerializeField] private float tiempoDescensoMax = 15f;

    [Header("Cuanto dura comiendo (segundos)")]
    [SerializeField] private float tiempoComerMin = 2f;
    [SerializeField] private float tiempoComerMax = 4f;

    private Estado estado = Estado.Volando;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private int puntoVueloActual;
    private Transform puntoAterrizaje;
    private float temporizadorDescenso;
    private float temporizadorComer;
    private float caminataRestante;
    private float direccionCaminata;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (puntosVuelo == null || puntosVuelo.Length == 0)
        {
            Debug.LogWarning("[CuervoNPC] No hay puntos de vuelo asignados en " + gameObject.name);
            enabled = false;
            return;
        }

        puntoVueloActual = Random.Range(0, puntosVuelo.Length);
        ReiniciarTemporizadorDescenso();
        GirarHacia(puntosVuelo[puntoVueloActual].position);
    }

    private void Update()
    {
        switch (estado)
        {
            case Estado.Volando: ActualizarVolando(); break;
            case Estado.Descendiendo: ActualizarDescendiendo(); break;
            case Estado.Caminando: ActualizarCaminando(); break;
            case Estado.Comiendo: ActualizarComiendo(); break;
            case Estado.Despegando: ActualizarDespegando(); break;
        }
    }

    private void ActualizarVolando()
    {
        Vector3 destino = puntosVuelo[puntoVueloActual].position;
        GirarHacia(destino);

        if (MoverHacia(destino, velocidadVuelo))
            puntoVueloActual = Random.Range(0, puntosVuelo.Length);

        temporizadorDescenso -= Time.deltaTime;
        if (temporizadorDescenso <= 0f && puntosAterrizaje != null && puntosAterrizaje.Length > 0)
        {
            puntoAterrizaje = puntosAterrizaje[Random.Range(0, puntosAterrizaje.Length)];
            CambiarEstado(Estado.Descendiendo);
        }
    }

    private void ActualizarDescendiendo()
    {
        Vector3 destino = puntoAterrizaje.position;
        GirarHacia(destino);

        if (MoverHacia(destino, velocidadVuelo))
        {
            // Prepara una caminata corta al azar (izquierda o derecha)
            direccionCaminata = Random.value < 0.5f ? -1f : 1f;
            caminataRestante = Random.Range(caminataMin, caminataMax);
            CambiarEstado(Estado.Caminando);
            GirarHacia(transform.position + Vector3.right * direccionCaminata);
        }
    }

    private void ActualizarCaminando()
    {
        float paso = velocidadCaminar * Time.deltaTime;
        if (paso > caminataRestante) paso = caminataRestante;

        transform.position += Vector3.right * direccionCaminata * paso;
        caminataRestante -= paso;

        if (caminataRestante <= 0f)
        {
            temporizadorComer = Random.Range(tiempoComerMin, tiempoComerMax);
            CambiarEstado(Estado.Comiendo);
        }
    }

    private void ActualizarComiendo()
    {
        temporizadorComer -= Time.deltaTime;
        if (temporizadorComer <= 0f)
        {
            puntoVueloActual = Random.Range(0, puntosVuelo.Length);
            CambiarEstado(Estado.Despegando);
        }
    }

    private void ActualizarDespegando()
    {
        Vector3 destino = puntosVuelo[puntoVueloActual].position;
        GirarHacia(destino);

        if (MoverHacia(destino, velocidadVuelo))
        {
            ReiniciarTemporizadorDescenso();
            CambiarEstado(Estado.Volando);
        }
    }

    // Mueve hacia el destino y devuelve true cuando llego.
    private bool MoverHacia(Vector3 destino, float velocidad)
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            destino,
            velocidad * Time.deltaTime
        );

        return Vector2.Distance(transform.position, destino) < distanciaMinima;
    }

    // El sprite del cuervo mira a la derecha por defecto:
    // flipX = false cuando va a la derecha, true cuando va a la izquierda.
    private void GirarHacia(Vector3 destino)
    {
        if (spriteRenderer == null) return;

        if (transform.position.x < destino.x)
            spriteRenderer.flipX = false; // se mueve a la derecha
        else
            spriteRenderer.flipX = true;  // se mueve a la izquierda
    }

    private void ReiniciarTemporizadorDescenso()
    {
        temporizadorDescenso = Random.Range(tiempoDescensoMin, tiempoDescensoMax);
    }

    private void CambiarEstado(Estado nuevo)
    {
        estado = nuevo;

        if (animator != null)
            animator.SetInteger(PARAM_ESTADO, AnimDeEstado(nuevo));
    }

    private static int AnimDeEstado(Estado e)
    {
        switch (e)
        {
            case Estado.Caminando: return ANIM_CAMINAR;
            case Estado.Comiendo: return ANIM_COMER;
            default: return ANIM_VOLAR; // Volando, Descendiendo y Despegando
        }
    }
}
