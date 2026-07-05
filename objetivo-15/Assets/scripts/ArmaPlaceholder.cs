using UnityEngine;

public enum TipoArma
{
    Lanzador,
    Red,
    Escudo,
    LanzaTubos
}

public class ArmaPlaceholder : MonoBehaviour
{
    public TipoArma tipo;
    public Color color = Color.blue;
    public float ancho = 0.5f;
    public float alto = 0.3f;

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = gameObject.AddComponent<SpriteRenderer>();

        if (sr.sprite == null)
            sr.sprite = GenerarSprite();
    }

    Sprite GenerarSprite()
    {
        int w = 64;
        int h = 40;
        int ppu = 100;

        Texture2D tex = new Texture2D(w, h, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                tex.SetPixel(x, y, color);

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), ppu);
    }
}
