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

        // Respeta la inmortalidad del power-up
        PlayerController pc = GameManager.instancia?.jugador?
            .GetComponent<PlayerController>();
        if (pc != null && pc.EsInmortal()) return;

        vidasActuales--;
        ActualizarUI();

        if (vidasActuales <= 0)
            MorirJugador();
        else
        {
            if (GameManager.instancia != null &&
                GameManager.instancia.jugador != null)
                GameManager.instancia.RespawnJugador(
                    GameManager.instancia.jugador);

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
            Debug.Log("❤️ Vida ganada! Vidas: " + vidasActuales);
        }
        else
        {
            Debug.Log("❤️ Ya tienes el máximo de vidas: " + vidasMaximas);
        }
    }

    void QuitarInvencibilidad() => esInvencible = false;

    void MorirJugador()
    {
        vidasActuales = vidasMaximas;
        ActualizarUI();

        // ✅ Limpiar checkpoint en memoria para que no persista a la próxima sesión
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
        if (textoVidas != null)
            textoVidas.text = "Vidas: " + vidasActuales;
    }
}