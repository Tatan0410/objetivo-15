using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;
    public Vector3 ultimoCheckpoint;
    public GameObject jugador;
    private Vector3 posicionInicialJugador;

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
            ultimoCheckpoint = posicionInicialJugador;
        }
    }

    public void GuardarCheckpoint(Vector3 posicion)
    {
        ultimoCheckpoint = posicion;
        Debug.Log("Checkpoint guardado: " + posicion);
    }

    public void RespawnJugador(GameObject obj)
    {
        if (obj == null) return;
        obj.transform.position = ultimoCheckpoint;
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;
    }

    // Se llama automáticamente cuando carga una escena nueva
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
        // Buscar el jugador en la nueva escena
        GameObject jugadorNuevo = GameObject.FindGameObjectWithTag("Player");
        if (jugadorNuevo != null)
        {
            jugador = jugadorNuevo;
            // Resetear checkpoint al inicio de la escena nueva
            ultimoCheckpoint = jugadorNuevo.transform.position;
        }
    }
}