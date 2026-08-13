using UnityEngine;
using UnityEngine.SceneManagement;

public class FinNivel : MonoBehaviour
{
    [Header("Configuración")]
    public int numeroNivel;

    [Header("Partículas")]
    public ParticleSystem particulas;

    private bool nivelTerminado = false;

    void Start()
    {
        if (particulas != null)
            particulas.Play();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !nivelTerminado)
        {
            nivelTerminado = true;
            TerminarNivel();
        }
    }

    void TerminarNivel()
    {
        if (GameManager.instancia != null)
            GameManager.instancia.ResetearCheckpoint();

        int nivelDesbloqueadoActual = PlayerPrefs.GetInt("NivelDesbloqueado", 0);
        if (nivelDesbloqueadoActual < numeroNivel)
            PlayerPrefs.SetInt("NivelDesbloqueado", numeroNivel);

        PlayerPrefs.SetInt("NodoActual", numeroNivel);
        PlayerPrefs.Save();
        SceneTransitionManager.CargarEscenaConFallback("Mapamundial");
    }
}