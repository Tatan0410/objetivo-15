using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPausa : MonoBehaviour
{
    [Header("Panel de pausa")]
    public GameObject panelPausa;

    [Header("Volumen")]
    public Slider sliderVolumen;

    private bool pausado = false;

    void Start()
    {
        panelPausa.SetActive(false);

        float volumenGuardado = PlayerPrefs.GetFloat("Volumen", 1f);
        AudioListener.volume = volumenGuardado;

        if (sliderVolumen != null)
        {
            sliderVolumen.value = volumenGuardado;
            sliderVolumen.onValueChanged.AddListener(CambiarVolumen);
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
        panelPausa.SetActive(true);
        Time.timeScale = 0f;
        pausado = true;
    }

    public void Reanudar()
    {
        panelPausa.SetActive(false);
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

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CambiarVolumen(float valor)
    {
        AudioListener.volume = valor;
        PlayerPrefs.SetFloat("Volumen", valor);
        PlayerPrefs.Save();
    }

    public void SalirAlMenu()
    {
        Time.timeScale = 1f;
        pausado = false;
        SceneManager.LoadScene("MenuPrincipal");
    }
}
