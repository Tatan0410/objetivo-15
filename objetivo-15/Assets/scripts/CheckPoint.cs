using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activado = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !activado)
        {
            activado = true;

            // ✅ Guardamos la posición del checkpoint
            // El offset Y se aplica dentro de GameManager.GuardarCheckpoint()
            GameManager.instancia.GuardarCheckpoint(transform.position);

            GetComponent<SpriteRenderer>().color = Color.green;
            Debug.Log("Checkpoint activado");
        }
    }
}