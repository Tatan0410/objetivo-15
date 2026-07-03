using UnityEngine;

public enum TipoPotenciador
{
    Velocidad,
    Inmortalidad,
    VidaExtra
}

public class Potenciador : MonoBehaviour
{
    [Header("Configuracion")]
    public TipoPotenciador tipo;
    public float tiempoVida = 10f;

    [Header("Animacion flotante")]
    public float velocidadFlotacion = 2f;
    public float alturaFlotacion = 0.15f;

    private Vector3 posInicial;

    void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            var cc = gameObject.AddComponent<CircleCollider2D>();
            cc.radius = 0.32f;
            cc.isTrigger = true;
        }
        else
        {
            col.isTrigger = true;
        }

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null)
        {
            if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();
            sr.sprite = GenerarSpritePlaceholder();
            sr.sortingLayerName = "Default";
            sr.sortingOrder = 0;
        }

        posInicial = transform.position;
        Destroy(gameObject, tiempoVida);
    }

    void Update()
    {
        float nuevoY = posInicial.y +
            Mathf.Sin(Time.time * velocidadFlotacion) * alturaFlotacion;
        transform.position = new Vector3(posInicial.x, nuevoY, posInicial.z);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        PlayerController pc = col.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.AplicarPotenciador(tipo);
            Debug.Log("Potenciador aplicado: " + tipo);
        }
        else
        {
            Debug.LogWarning("No se encontro PlayerController en el jugador");
        }

        Destroy(gameObject);
    }

    // TODO: Reemplazar con sprites reales cuando esten disponibles
    Sprite GenerarSpritePlaceholder()
    {
        int size = 64;
        int ppu = 100;

        Color color = tipo switch
        {
            TipoPotenciador.Velocidad => new Color(1f, 0.84f, 0f),      // dorado
            TipoPotenciador.Inmortalidad => new Color(0f, 0.75f, 1f),   // cian
            TipoPotenciador.VidaExtra => new Color(0f, 0.8f, 0.2f),     // verde
            _ => Color.white
        };

        Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Bilinear;

        float center = size / 2f;
        float radius = size / 2f - 2;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center;
                float dy = y - center;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                if (dist <= radius)
                {
                    float alpha = 1f;
                    if (dist > radius - 2f)
                        alpha = radius - dist + 1f;
                    Color c = color;
                    c.a = Mathf.Clamp01(alpha);
                    tex.SetPixel(x, y, c);
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }

        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), ppu);
    }
}
