using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activado = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !activado)
        {
            activado = true;
            GameManager.instancia.GuardarCheckpoint(transform.position);
            GetComponent<SpriteRenderer>().color = Color.green;
        }
    }
}