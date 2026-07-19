using UnityEngine;

public class AplicarVolumenMusica : MonoBehaviour
{
    void Start()
    {
        float volumen = PlayerPrefs.GetFloat("VolumenMusica", 1f);
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
            audioSource.volume = volumen;
    }
}
