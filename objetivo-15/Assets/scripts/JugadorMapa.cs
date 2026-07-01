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
        nodoActual = Mathf.Clamp(nodoGuardado, 0, nodos.Length - 1);

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

        int nivelDesbloqueado = PlayerPrefs.GetInt("NivelDesbloqueado", 0);

        if (Input.GetKeyDown(KeyCode.RightArrow) &&
            nodoActual < nodos.Length - 1)
        {
            if (nodoActual + 1 <= nivelDesbloqueado)
            {
                rutaActual = ObtenerRutaDirecta(nodoActual);
                puntoRutaActual = 0;
                moviendose = true;
                nodoActual++;
                PlayerPrefs.SetInt("NodoActual", nodoActual);
                PlayerPrefs.Save();
            }
            else
            {
                Debug.Log("🔒 Nivel bloqueado! Debes completar el nivel anterior.");
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && nodoActual > 0)
        {
            rutaActual = ObtenerRutaInversa(nodoActual - 1);
            puntoRutaActual = 0;
            moviendose = true;
            nodoActual--;
            PlayerPrefs.SetInt("NodoActual", nodoActual);
            PlayerPrefs.Save();
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

        // ✅ CLAVE: limpiar checkpoint ANTES de cargar el nivel
        // El GameManager tiene DontDestroyOnLoad y guarda el checkpoint
        // en memoria — si no lo limpiamos aquí, al entrar al nivel
        // el jugador respawnea en el checkpoint de la sesión anterior
        if (GameManager.instancia != null)
            GameManager.instancia.ResetearCheckpoint();

        switch (nodoActual)
        {
            case 0: SceneManager.LoadScene("cutscene_0"); break;
            case 1: SceneManager.LoadScene("cutscene_1"); break;
            case 2: SceneManager.LoadScene("cutscene_2"); break;
            case 3: SceneManager.LoadScene("nivel4_basurero"); break;
            case 4: SceneManager.LoadScene("nivel5_subterraneo"); break;
            case 5: SceneManager.LoadScene("nivel6_empresa"); break;
            default: Debug.LogWarning("Nodo sin escena asignada: " + nodoActual); break;
        }
    }
}

[System.Serializable]
public class CaminoEntreNodos
{
    public string nombre;
    public Transform[] puntos;
}