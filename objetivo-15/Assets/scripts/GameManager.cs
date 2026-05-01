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
            instancia = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        ultimoCheckpoint = jugador.transform.position;
    }

    public void GuardarCheckpoint(Vector3 posicion)
    {
        ultimoCheckpoint = posicion;
    }

    public void RespawnJugador(GameObject obj)
    {
        obj.transform.position = ultimoCheckpoint;
    }
}