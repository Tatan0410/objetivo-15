using UnityEngine;

public class ItemMunicion : MonoBehaviour
{
    public int cantidad = 5;

    void Start()
    {
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
            col.radius = 0.3f;
            col.isTrigger = true;
        }

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = gameObject.AddComponent<SpriteRenderer>();

        if (sr.sprite == null)
            sr.sprite = GenerarSprite();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        if (MunicionManager.instancia != null)
            MunicionManager.instancia.AgregarMunicion(cantidad);

        Destroy(gameObject);
    }

    Sprite GenerarSprite()
    {
        int size = 24;
        int ppu = 100;

        Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Bilinear;

        Color c = new Color(1f, 0.6f, 0.1f);

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                tex.SetPixel(x, y, c);

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), ppu);
    }
}
