using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;
    public Vector3 ultimoCheckpoint;
    public GameObject jugador;

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
            ultimoCheckpoint = jugador.transform.position;
    }

    public void GuardarCheckpoint(Vector3 posicion)
    {
        ultimoCheckpoint = posicion;
    }

    public void RespawnJugador(GameObject obj)
    {
        if (obj == null) return;

        obj.transform.position = ultimoCheckpoint;

        // Resetea velocidad para que no siga con inercia
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;
    }
}