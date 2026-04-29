using UnityEngine;

public class MuerteJugador : MonoBehaviour
{
    public float limiteY = -10f;

    void Update()
    {
        if (transform.position.y < limiteY)
        {
            GameManager.instancia.MorirJugador();
        }
    }
}