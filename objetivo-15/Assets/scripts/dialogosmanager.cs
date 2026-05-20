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
    public Sprite imagen;
}

public class DialogoManager : MonoBehaviour
{
    [Header("UI")]
    public Image imagenFondo;
    public TMP_Text textoDialogo;
    public TMP_Text textoBoton;
    public GameObject boton;

    [Header("Configuración")]
    public Dialogo[] dialogos;
    public string escenaDestino;
    public float velocidadTexto = 0.03f;

    private int indiceActual = 0;
    private bool escribiendo = false;

    void Start()
    {
        boton.SetActive(false);
        MostrarDialogo(0);
    }

    void MostrarDialogo(int indice)
    {
        if (indice >= dialogos.Length)
        {
            SceneManager.LoadScene(escenaDestino);
            return;
        }

        if (dialogos[indice].imagen != null)
            imagenFondo.sprite = dialogos[indice].imagen;

        StopAllCoroutines();
        StartCoroutine(EscribirTexto(dialogos[indice].texto));
    }

    IEnumerator EscribirTexto(string texto)
    {
        escribiendo = true;
        boton.SetActive(false);
        textoDialogo.text = "";

        foreach (char letra in texto)
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadTexto);
        }

        escribiendo = false;
        boton.SetActive(true);
        textoBoton.text = indiceActual < dialogos.Length - 1 ?
            "Siguiente →" : "¡Jugar!";
    }

    public void Siguiente()
    {
        if (escribiendo)
        {
            StopAllCoroutines();
            textoDialogo.text = dialogos[indiceActual].texto;
            escribiendo = false;
            boton.SetActive(true);
            textoBoton.text = indiceActual < dialogos.Length - 1 ?
                "Siguiente →" : "¡Jugar!";
            return;
        }

        indiceActual++;
        MostrarDialogo(indiceActual);
    }
}