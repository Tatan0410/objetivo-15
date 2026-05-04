using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activado = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !activado)
        {
            activado = true;
            // Guardar posición del jugador, no del checkpoint
            GameManager.instancia.GuardarCheckpoint(other.transform.position);
            GetComponent<SpriteRenderer>().color = Color.green;
        }
    }
}