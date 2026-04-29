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
        // Por defecto solo el nivel 1 está desbloqueado
        if (!PlayerPrefs.HasKey("NivelDesbloqueado"))
            PlayerPrefs.SetInt("NivelDesbloqueado", 0);
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
                Debug.Log("Nivel bloqueado! Completa el nivel anterior.");
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && nodoActual > 0)
        {
            nodoActual--;
            destino = nodos[nodoActual].position;
            moviendose = true;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            EntrarNivel();
        }
    }

    void EntrarNivel()
    {
        switch (nodoActual)
        {
            case 0: SceneManager.LoadScene("Nivel1_Colegio"); break;
            case 1: SceneManager.LoadScene("Nivel2_Hipodromo"); break;
            case 2: SceneManager.LoadScene("Nivel3_Mercado"); break;
            case 3: SceneManager.LoadScene("Nivel4_Basurero"); break;
            case 4: SceneManager.LoadScene("Nivel5_Subterraneo"); break;
            case 5: SceneManager.LoadScene("Nivel6_EmpresaFinal"); break;
        }
    }
}