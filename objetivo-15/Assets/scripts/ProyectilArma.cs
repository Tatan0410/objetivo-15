using UnityEngine;

public enum TipoProyectil
{
    Lanzador,
    Red,
    LanzaTubos
}

public class ProyectilArma : MonoBehaviour
{
    public TipoProyectil tipoArma = TipoProyectil.Lanzador;
    public float velocidad = 12f;
    public int danio = 1;
    public float tiempoVida = 3f;
    public float radioExplosion = 2f;
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

        if (enemigo != null || volador != null)
        {
            switch (tipoArma)
            {
                case TipoProyectil.Red:
                    if (enemigo != null)
                        enemigo.Congelar();
                    else
                        volador.Congelar();
                    break;

                case TipoProyectil.LanzaTubos:
                    Collider2D[] hits = Physics2D.OverlapCircleAll(
                        transform.position, radioExplosion);
                    foreach (Collider2D hit in hits)
                    {
                        Enemigo e = hit.GetComponent<Enemigo>();
                        if (e != null) e.RecibirDanio(danio);
                        EnemigoVolador v = hit.GetComponent<EnemigoVolador>();
                        if (v != null) v.RecibirDanio(danio);
                    }
                    break;

                default:
                    if (enemigo != null)
                        enemigo.RecibirDanio(danio);
                    else
                        volador.RecibirDanio(danio);
                    break;
            }

            Destroy(gameObject);
            return;
        }

        if (!col.CompareTag("Player") && !col.CompareTag("Plastico"))
            Destroy(gameObject);
    }

    Sprite GenerarSprite()
    {
        int size = 16;
        int ppu = 100;

        Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Bilinear;

        Color c;
        switch (tipoArma)
        {
            case TipoProyectil.Red:
                c = new Color(0.2f, 0.6f, 1f);
                break;
            case TipoProyectil.LanzaTubos:
                c = new Color(0.2f, 1f, 0.3f);
                break;
            default:
                c = new Color(1f, 0.9f, 0.2f);
                break;
        }

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                tex.SetPixel(x, y, c);

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), ppu);
    }
}
