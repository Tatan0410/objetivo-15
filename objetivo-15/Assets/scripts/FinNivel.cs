using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FinNivel : MonoBehaviour
{
    public int numeroNivel;
    public Transform bandera;
    public float velocidadBandera = 2f;

    private bool nivelTerminado = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !nivelTerminado)
        {
            nivelTerminado = true;
            StartCoroutine(AnimacionFinNivel(other.gameObject));
        }
    }

    IEnumerator AnimacionFinNivel(GameObject jugador)
    {
        Rigidbody2D rb = jugador.GetComponent<Rigidbody2D>();
        PlayerController pc = jugador.GetComponent<PlayerController>();

        if (pc != null) pc.DesactivarControl();
        if (rb != null) rb.velocity = Vector2.zero;

        // Jugador camina hacia la bandera por tiempo fijo
        if (bandera != null && rb != null)
        {
            float direccion = bandera.position.x >
                jugador.transform.position.x ? 1f : -1f;

            float tiempoCaminando = 0f;
            while (tiempoCaminando < 0.5f)
            {
                rb.velocity = new Vector2(direccion * 3f, rb.velocity.y);
                tiempoCaminando += Time.deltaTime;
                yield return null;
            }

            rb.velocity = Vector2.zero;
        }

        yield return new WaitForSeconds(0.3f);

        // Animar la bandera bajando
        if (bandera != null)
        {
            Vector3 posInicial = bandera.position;
            Vector3 posFinal = posInicial + Vector3.down * 2f;
            float t = 0;

            while (t < 1f)
            {
                t += Time.deltaTime * velocidadBandera;
                bandera.position = Vector3.Lerp(posInicial, posFinal, t);
                yield return null;
            }
        }

        yield return new WaitForSeconds(1f);

        // Resetear checkpoint antes de ir al mapa
        if (GameManager.instancia != null)
            GameManager.instancia.ResetearCheckpoint();

        if (PlayerPrefs.GetInt("NivelDesbloqueado") < numeroNivel)
            PlayerPrefs.SetInt("NivelDesbloqueado", numeroNivel);

        PlayerPrefs.SetInt("NodoActual", numeroNivel - 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Mapamundial");
    }
}