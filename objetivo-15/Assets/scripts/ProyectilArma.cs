using UnityEngine;

public enum TipoProyectil
{
    Lanzador
}

public class ProyectilArma : MonoBehaviour
{
    public TipoProyectil tipoArma = TipoProyectil.Lanzador;
    public float velocidad = 12f;
    public int danio = 1;
    public float tiempoVida = 3f;
    public float radioDeteccion = 0.3f;

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
            rb.gravityScale = 0f;
        }

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = gameObject.AddComponent<SpriteRenderer>();

        if (sr.sprite == null)
            sr.sprite = GenerarSprite();

        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
            col.radius = 0.15f;
            col.isTrigger = true;
        }
    }

    public void Iniciar(Vector2 dir, TipoProyectil tipo = TipoProyectil.Lanzador)
    {
        tipoArma = tipo;
        direccion = dir.normalized;
        Destroy(gameObject, tiempoVida);
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
            if (col.CompareTag("Player") || col.CompareTag("Plastico")) continue;

            Enemigo enemigo = col.GetComponentInParent<Enemigo>();
            if (enemigo != null)
            {
                enemigo.RecibirDanio(danio);
                Destruir();
                return;
            }

            EnemigoVolador volador = col.GetComponentInParent<EnemigoVolador>();
            if (volador != null)
            {
                volador.RecibirDanio(danio);
                Destruir();
                return;
            }

            rata enemigoRata = col.GetComponentInParent<rata>();
            if (enemigoRata != null)
            {
                enemigoRata.RecibirDanio(danio);
                Destruir();
                return;
            }
        }
    }

    void Destruir()
    {
        if (destruido) return;
        destruido = true;
        Debug.Log($"[ProyectilArma] Impacto en enemigo en ({transform.position.x:F2}, {transform.position.y:F2})");
        Destroy(gameObject);
    }

    Sprite GenerarSprite()
    {
        int size = 32;
        int ppu = 100;

        Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                tex.SetPixel(x, y, new Color(1f, 0.9f, 0.2f));

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), ppu);
    }
}