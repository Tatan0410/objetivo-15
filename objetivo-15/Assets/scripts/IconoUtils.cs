using UnityEngine;

public static class IconoUtils
{
    public static Sprite GenerarCirculo(int size, int ppu, Color color)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Bilinear;

        float centro = size / 2f;
        float radio = centro - 1;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - centro + 0.5f;
                float dy = y - centro + 0.5f;
                bool dentro = (dx * dx + dy * dy) <= radio * radio;
                tex.SetPixel(x, y, dentro ? color : Color.clear);
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), ppu);
    }

    // TODO: reemplazar icono placeholder con sprite real de corazon
    public static Sprite GenerarCorazon(int size, int ppu, Color color)
    {
        Texture2D tex = GenerarCorazonTexture(size, color);
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), ppu);
    }

    public static Texture2D GenerarCorazonTexture(int size, Color color)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Bilinear;
        Color clear = Color.clear;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool px = EsPixelCorazon(x, y, size);
                tex.SetPixel(x, y, px ? color : clear);
            }
        }

        tex.Apply();
        return tex;
    }

    static bool EsPixelCorazon(int x, int y, int size)
    {
        float fx = (float)x / size;
        float fy = (float)y / size;
        float cx = 0.5f;
        float cy = 0.6f;

        float dx = (fx - cx) * 2f;
        float dy = (fy - cy) * 2f;

        // Heart curve approximation
        float h = dx * dx + dy * dy - 0.3f;
        return h * h * h - dx * dx * dy * dy * dy <= 0f;
    }
}
