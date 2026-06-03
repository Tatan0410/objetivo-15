using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FinNivel : MonoBehaviour
{
    [Header("Configuración")]
    // Número del nivel actual (1 para nivel1_colegio, 2 para nivel2_hipodromo, etc.)
    public int numeroNivel;

    [Header("Animación")]
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

        // Jugador camina hacia la bandera
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

        // El jugador iza la bandera (sube en vez de caer)
        if (bandera != null)
        {
            Vector3 posInicial = bandera.position;
            Vector3 posFinal = posInicial + Vector3.up * 2f;
            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * velocidadBandera;
                bandera.position = Vector3.Lerp(posInicial, posFinal, t);
                yield return null;
            }
        }

        yield return new WaitForSeconds(1f);

        // Resetear checkpoint al completar el nivel
        if (GameManager.instancia != null)
            GameManager.instancia.ResetearCheckpoint();

        // Desbloquear el siguiente nodo solo si no estaba ya desbloqueado
        int nivelDesbloqueadoActual = PlayerPrefs.GetInt("NivelDesbloqueado", 0);
        if (nivelDesbloqueadoActual < numeroNivel)
            PlayerPrefs.SetInt("NivelDesbloqueado", numeroNivel);

        // ✅ CLAVE: el jugador del mapa queda parado en el nodo del nivel recién desbloqueado
        // Ejemplo: completar nivel 1 (numeroNivel=1) → NodoActual=1 → jugador aparece en nodo 1
        PlayerPrefs.SetInt("NodoActual", numeroNivel);

        PlayerPrefs.Save();
        SceneManager.LoadScene("Mapamundial");
    }
}