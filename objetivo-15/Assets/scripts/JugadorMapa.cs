using UnityEngine;
using UnityEngine.SceneManagement;

public class JugadorMapa : MonoBehaviour
{
    [Header("Nodos principales (niveles)")]
    public Transform[] nodos;

    [Header("Puntos del camino entre nodos")]
    public CaminoEntreNodos[] caminos;

    public float velocidadMovimiento = 3f;

    private int nodoActual = 0;
    private bool moviendose = false;
    private Transform[] rutaActual;
    private int puntoRutaActual = 0;

    void Start()
    {
        if (!PlayerPrefs.HasKey("NivelDesbloqueado"))
            PlayerPrefs.SetInt("NivelDesbloqueado", 0);

        int nodoGuardado = PlayerPrefs.GetInt("NodoActual", 0);
        nodoActual = nodoGuardado;

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

        if (Input.GetKeyDown(KeyCode.RightArrow) &&
            nodoActual < nodos.Length - 1)
        {
            if (nodoActual + 1 <= nivelDesbloqueado)
            {
                rutaActual = ObtenerRutaDirecta(nodoActual);
                puntoRutaActual = 0;
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
            rutaActual = ObtenerRutaInversa(nodoActual - 1);
            puntoRutaActual = 0;
            moviendose = true;
            nodoActual--;
        }

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.Space))
            EntrarNivel();
    }

    Transform[] ObtenerRutaDirecta(int indiceCamino)
    {
        Transform nodoDestino = nodos[indiceCamino + 1];

        if (indiceCamino >= caminos.Length ||
            caminos[indiceCamino].puntos.Length == 0)
            return new Transform[] { nodoDestino };

        Transform[] pts = caminos[indiceCamino].puntos;
        Transform[] ruta = new Transform[pts.Length + 1];

        for (int i = 0; i < pts.Length; i++)
            ruta[i] = pts[i];

        ruta[ruta.Length - 1] = nodoDestino;
        return ruta;
    }

    Transform[] ObtenerRutaInversa(int indiceCamino)
    {
        Transform nodoDestino = nodos[indiceCamino];

        if (indiceCamino >= caminos.Length ||
            caminos[indiceCamino].puntos.Length == 0)
            return new Transform[] { nodoDestino };

        Transform[] pts = caminos[indiceCamino].puntos;
        Transform[] ruta = new Transform[pts.Length + 1];

        for (int i = 0; i < pts.Length; i++)
            ruta[i] = pts[pts.Length - 1 - i];

        ruta[ruta.Length - 1] = nodoDestino;
        return ruta;
    }

    void MoverPorRuta()
    {
        if (puntoRutaActual >= rutaActual.Length)
        {
            moviendose = false;
            return;
        }

        Vector3 destino = rutaActual[puntoRutaActual].position;

        Debug.Log("Yendo a punto " + puntoRutaActual +
            " posicion: " + destino +
            " desde: " + transform.position);

        transform.position = Vector3.MoveTowards(
            transform.position, destino,
            velocidadMovimiento * Time.deltaTime);

        if (Vector3.Distance(transform.position, destino) < 0.01f)
        {
            transform.position = destino;
            puntoRutaActual++;

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

[System.Serializable]
public class CaminoEntreNodos
{
    public string nombre;
    public Transform[] puntos;
}