using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;
    public int vidas = 3;
    public Vector3 ultimoCheckpoint;
    public GameObject jugador;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ultimoCheckpoint = jugador.transform.position;
    }

    public void MorirJugador()
    {
        vidas--;
        if (vidas <= 0)
        {
            // Sin vidas - volver al mapa
            vidas = 3;
            SceneManager.LoadScene("Mapamundial");
        }
        else
        {
            // Revivir en el checkpoint
            jugador.transform.position = ultimoCheckpoint;
        }
    }

    public void GuardarCheckpoint(Vector3 posicion)
    {
        ultimoCheckpoint = posicion;
    }
}