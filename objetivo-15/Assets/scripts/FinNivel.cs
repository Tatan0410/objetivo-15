using UnityEngine;
using UnityEngine.SceneManagement;

public class FinNivel : MonoBehaviour
{
    public int numeroNivel;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerPrefs.GetInt("NivelDesbloqueado") < numeroNivel)
                PlayerPrefs.SetInt("NivelDesbloqueado", numeroNivel);

            // Guardar nodo actual igual al nivel completado
            PlayerPrefs.SetInt("NodoActual", numeroNivel - 1);

            // Indicar que NO debe moverse automáticamente
            PlayerPrefs.SetInt("MoverAutomatico", 0);
            PlayerPrefs.Save();

            SceneManager.LoadScene("Mapamundial");
        }
    }
}