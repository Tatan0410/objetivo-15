using UnityEngine;
using System.Collections;

public class MuerteJugador : MonoBehaviour
{
    [Header("Límite de caída")]
    public float limiteY = -20f;

    private bool muriendo = false;

    void Update()
    {
        // Evitar múltiples muertes
        if (muriendo) return;

        // Detectar caída al vacío
        if (transform.position.y < limiteY)
        {
            StartCoroutine(Morir());
        }
    }

    IEnumerator Morir()
    {
        muriendo = true;

        Debug.Log("Jugador cayó al vacío");

        // Perder vida
        if (VidasManager.instancia != null)
        {
            VidasManager.instancia.PerderVida();
        }

        // Esperar un poquito para estabilizar
        yield return new WaitForSeconds(0.2f);

        // Permitir futuras muertes
        muriendo = false;
    }
}