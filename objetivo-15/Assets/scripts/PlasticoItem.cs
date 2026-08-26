using UnityEngine;

public class PlasticoItem : MonoBehaviour
{
    [Header("Tipo de plástico")]
    public TipoPlastico tipo;
    public float tiempoVida = 9999f;
    public float limiteY = -10f;

    [Header("Animación flotante")]
    public float velocidadFlotacion = 1.5f;
    public float alturaFlotacion = 0.2f;

    private bool recolectado = false;
    private Vector3 posicionInicial;
    private Rigidbody2D rb;
    private Collider2D[] colliders;

    void Start()
    {
        if (alturaFlotacion == 0f) alturaFlotacion = 0.2f;
        if (limiteY == -10f) limiteY = -50f;

        Debug.Log($"[PlasticoItem] Spawn '{tipo}' en {transform.position} limiteY={limiteY} escena={UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");

        posicionInicial = transform.position;
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<Collider2D>();

        // Apagar física para que no caiga
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;

        // Desactivar TODOS los colliders al spawn para que no se auto-recolecte
        // cuando aparece encima del jugador al matar un enemigo
        foreach (Collider2D c in colliders)
            c.enabled = false;
        Invoke("ActivarCollider", 0.3f);

        Destroy(gameObject, tiempoVida);
    }

    void ActivarCollider()
    {
        if (recolectado) return;
        foreach (Collider2D c in colliders)
            c.enabled = true;
    }

    void Update()
    {
        // Animación de flotación estilo monedas de Mario
        float nuevoY = posicionInicial.y +
            Mathf.Sin(Time.time * velocidadFlotacion) * alturaFlotacion;
        transform.position = new Vector3(
            posicionInicial.x, nuevoY, posicionInicial.z);

        // Destruir si cae fuera del mapa
        if (transform.position.y < limiteY)
        {
            Debug.Log($"[PlasticoItem] Destruido por limiteY {transform.position.y} < {limiteY} tipo={tipo}");
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (recolectado) return;
        if (col.CompareTag("Player"))
        {
            recolectado = true;

            // ✅ FIX: deshabilitar los colliders de inmediato para que no dispare
            // dos veces en el mismo frame por colliders múltiples del jugador
            foreach (Collider2D c in colliders)
                c.enabled = false;

            if (Inventario.instancia != null)
                Inventario.instancia.AgregarPlastico(tipo);
            else
                Debug.LogWarning("⚠️ Inventario.instancia es NULL");

            if (ColeccionablesManager.instancia != null)
                ColeccionablesManager.instancia.AgregarPlastico();

            Destroy(gameObject);
        }
    }
}