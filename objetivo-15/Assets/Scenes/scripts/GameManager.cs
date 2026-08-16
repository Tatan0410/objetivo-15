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
        if (jugador == null)
            jugador = GameObject.FindGameObjectWithTag("Player");

        if (jugador != null)
        {
            posicionInicialJugador = jugador.transform.position;
            ultimoCheckpoint = posicionInicialJugador;
        }
        else
        {
            Debug.LogError("GameManager: no se encontró ningún jugador con tag 'Player'");
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

        // ✅ Teletransportar usando rb.position Y transform.position
        // para garantizar que el motor de físicas lo registre correctamente
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.position = ultimoCheckpoint;
        }
        obj.transform.position = ultimoCheckpoint;

        Debug.Log("Respawn en: " + ultimoCheckpoint);
    }

    public void ResetearCheckpoint()
    {
        ultimoCheckpoint = posicionInicialJugador;
        Debug.Log("Checkpoint reseteado al inicio");
    }

    void OnEnable() { SceneManager.sceneLoaded += OnEscenaCargada; }
    void OnDisable() { SceneManager.sceneLoaded -= OnEscenaCargada; }

    void OnEscenaCargada(Scene escena, LoadSceneMode modo)
    {
        if (escena.name == "Mapamundial") return;

        GameObject jugadorNuevo = GameObject.FindGameObjectWithTag("Player");
        if (jugadorNuevo != null)
        {
            jugador = jugadorNuevo;
            posicionInicialJugador = jugadorNuevo.transform.position;
            ultimoCheckpoint = posicionInicialJugador;
        }
    }
}