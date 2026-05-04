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
            // ✅ El respawn ahora lo maneja VidasManager.PerderVida()
            // ya no necesitas llamar GameManager aquí para evitar doble respawn

            muriendo = false;
        }
    }
}