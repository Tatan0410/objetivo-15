using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activado = false;

    [Header("Punto exacto de respawn")]
    public Transform spawnPoint;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !activado)
        {
            activado = true;

            if (spawnPoint != null)
            {
                GameManager.instancia.GuardarCheckpoint(spawnPoint.position);

                Debug.Log("Checkpoint guardado en: " + spawnPoint.position);
            }
            else
            {
                Debug.LogWarning("No hay SpawnPoint asignado");
            }

            GetComponent<SpriteRenderer>().color = Color.green;
        }
    }
}