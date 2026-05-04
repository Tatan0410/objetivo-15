using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class VidasManager : MonoBehaviour
{
    public static VidasManager instancia;

    [Header("Configuración")]
    public int vidasMaximas = 3;
    public int vidasActuales;

    [Header("Invencibilidad temporal")]
    public float tiempoInvencible = 1.5f;
    private bool esInvencible = false;

    [Header("UI")]
    public TMP_Text textoVidas;
    public GameObject panelHUD; // ← arrastra aquí el Canvas/Panel del HUD

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
        vidasActuales = vidasMaximas;
        ActualizarUI();
    }

    public void AsignarTexto(TMP_Text texto)
    {
        textoVidas = texto;
        ActualizarUI();
    }

    public void PerderVida()
    {
        if (esInvencible) return;

        vidasActuales--;
        ActualizarUI();

        if (vidasActuales <= 0)
            MorirJugador();
        else
        {
            esInvencible = true;
            Invoke("QuitarInvencibilidad", tiempoInvencible);

            // Respawn en el mismo nivel
            if (GameManager.instancia != null && GameManager.instancia.jugador != null)
                GameManager.instancia.RespawnJugador(GameManager.instancia.jugador);
        }
    }

    void QuitarInvencibilidad() => esInvencible = false;

    void MorirJugador()
    {
        // ✅ BUG FIX: Ocultar HUD antes de cambiar escena
        if (panelHUD != null)
            panelHUD.SetActive(false);

        vidasActuales = vidasMaximas;
        ActualizarUI();

        // Carga la escena y espera que termine para volver a mostrar el HUD
        SceneManager.sceneLoaded += OnEscenaCargada;
        SceneManager.LoadScene("Mapamundial");
    }

    // ✅ BUG FIX: Se ejecuta cuando la escena nueva ya cargó
    void OnEscenaCargada(Scene escena, LoadSceneMode modo)
    {
        SceneManager.sceneLoaded -= OnEscenaCargada; // desuscribirse

        // Volver a mostrar el HUD
        if (panelHUD != null)
            panelHUD.SetActive(true);

        // Reasignar el jugador (es un nuevo objeto en la escena nueva)
        GameObject jugadorNuevo = GameObject.FindGameObjectWithTag("Player");
        if (jugadorNuevo != null && GameManager.instancia != null)
        {
            GameManager.instancia.jugador = jugadorNuevo;
            GameManager.instancia.ultimoCheckpoint = jugadorNuevo.transform.position;
        }
    }

    void ActualizarUI()
    {
        if (textoVidas != null)
            textoVidas.text = "Vidas: " + vidasActuales;
    }
}