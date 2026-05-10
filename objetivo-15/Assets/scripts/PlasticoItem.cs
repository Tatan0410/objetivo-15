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

    void Start()
    {
        posicionInicial = transform.position;
        rb = GetComponent<Rigidbody2D>();

        // ✅ FIX PRINCIPAL: apagar la física para que no caiga
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;

        Destroy(gameObject, tiempoVida);
    }

    void Update()
    {
        // Animación de flotación (como monedas de Mario)
        float nuevoY = posicionInicial.y +
            Mathf.Sin(Time.time * velocidadFlotacion) * alturaFlotacion;
        transform.position = new Vector3(
            posicionInicial.x, nuevoY, posicionInicial.z);

        // Seguridad por si cae de todas formas
        if (transform.position.y < limiteY)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (recolectado) return;

        if (col.CompareTag("Player"))
        {
            recolectado = true;

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