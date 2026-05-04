using UnityEngine;
using UnityEngine.SceneManagement;

public class JugadorMapa : MonoBehaviour
{
    public Transform[] nodos;
    public int nodoActual = 0;
    public float velocidadMovimiento = 3f;
    private bool moviendose = false;
    private Vector3 destino;

    void Start()
    {
        if (!PlayerPrefs.HasKey("NivelDesbloqueado"))
            PlayerPrefs.SetInt("NivelDesbloqueado", 0);

        // Posicionar al jugador en el nodo actual
        if (nodos.Length > 0)
            transform.position = nodos[nodoActual].position;
    }

    void Update()
    {
        if (moviendose)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, destino, velocidadMovimiento * Time.deltaTime);
            if (Vector3.Distance(transform.position, destino) < 0.01f)
            {
                transform.position = destino;
                moviendose = false;
            }
            return;
        }

        int nivelDesbloqueado = PlayerPrefs.GetInt("NivelDesbloqueado");

        if (Input.GetKeyDown(KeyCode.RightArrow) && nodoActual < nodos.Length - 1)
        {
            if (nodoActual + 1 <= nivelDesbloqueado)
            {
                nodoActual++;
                destino = nodos[nodoActual].position;
                moviendose = true;
            }
            else
            {
                Debug.Log("Nivel bloqueado!");
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && nodoActual > 0)
        {
            nodoActual--;
            destino = nodos[nodoActual].position;
            moviendose = true;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            EntrarNivel();
    }

    void EntrarNivel()
    {
        switch (nodoActual)
        {
            case 0: SceneManager.LoadScene("nivel1_colegio"); break;
            case 1: SceneManager.LoadScene("nivel2_hipodromo"); break;
            case 2: SceneManager.LoadScene("nivel3_mercado"); break;
            case 3: SceneManager.LoadScene("nivel4_basurero"); break;
            case 4: SceneManager.LoadScene("nivel5_subterraneo"); break;
            case 5: SceneManager.LoadScene("nivel6_empresa"); break;
        }
    }
}