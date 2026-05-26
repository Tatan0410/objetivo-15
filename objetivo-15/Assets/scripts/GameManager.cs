using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    public Vector3 ultimoCheckpoint;
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
                ultimoCheckpoint = posicionInicialJugador;
        }
    }

    public void GuardarCheckpoint(Vector3 posicion)
    {
        ultimoCheckpoint = posicion;
        checkpointGuardado = true;
        Debug.Log("Checkpoint guardado: " + posicion);
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
        // ✅ FIX: resetear ultimoCheckpoint a la posición inicial
        // para que al reentrar el nivel, el jugador aparezca al inicio
        ultimoCheckpoint = posicionInicialJugador;
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
        // Si cargamos el mapa mundial, no buscar jugador
        if (escena.name == "Mapamundial") return;

        GameObject jugadorNuevo = GameObject.FindGameObjectWithTag("Player");
        if (jugadorNuevo != null)
        {
            jugador = jugadorNuevo;

            // ✅ Guardar posición inicial del jugador en esta escena
            // ANTES de moverlo, por si no hay checkpoint guardado
            posicionInicialJugador = jugadorNuevo.transform.position;

            if (checkpointGuardado)
            {
                // Hay checkpoint activo — respawnear ahí
                jugadorNuevo.transform.position = ultimoCheckpoint;
                Rigidbody2D rb = jugadorNuevo.GetComponent<Rigidbody2D>();
                if (rb != null) rb.velocity = Vector2.zero;
            }
            else
            {
                // Sin checkpoint — el jugador queda en su posición inicial de la escena
                ultimoCheckpoint = posicionInicialJugador;
            }
        }
    }
}