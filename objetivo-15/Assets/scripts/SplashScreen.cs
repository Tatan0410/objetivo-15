using UnityEngine;
using System.Collections;

// Splash custom: se muestra justo despues del splash "Made with Unity"
// (obligatorio en Unity Personal) como escena 0 del Build Settings.
// Muestra la portada unos segundos y luego carga el menu principal.
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
        SceneTransitionManager.CargarEscenaConFallback(escenaMenu);
    }
}
