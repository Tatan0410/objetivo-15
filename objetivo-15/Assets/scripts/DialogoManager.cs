using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

[System.Serializable]
public class Dialogo
{
    [TextArea(3, 6)]
    public string texto;
    public AudioClip vozAudio;
}

public class DialogoManager : MonoBehaviour
{
    [Header("Personaje - Planeta Tierra")]
    public Image imagenPlaneta;
    public Sprite spriteBocaCerrada;
    public Sprite spriteBocaAbierta;
    public float velocidadBoca = 0.12f;

    [Header("UI Viñeta (speech bubble)")]
    public TMP_Text textoDialogo;

    [Header("Botones")]
    public GameObject botonSiguiente;
    public TMP_Text textoBotonSiguiente;
    public GameObject botonSkip;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Configuración")]
    public Dialogo[] dialogos;
    public string escenaDestino;
    public string textoSiguienteUltimo = "¡Jugar!";
    public float velocidadTexto = 0.03f;

    private int indiceActual = 0;
    private bool escribiendo = false;
    private Coroutine corrutinaBoca;

    void Start()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("Volumen", 1f);
        botonSiguiente.SetActive(false);
        botonSkip.SetActive(true);
        if (imagenPlaneta != null && spriteBocaCerrada != null)
            imagenPlaneta.sprite = spriteBocaCerrada;

        if (botonSiguiente != null)
            botonSiguiente.GetComponent<Button>().onClick.AddListener(Siguiente);
        if (botonSkip != null)
            botonSkip.GetComponent<Button>().onClick.AddListener(SkipCutscene);

        MostrarDialogo(0);
    }

    void MostrarDialogo(int indice)
    {
        if (indice >= dialogos.Length) { IrAlNivel(); return; }
        StopAllCoroutines();
        StartCoroutine(EscribirTexto(dialogos[indice].texto, dialogos[indice].vozAudio));
    }

    IEnumerator EscribirTexto(string texto, AudioClip voz)
    {
        escribiendo = true;
        botonSiguiente.SetActive(false);
        textoDialogo.text = "";
        if (audioSource != null && voz != null) { audioSource.clip = voz; audioSource.Play(); }
        corrutinaBoca = StartCoroutine(AnimarBoca());
        foreach (char letra in texto)
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadTexto);
        }
        DetenerBoca();
        escribiendo = false;
        botonSiguiente.SetActive(true);
        textoBotonSiguiente.text = indiceActual < dialogos.Length - 1 ? "Siguiente →" : textoSiguienteUltimo;
    }

    IEnumerator AnimarBoca()
    {
        while (true)
        {
            if (imagenPlaneta != null && spriteBocaAbierta != null)
                imagenPlaneta.sprite = spriteBocaAbierta;
            yield return new WaitForSeconds(velocidadBoca);
            if (imagenPlaneta != null && spriteBocaCerrada != null)
                imagenPlaneta.sprite = spriteBocaCerrada;
            yield return new WaitForSeconds(velocidadBoca);
        }
    }

    void DetenerBoca()
    {
        if (corrutinaBoca != null) { StopCoroutine(corrutinaBoca); corrutinaBoca = null; }
        if (imagenPlaneta != null && spriteBocaCerrada != null)
            imagenPlaneta.sprite = spriteBocaCerrada;
    }

    public void Siguiente()
    {
        if (escribiendo)
        {
            StopAllCoroutines();
            textoDialogo.text = dialogos[indiceActual].texto;
            if (audioSource != null) audioSource.Stop();
            DetenerBoca();
            escribiendo = false;
            botonSiguiente.SetActive(true);
            textoBotonSiguiente.text = indiceActual < dialogos.Length - 1 ? "Siguiente →" : textoSiguienteUltimo;
            return;
        }
        indiceActual++;
        MostrarDialogo(indiceActual);
    }

    public void SkipCutscene()
    {
        StopAllCoroutines();
        if (audioSource != null) audioSource.Stop();
        DetenerBoca();
        IrAlNivel();
    }

    void IrAlNivel()
    {
        if (SceneTransitionManager.instancia != null)
            SceneTransitionManager.instancia.CargarEscena(escenaDestino);
    }
}



