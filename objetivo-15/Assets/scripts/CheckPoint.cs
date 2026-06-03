using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activado = false;

    [Header("Offset adicional sobre el checkpoint")]
    public float offsetRespawn = 1f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !activado)
        {
            activado = true;

            Vector3 posRespawn = transform.position + Vector3.up * offsetRespawn;

            // Guardar respawn localmente en el jugador
            MuerteJugador muerte = other.GetComponent<MuerteJugador>();
            if (muerte != null)
                muerte.GuardarRespawn(posRespawn);

            if (GameManager.instancia != null)
                GameManager.instancia.GuardarCheckpoint(posRespawn);

            GetComponent<SpriteRenderer>().color = Color.green;
            Debug.Log("Checkpoint activado en: " + posRespawn);
        }
    }
}