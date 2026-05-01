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
        }
    }

    void QuitarInvencibilidad() => esInvencible = false;

    void MorirJugador()
    {
        vidasActuales = vidasMaximas;
        ActualizarUI();
        SceneManager.LoadScene("Mapamundial");
    }

    void ActualizarUI()
    {
        if (textoVidas != null)
            textoVidas.text = "Vidas: " + vidasActuales;
    }
}