using UnityEngine;

public class ProyectilBoss : MonoBehaviour
{
    public float velocidad = 6f;
    public float tiempoVida = 4f;
    public float radioDeteccion = 0.3f;

    [Tooltip("Offset de rotación del sprite. Usa 180 si el sprite mira a la izquierda por defecto.")]
    public float giroSprite = 180f;
    public bool ignoraInmortalidad = false;

    private Vector2 direccion;
    private Rigidbody2D rb;
    private bool destruido = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
        rb.gravityScale = 0f;
    }

    public void Iniciar(Vector2 dir)
    {
        direccion = dir.normalized;
        Destroy(gameObject, tiempoVida);

        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg + giroSprite;
        transform.rotation = Quaternion.Euler(0f, 0f, angulo);
    }

    void FixedUpdate()
    {
        if (destruido) return;

        if (rb != null)
            rb.MovePosition(rb.position + direccion * velocidad * Time.fixedDeltaTime);
        else
            transform.Translate(direccion * velocidad * Time.deltaTime);

        DetectarImpacto();
    }

    void DetectarImpacto()
    {
        Collider2D[] colisiones = Physics2D.OverlapCircleAll(transform.position, radioDeteccion);

        foreach (Collider2D col in colisiones)
        {
            if (col.gameObject == gameObject) continue;
            if (col.isTrigger) continue;

            if (col.CompareTag("Player"))
            {
                MuerteJugador muerte = col.GetComponentInParent<MuerteJugador>();
                if (muerte != null)
                    muerte.MorirPorEnemigo(ignoraInmortalidad);
                Destruir();
                return;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (destruido) return;

        Enemigo e = other.GetComponentInParent<Enemigo>();
        EnemigoVolador ev = other.GetComponentInParent<EnemigoVolador>();
        EnemigoDiablo ed = other.GetComponentInParent<EnemigoDiablo>();
        BossFinal boss = other.GetComponentInParent<BossFinal>();
        rata r = other.GetComponentInParent<rata>();
        if (e != null || ev != null || ed != null || boss != null || r != null)
            return;

        if (other.CompareTag("Player"))
        {
            MuerteJugador muerte = other.GetComponentInParent<MuerteJugador>();
            if (muerte != null)
                muerte.MorirPorEnemigo(ignoraInmortalidad);
            Destruir();
        }
    }

    void Destruir()
    {
        if (destruido) return;
        destruido = true;
        Destroy(gameObject);
    }
}