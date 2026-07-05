using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VidasManager : MonoBehaviour
{
    public static VidasManager instancia;

    [Header("Configuración")]
    public int vidasMaximas = 5;
    public int vidasIniciales = 3;
    public int vidasActuales;

    [Header("Invencibilidad temporal")]
    public float tiempoInvencible = 1.5f;
    private bool esInvencible = false;

    [Header("UI - Corazones")]
    public Image[] corazones;
    // TODO: reemplazar sprites placeholder con corazones reales
    public Sprite corazonLleno;
    public Sprite corazonVacio;
    public GameObject panelHUD;

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
        if (corazonLleno == null)
            corazonLleno = IconoUtils.GenerarCorazon(32, 100, new Color(1f, 0.2f, 0.2f));
        if (corazonVacio == null)
            corazonVacio = IconoUtils.GenerarCorazon(32, 100, new Color(0.3f, 0.3f, 0.3f));

        vidasActuales = vidasIniciales;
        ActualizarUI();
    }

    public void AsignarCorazones(Image[] imagenes)
    {
        corazones = imagenes;
        ActualizarUI();
    }

    public void PerderVida()
    {
        if (esInvencible) return;

        PlayerController pc = GameManager.instancia?.jugador?
            .GetComponent<PlayerController>();
        if (pc != null && pc.EsInmortal()) return;

        vidasActuales--;
        ActualizarUI();

        if (vidasActuales <= 0)
            MorirJugador();
        else
        {
            esInvencible = true;
            Invoke("QuitarInvencibilidad", tiempoInvencible);
        }
    }

    public void AgregarVida()
    {
        if (vidasActuales < vidasMaximas)
        {
            vidasActuales++;
            ActualizarUI();
            Debug.Log("VIDA GANADA! Vidas: " + vidasActuales);
        }
        else
        {
            Debug.Log("Ya tienes el maximo de vidas: " + vidasMaximas);
        }
    }

    void QuitarInvencibilidad() => esInvencible = false;

    void MorirJugador()
    {
        vidasActuales = vidasIniciales;
        ActualizarUI();

        if (GameManager.instancia != null)
            GameManager.instancia.ResetearCheckpoint();

        SceneManager.LoadScene("Mapamundial");
    }

    void OnEscenaCargada(Scene escena, LoadSceneMode modo)
    {
        SceneManager.sceneLoaded -= OnEscenaCargada;
        if (panelHUD != null)
            panelHUD.SetActive(true);
    }

    void ActualizarUI()
    {
        if (corazones == null) return;

        for (int i = 0; i < corazones.Length; i++)
        {
            if (corazones[i] == null) continue;
            corazones[i].sprite = i < vidasActuales ? corazonLleno : corazonVacio;
        }
    }
}
