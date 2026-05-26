using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    public Vector3 ultimoCheckpoint;
    public GameObject jugador;

    private Vector3 posicionInicialJugador;
    private bool checkpointGuardado = false;

    // Offset para que el jugador aparezca ENCIMA del checkpoint, no dentro del suelo
    private const float RESPAWN_OFFSET_Y = 1f;

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
                ultimoCheckpoint = posicionInicialJugador;
        }
    }

    public void GuardarCheckpoint(Vector3 posicion)
    {
        // ✅ Guardamos con offset Y para que el respawn sea sobre el suelo
        ultimoCheckpoint = new Vector3(posicion.x, posicion.y + RESPAWN_OFFSET_Y, posicion.z);
        checkpointGuardado = true;
        Debug.Log("Checkpoint guardado: " + ultimoCheckpoint);
    }

    public void RespawnJugador(GameObject obj)
    {
        if (obj == null) return;
        obj.transform.position = ultimoCheckpoint;
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;
    }

    public void ResetearCheckpoint()
    {
        checkpointGuardado = false;
        // ✅ Resetear la posición también para que no quede la vieja en memoria
        // Se actualizará con la posición inicial del jugador al cargar la escena
        ultimoCheckpoint = Vector3.zero;
        Debug.Log("Checkpoint reseteado");
    }

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
        GameObject jugadorNuevo = GameObject.FindGameObjectWithTag("Player");
        if (jugadorNuevo != null)
        {
            jugador = jugadorNuevo;

            if (checkpointGuardado)
            {
                // Hay checkpoint activo — respawnear ahí
                jugadorNuevo.transform.position = ultimoCheckpoint;
                Rigidbody2D rb = jugadorNuevo.GetComponent<Rigidbody2D>();
                if (rb != null) rb.velocity = Vector2.zero;
            }
            else
            {
                // ✅ Sin checkpoint — guardar posición inicial de la escena
                // para que futuros respawns sin checkpoint vayan al inicio del nivel
                posicionInicialJugador = jugadorNuevo.transform.position;
                ultimoCheckpoint = posicionInicialJugador;
            }
        }
    }
}