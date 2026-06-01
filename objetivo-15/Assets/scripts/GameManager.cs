using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("Checkpoint actual")]
    public Vector3 ultimoCheckpoint;

    [Header("Referencia jugador")]
    public GameObject jugador;

    private Vector3 posicionInicialJugador;
    private bool checkpointGuardado = false;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (jugador != null)
        {
            posicionInicialJugador = jugador.transform.position;

            if (!checkpointGuardado)
            {
                ultimoCheckpoint = posicionInicialJugador;
            }
        }
    }

    // ═══════════════════════════════════════
    // GUARDAR CHECKPOINT
    // ═══════════════════════════════════════

    public void GuardarCheckpoint(Vector3 posicion)
    {
        ultimoCheckpoint = posicion;
        checkpointGuardado = true;

        Debug.Log("Checkpoint guardado en: " + posicion);
    }

    // ═══════════════════════════════════════
    // RESPAWN DEL JUGADOR
    // ═══════════════════════════════════════

    public void RespawnJugador(GameObject obj)
    {
        if (obj == null) return;

        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Detener TODO movimiento
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;

            // Apagar física temporalmente
            rb.simulated = false;
        }

        // Mover al checkpoint
        obj.transform.position = ultimoCheckpoint;

        // Resetear rotación
        obj.transform.rotation = Quaternion.identity;

        // Reactivar física
        if (rb != null)
        {
            rb.simulated = true;
        }

        Debug.Log("Jugador respawneado en: " + ultimoCheckpoint);
    }

    // ═══════════════════════════════════════
    // RESETEAR CHECKPOINT
    // ═══════════════════════════════════════

    public void ResetearCheckpoint()
    {
        checkpointGuardado = false;

        // Volver al inicio del nivel
        ultimoCheckpoint = posicionInicialJugador;
    }

    // ═══════════════════════════════════════
    // CAMBIO DE ESCENA
    // ═══════════════════════════════════════

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnEscenaCargada;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnEscenaCargada;
    }

    void OnEscenaCargada(Scene escena, LoadSceneMode modo)
    {
        // Ignorar mapa mundial
        if (escena.name == "Mapamundial")
            return;

        GameObject jugadorNuevo = GameObject.FindGameObjectWithTag("Player");

        if (jugadorNuevo != null)
        {
            jugador = jugadorNuevo;

            posicionInicialJugador = jugador.transform.position;

            if (checkpointGuardado)
            {
                RespawnJugador(jugador);
            }
            else
            {
                ultimoCheckpoint = posicionInicialJugador;
            }
        }
    }
}