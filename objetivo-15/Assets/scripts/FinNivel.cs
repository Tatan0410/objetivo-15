using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FinNivel : MonoBehaviour
{
    [Header("Configuración")]
    public int numeroNivel;

    [Header("Animación")]
    public Transform bandera;
    public Transform puntoAsta;
    public float velocidadSubida = 5f;
    public float alturaTopeAsta = 0f;

    private bool nivelTerminado = false;

    void Start()
    {
        if (alturaTopeAsta == 0f && bandera != null)
            alturaTopeAsta = bandera.position.y + 3f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !nivelTerminado)
        {
            nivelTerminado = true;
            StartCoroutine(SubirAsta(other.gameObject));
        }
    }

    IEnumerator SubirAsta(GameObject jugador)
    {
        Rigidbody2D rb = jugador.GetComponent<Rigidbody2D>();
        PlayerController pc = jugador.GetComponent<PlayerController>();

        if (pc != null) pc.DesactivarControl();

        float gravOriginal = rb != null ? rb.gravityScale : 1f;
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.isKinematic = true;
        }

        float alturaInicial = jugador.transform.position.y;
        float alturaFinal = alturaTopeAsta;
        if (alturaFinal <= alturaInicial)
            alturaFinal = alturaInicial + 0.5f;

        float duracion = Mathf.Max((alturaFinal - alturaInicial) / velocidadSubida, 0.3f);
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            float t = tiempo / duracion;

            Vector3 pos = jugador.transform.position;
            pos.x = puntoAsta != null ? puntoAsta.position.x : transform.position.x;
            pos.y = Mathf.Lerp(alturaInicial, alturaFinal, t);
            jugador.transform.position = pos;
            if (rb != null) rb.position = pos;

            tiempo += Time.deltaTime;
            yield return null;
        }

        Vector3 posFinal = jugador.transform.position;
        posFinal.x = puntoAsta != null ? puntoAsta.position.x : transform.position.x;
        posFinal.y = alturaFinal;
        jugador.transform.position = posFinal;
        if (rb != null) rb.position = posFinal;

        yield return new WaitForSeconds(0.5f);

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.gravityScale = gravOriginal;
        }

        if (GameManager.instancia != null)
            GameManager.instancia.ResetearCheckpoint();

        int nivelDesbloqueadoActual = PlayerPrefs.GetInt("NivelDesbloqueado", 0);
        if (nivelDesbloqueadoActual < numeroNivel)
            PlayerPrefs.SetInt("NivelDesbloqueado", numeroNivel);

        PlayerPrefs.SetInt("NodoActual", numeroNivel);
        PlayerPrefs.Save();
        if (SceneTransitionManager.instancia != null)
            SceneTransitionManager.instancia.CargarEscena("Mapamundial");
    }
}