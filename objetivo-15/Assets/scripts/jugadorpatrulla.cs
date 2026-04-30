using UnityEngine;

public class jugadorpatrulla : MonoBehaviour
{
    [Header("Patrulla")]
    public float velocidad = 2f;
    public float distanciaPatrulla = 3f;

    private Vector2 puntoInicio;
    private bool moviendoDerecha = true;

    void Start()
    {
        puntoInicio = transform.position;
    }

    void Update()
    {
        Patrullar();
    }

    void Patrullar()
    {
        if (moviendoDerecha)
        {
            transform.Translate(Vector2.right * velocidad * Time.deltaTime);
            if (transform.position.x >= puntoInicio.x + distanciaPatrulla)
                moviendoDerecha = false;
        }
        else
        {
            transform.Translate(Vector2.left * velocidad * Time.deltaTime);
            if (transform.position.x <= puntoInicio.x - distanciaPatrulla)
                moviendoDerecha = true;
        }
    }

    // ← OnTriggerEnter2D porque el jugador tiene Is Trigger activado
    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Trigger con: " + col.gameObject.name + " | Tag: " + col.gameObject.tag);

        if (col.CompareTag("Player"))
        {
            if (VidasManager.instancia != null)
                VidasManager.instancia.PerderVida();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 pos = Application.isPlaying ? puntoInicio : (Vector2)transform.position;
        Gizmos.DrawLine(
            new Vector2(pos.x - distanciaPatrulla, pos.y),
            new Vector2(pos.x + distanciaPatrulla, pos.y)
        );
    }
}