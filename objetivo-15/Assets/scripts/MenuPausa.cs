using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPausa : MonoBehaviour
{
    [Header("Panel de pausa (en escena, editable)")]
    public GameObject panelPausa;

    [Header("Volumen")]
    public Slider sliderVolumen;

    private bool pausado = false;

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausado) Reanudar();
            else Pausar();
        }
    }

    public void Pausar()
    {
        if (panelPausa != null) panelPausa.SetActive(true);
        Time.timeScale = 0f;
        pausado = true;
    }

    public void Reanudar()
    {
        if (panelPausa != null) panelPausa.SetActive(false);
        Time.timeScale = 1f;
        pausado = false;
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        pausado = false;

        if (GameManager.instancia != null)
            GameManager.instancia.ResetearCheckpoint();

        if (VidasManager.instancia != null)
        {
            VidasManager.instancia.vidasActuales =
                VidasManager.instancia.vidasIniciales;
        }

        if (SceneTransitionManager.instancia != null)
            SceneTransitionManager.instancia.CargarEscena(SceneManager.GetActiveScene().name);
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
        if (SceneTransitionManager.instancia != null)
            SceneTransitionManager.instancia.CargarEscena("menuprincipal");
    }
}