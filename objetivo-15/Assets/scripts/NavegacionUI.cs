using UnityEngine;
using UnityEngine.SceneManagement;

public class NavegacionUI : MonoBehaviour
{
    public AudioSource musicaFondo;

    public void VolverAlMenu()
    {
        if (musicaFondo != null)
            musicaFondo.Stop();
        SceneManager.LoadScene("menuprincipal");
    }
}
