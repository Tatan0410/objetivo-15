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
            if (GameManager.instancia != null)
                GameManager.instancia.RespawnJugador(gameObject);
            muriendo = false;
        }
    }
}