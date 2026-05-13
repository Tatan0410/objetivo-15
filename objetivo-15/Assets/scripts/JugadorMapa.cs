using UnityEngine;
using UnityEngine.SceneManagement;

public class JugadorMapa : MonoBehaviour
{
    [Header("Nodos principales (niveles)")]
    public Transform[] nodos;

    [Header("Puntos del camino entre nodos")]
    // Cada elemento es un grupo de puntos entre un nodo y el siguiente
    public CaminoEntreNodos[] caminos;

    public float velocidadMovimiento = 3f;

    private int nodoActual = 0;
    private bool moviendose = false;
    private Transform[] rutaActual;
    private int puntoRutaActual = 0;
    private bool yendo = true; // true = avanzando, false = retrocediendo

    void Start()
    {
        if (!PlayerPrefs.HasKey("NivelDesbloqueado"))
            PlayerPrefs.SetInt("NivelDesbloqueado", 0);

        nodoActual = PlayerPrefs.GetInt("NodoActual", 0);

        if (nodos.Length > 0)
            transform.position = nodos[nodoActual].position;
    }

    void Update()
    {
        if (moviendose)
        {
            MoverPorRuta();
            return;
        }

        int nivelDesbloqueado = PlayerPrefs.GetInt("NivelDesbloqueado");

        if (Input.GetKeyDown(KeyCode.RightArrow) && nodoActual < nodos.Length - 1)
        {
            if (nodoActual + 1 <= nivelDesbloqueado)
            {
                // Construye la ruta: puntos intermedios + nodo destino
                rutaActual = ObtenerRuta(nodoActual, true);
                puntoRutaActual = 0;
                yendo = true;
                moviendose = true;
                nodoActual++;
            }
            else
            {
                Debug.Log("¡Nivel bloqueado!");
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && nodoActual > 0)
        {
            // Ruta al revés
            rutaActual = ObtenerRuta(nodoActual - 1, false);
            puntoRutaActual = 0;
            yendo = false;
            moviendose = true;
            nodoActual--;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            EntrarNivel();
    }

    // Construye el array de puntos a seguir
    Transform[] ObtenerRuta(int indiceCamino, bool avanzando)
    {
        if (indiceCamino >= caminos.Length || caminos[indiceCamino].puntos.Length == 0)
        {
            // Si no hay camino definido, va directo al nodo
            return new Transform[] { nodos[avanzando ? indiceCamino + 1 : indiceCamino] };
        }

        Transform[] puntosIntermedios = caminos[indiceCamino].puntos;
        Transform nodoDestino = nodos[avanzando ? indiceCamino + 1 : indiceCamino];

        // Arma la ruta completa
        if (avanzando)
        {
            Transform[] ruta = new Transform[puntosIntermedios.Length + 1];
            for (int i = 0; i < puntosIntermedios.Length; i++)
                ruta[i] = puntosIntermedios[i];
            ruta[ruta.Length - 1] = nodoDestino;
            return ruta;
        }
        else
        {
            // Al revés: nodo destino + puntos al revés
            Transform[] ruta = new Transform[puntosIntermedios.Length + 1];
            ruta[0] = nodoDestino;
            for (int i = 0; i < puntosIntermedios.Length; i++)
                ruta[i + 1] = puntosIntermedios[puntosIntermedios.Length - 1 - i];
            return ruta;
        }
    }

    void MoverPorRuta()
    {
        if (puntoRutaActual >= rutaActual.Length)
        {
            moviendose = false;
            return;
        }

        Vector3 destino = rutaActual[puntoRutaActual].position;
        transform.position = Vector3.MoveTowards(
            transform.position, destino, velocidadMovimiento * Time.deltaTime);

        if (Vector3.Distance(transform.position, destino) < 0.01f)
        {
            transform.position = destino;
            puntoRutaActual++;

            // Si llegó al último punto, terminó
            if (puntoRutaActual >= rutaActual.Length)
                moviendose = false;
        }
    }

    void EntrarNivel()
    {
        PlayerPrefs.SetInt("NodoActual", nodoActual);
        PlayerPrefs.Save();

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

// Agrupa los puntos intermedios de cada camino
[System.Serializable]
public class CaminoEntreNodos
{
    public string nombre; // Solo para identificarlo en el Inspector
    public Transform[] puntos;
}