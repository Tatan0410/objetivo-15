using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPausa : MonoBehaviour
{
    public static MenuPausa instancia;

    [Header("Panel de pausa (en escena, editable)")]
    public GameObject panelPausa;

    [Header("Volumen")]
    public Slider sliderVolumen;

    public static bool JuegoPausado { get; private set; }

    private bool pausado = false;

    void Awake()
    {
        if (instancia != null) { Destroy(gameObject); return; }
        instancia = this;
        JuegoPausado = false;
    }

    void Start()
    {
        if (panelPausa != null)
            panelPausa.SetActive(false);

        float volumenGuardado = PlayerPrefs.GetFloat("VolumenMusica", 1f);
        AplicarVolumen(volumenGuardado);

        if (sliderVolumen != null)
        {
            sliderVolumen.value = volumenGuardado;
            sliderVolumen.onValueChanged.AddListener(CambiarVolumen);
        }

        ConectarBotonPausa();
    }

    // El boton BtnPausa quedaba conectado a un metodo estatico del ensamblado de
    // Editor (SetupEscenas.PausarJuego), por lo que al hacer click no hacia nada.
    // Aqui lo reconectamos a la instancia real de MenuPausa en runtime.
    void ConectarBotonPausa()
    {
        var btnGO = GameObject.Find("BtnPausa");
        if (btnGO == null) return;

        Button btn = btnGO.GetComponent<Button>();
        if (btn == null) return;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(Pausar);
    }

    void AplicarVolumen(float valor)
    {
        GameObject musicGO = GameObject.Find("musicafondo");
        if (musicGO != null)
        {
            AudioSource[] audios = musicGO.GetComponents<AudioSource>();
            foreach (var a in audios)
                a.volume = valor;
        }
    }

    void Update()
    {
        bool togglePausa = Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Pause");
        bool volverAtras = Input.GetButtonDown("Cancel");

        if (togglePausa)
        {
            if (pausado) Reanudar();
            else Pausar();
            return;
        }

        if (volverAtras && pausado)
            Reanudar();
    }

    public void Pausar()
    {
        if (panelPausa != null)
        {
            panelPausa.SetActive(true);
            SeleccionUI.SeleccionarPrimero(panelPausa);
        }
        Time.timeScale = 0f;
        pausado = true;
        JuegoPausado = true;
    }

    public void Reanudar()
    {
        if (panelPausa != null) panelPausa.SetActive(false);
        SeleccionUI.LimpiarSeleccion();
        Time.timeScale = 1f;
        pausado = false;
        JuegoPausado = false;
    }

    public void ReiniciarNivel()
    {
        if (GameOverManager.instancia != null)
        {
            var canvasField = typeof(GameOverManager).GetField("canvasGameOver", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var canvas = canvasField?.GetValue(GameOverManager.instancia) as GameObject;
            if (canvas != null) canvas.SetActive(false);
        }
        SeleccionUI.LimpiarSeleccion();
        Time.timeScale = 1f;
        pausado = false;
        JuegoPausado = false;

        if (VidasManager.instancia != null)
        {
            VidasManager.instancia.vidasActuales =
                VidasManager.instancia.vidasIniciales;
        }

        SceneTransitionManager.CargarEscenaConFallback(SceneManager.GetActiveScene().name);
    }

    public void CambiarVolumen(float valor)
    {
        AplicarVolumen(valor);
        PlayerPrefs.SetFloat("VolumenMusica", valor);
        PlayerPrefs.Save();
    }

    public void SalirAlMenu()
    {
        Time.timeScale = 1f;
        pausado = false;
        JuegoPausado = false;
        SceneTransitionManager.CargarEscenaConFallback("menuprincipal");
    }
}
