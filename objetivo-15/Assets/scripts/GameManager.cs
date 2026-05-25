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
                jugadorNuevo.transform.position = ultimoCheckpoint;
            // Si no hay checkpoint, el jugador queda donde está en la escena
        }
    }
}