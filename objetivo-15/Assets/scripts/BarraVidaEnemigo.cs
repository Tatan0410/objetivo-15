using UnityEngine;

public class BarraVidaEnemigo : MonoBehaviour
{
    private SpriteRenderer fondo;
    private SpriteRenderer relleno;
    private float anchoBarra;

    public void Inicializar(float ancho, float alto)
    {
        anchoBarra = ancho;

        fondo = CrearRenderer("Fondo", new Color(0f, 0f, 0f, 0.65f), ancho, alto, 0.5f);
        fondo.transform.localPosition = Vector3.zero;
        fondo.transform.localScale = new Vector3(ancho, alto, 1f);

        relleno = CrearRenderer("Relleno", Color.green, 0f, 0f, 0f);
        relleno.transform.localPosition = new Vector3(-ancho / 2f, 0f, 0f);
        relleno.transform.localScale = new Vector3(ancho, alto * 0.75f, 1f);
    }

    public void Seguir(Vector3 pos)
    {
        transform.position = pos;
    }

    public void Actualizar(float porcentaje)
    {
        if (relleno == null) return;
        porcentaje = Mathf.Clamp01(porcentaje);

        Vector3 escala = relleno.transform.localScale;
        escala.x = anchoBarra * porcentaje;
        relleno.transform.localScale = escala;

        relleno.color = Color.Lerp(Color.red, Color.green, porcentaje);
    }

    public void Ocultar()
    {
        Destroy(gameObject);
    }

    private SpriteRenderer CrearRenderer(string nombre, Color color, float ancho, float alto, float pivotX)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(transform, false);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = GenerarSprite(pivotX);
        sr.color = color;
        sr.sortingOrder = 32767;
        return sr;
    }

    private Sprite GenerarSprite(float pivotX)
    {
        Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(pivotX, 0.5f), 100f);
    }
}