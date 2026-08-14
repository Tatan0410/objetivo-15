using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activado = false;
    private SpriteRenderer sr;

    [Header("Offset adicional sobre el checkpoint")]
    public float offsetRespawn = 1f;

    [Header("Sprites de estado")]
    public Sprite spriteInactivo;
    public Sprite spriteActivo;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null && spriteInactivo != null)
        {
            sr.sprite = spriteInactivo;
            sr.color = Color.white;
        }
    }

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

            if (sr == null) sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                if (spriteActivo != null)
                    sr.sprite = spriteActivo;
                sr.color = Color.white;
            }
            Debug.Log("Checkpoint activado en: " + posRespawn);
        }
    }
}