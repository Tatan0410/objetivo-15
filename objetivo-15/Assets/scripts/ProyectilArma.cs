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
    private Vector2 direccion;
    private Rigidbody2D rb;

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
        if (rb != null)
            rb.MovePosition(rb.position + direccion * velocidad * Time.fixedDeltaTime);
        else
            transform.Translate(direccion * velocidad * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Enemigo enemigo = col.GetComponent<Enemigo>();
        EnemigoVolador volador = col.GetComponent<EnemigoVolador>();

        if (enemigo != null)
            enemigo.RecibirDanio(danio);
        else if (volador != null)
            volador.RecibirDanio(danio);
        else if (!col.CompareTag("Player") && !col.CompareTag("Plastico"))
        {
            Destroy(gameObject);
            return;
        }

        if (enemigo != null || volador != null)
            Destroy(gameObject);
    }

    Sprite GenerarSprite()
    {
        int size = 16;
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
