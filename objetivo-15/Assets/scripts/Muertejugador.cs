using UnityEngine;

public class MuerteJugador : MonoBehaviour
{
    public float limiteY = -10f;
    private bool muriendo = false;

    void Update()
    {
        if (muriendo) return;

        if (transform.position.y < limiteY)
        {
            muriendo = true;
            if (VidasManager.instancia != null)
                VidasManager.instancia.PerderVida();

            // Esperar a que el respawn resetee la posición
            Invoke("ResetearMuriendo", 0.5f);
        }
    }

    void ResetearMuriendo()
    {
        muriendo = false;
    }
}