using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activado = false;

    [Header("Punto exacto de respawn (opcional)")]
    // Si asignas un SpawnPoint, el jugador aparece ahí
    // Si no, aparece encima del propio checkpoint
    public Transform spawnPoint;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !activado)
        {
            activado = true;

            Vector3 posRespawn;

            if (spawnPoint != null)
            {
                // Usar el SpawnPoint asignado en el Inspector
                posRespawn = spawnPoint.position;
            }
            else
            {
                // ✅ Fallback: usar posición del checkpoint + offset hacia arriba
                // para no quedar dentro del suelo
                posRespawn = transform.position + Vector3.up * 1f;
                Debug.LogWarning("Checkpoint sin SpawnPoint — usando posición automática: " + posRespawn);
            }

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