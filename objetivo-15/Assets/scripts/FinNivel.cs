using UnityEngine;
using UnityEngine.SceneManagement;

public class FinNivel : MonoBehaviour
{
    public int numeroNivel;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Desbloquea el siguiente nivel
            if (PlayerPrefs.GetInt("NivelDesbloqueado") < numeroNivel)
                PlayerPrefs.SetInt("NivelDesbloqueado", numeroNivel);

            SceneManager.LoadScene("Mapamundial");
        }
    }
}