using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// Splash custom: se muestra justo despues del splash "Made with Unity"
// (obligatorio en Unity Personal) como escena 0 del Build Settings.
// Muestra la portada unos segundos y luego carga el menu principal.
// NOTA: se carga con SceneManager.LoadScene directo y NO por el
// SceneTransitionManager, porque el splash ya actua como transicion
// y no queremos la pantalla de carga encadenada al abrir el juego.
public class SplashScreen : MonoBehaviour
{
    [SerializeField] private float duracion = 3f;
    [SerializeField] private string escenaMenu = "menuprincipal";

    private void Start()
    {
        Time.timeScale = 1f;
        StartCoroutine(EsperarYCargar());
    }

    private IEnumerator EsperarYCargar()
    {
        yield return new WaitForSeconds(duracion);
        SceneManager.LoadScene(escenaMenu);
    }
}
