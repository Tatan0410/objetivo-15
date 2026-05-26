using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activado = false;

    [Header("Offset de spawn")]
    // ✅ El jugador aparece un poco arriba del checkpoint para no
    // quedar dentro del suelo ni de la geometría
    public float offsetY = 1f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !activado)
        {
            activado = true;

            // ✅ Guardar posición con offset hacia arriba
            Vector3 posSpawn = transform.position + Vector3.up * offsetY;
            GameManager.instancia.GuardarCheckpoint(posSpawn);

            GetComponent<SpriteRenderer>().color = Color.green;
            Debug.Log("Checkpoint activado en: " + posSpawn);
        }
    }
}