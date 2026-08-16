using UnityEngine;

public class DañoEnemigo : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MuerteJugador muerte = collision.GetComponent<MuerteJugador>();
            if (muerte != null)
                muerte.MorirPorEnemigo();
        }
    }
}