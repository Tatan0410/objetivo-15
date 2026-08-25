using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Text;

[System.Serializable]
public class PersonaCreditos
{
    [Tooltip("Nombre de la persona")]
    public string nombre;

    [Tooltip("Rol de la persona (se resalta)")]
    public string rol;
}

[System.Serializable]
public class SeccionCreditos
{
    [Tooltip("Titulo de la seccion (ej: Programacion)")]
    public string titulo;

    [Tooltip("Personas de la seccion, cada una con nombre y rol")]
    public PersonaCreditos[] personas;
}

public class CreditosManager : MonoBehaviour
{
    [Tooltip("Escena a cargar al terminar (lo define quien entra: Mapamundial o menuprincipal)")]
    public static string escenaRetorno = "menuprincipal";

    [Header("Scroll")]
    [Tooltip("Velocidad de subida en pixeles/segundo")]
    public float velocidadScroll = 40f;

    [Tooltip("Margen inicial por debajo de la pantalla")]
    public float margenInicial = 120f;

    [Tooltip("Margen final por encima de la pantalla")]
    public float margenFinal = 120f;

    [Tooltip("Pausa en segundos al terminar el scroll antes de cargar la escena de retorno")]
    public float pausaFinal = 1f;

    [Header("Contenido")]
    public SeccionCreditos[] secciones;

    [Header("Estilo")]
    public Color colorTituloSeccion = new Color(0.074f, 0.706f, 0.035f);
    public Color colorNombre = new Color(0.93f, 0.91f, 0.85f);
    public Color colorRol = new Color(0.074f, 0.706f, 0.035f);
    public float tamanioTitulo = 44f;
    public float tamanioNombre = 30f;

    [Header("Fuentes")]
    [Tooltip("Fuente de los TITULOS de seccion. Si queda vacia, usa fuenteNombres")]
    public TMP_FontAsset fuenteTitulo;

    [Tooltip("Fuente de los nombres y roles")]
    public TMP_FontAsset fuenteNombres;

    [Header("Referencias")]
    public RectTransform contenedor;
    public GameObject botonSaltar;

    private float alturaPantalla = 1080f;
    private bool scrollTerminado = false;

    void Start()
    {
        Time.timeScale = 1f;

        if (contenedor != null && contenedor.parent is RectTransform parentRT)
            alturaPantalla = parentRT.rect.height;
        if (alturaPantalla <= 0f) alturaPantalla = 1080f;

        if (contenedor == null)
        {
            Debug.LogError("CreditosManager: falta la referencia 'contenedor'.");
            return;
        }

        ReconstruirCreditos();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contenedor);
        float altoContenido = contenedor.rect.height;
        contenedor.anchoredPosition = new Vector2(contenedor.anchoredPosition.x, -(alturaPantalla * 0.5f + margenInicial) + altoContenido * 0.5f);
        scrollTerminado = false;
    }

    void Update()
    {
        if (scrollTerminado) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Submit") || Input.GetButtonDown("Cancel"))
        {
            Terminar();
            return;
        }

        float posY = contenedor.anchoredPosition.y + velocidadScroll * Time.deltaTime;
        contenedor.anchoredPosition = new Vector2(contenedor.anchoredPosition.x, posY);

        float altoContenido = contenedor.rect.height;
        if (posY >= alturaPantalla * 0.5f + margenFinal + altoContenido * 0.5f)
        {
            scrollTerminado = true;
            StartCoroutine(IrAlDestino());
        }
    }

    IEnumerator IrAlDestino()
    {
        yield return new WaitForSeconds(pausaFinal);
        Terminar();
    }

    public void Saltar()
    {
        Terminar();
    }

    void Terminar()
    {
        if (scrollTerminado) return;
        scrollTerminado = true;
        SceneTransitionManager.CargarEscenaConFallback(escenaRetorno);
    }

    public void ReconstruirCreditos()
    {
        if (contenedor == null) return;

        for (int i = contenedor.childCount - 1; i >= 0; i--)
        {
            var hijo = contenedor.GetChild(i).gameObject;
            if (Application.isPlaying) Destroy(hijo);
            else DestroyImmediate(hijo);
        }

        string hexNombre = ColorUtility.ToHtmlStringRGB(colorNombre);
        string hexRol = ColorUtility.ToHtmlStringRGB(colorRol);
        TMP_FontAsset fontTitulo = fuenteTitulo != null ? fuenteTitulo : fuenteNombres;

        for (int i = 0; i < secciones.Length; i++)
        {
            var s = secciones[i];
            if (s == null) continue;

            if (!string.IsNullOrEmpty(s.titulo))
                CrearTexto("Titulo", fontTitulo, Mathf.RoundToInt(tamanioTitulo), colorTituloSeccion, s.titulo.ToUpperInvariant());

            if (s.personas != null)
            {
                for (int j = 0; j < s.personas.Length; j++)
                {
                    var p = s.personas[j];
                    if (p == null) continue;

                    bool hayNombre = !string.IsNullOrEmpty(p.nombre);
                    bool hayRol = !string.IsNullOrEmpty(p.rol);

                    var sb = new StringBuilder();
                    if (hayNombre)
                        sb.Append("<color=#").Append(hexNombre).Append(">").Append(p.nombre).Append("</color>");
                    if (hayNombre && hayRol)
                        sb.Append(" — ");
                    if (hayRol)
                        sb.Append("<color=#").Append(hexRol).Append("><b>").Append(p.rol).Append("</b></color>");

                    if (sb.Length > 0)
                        CrearTexto("Persona", fuenteNombres, Mathf.RoundToInt(tamanioNombre), colorNombre, sb.ToString());
                }
            }
        }

        CrearTexto("TituloFooter", fontTitulo, 46, colorTituloSeccion, "OBJETIVO 15 · 2026");
        CrearTexto("Footer", fuenteNombres, 24, colorNombre, "¡Gracias por jugar!");
    }

    TMP_Text CrearTexto(string nombre, TMP_FontAsset font, int tamano, Color color, string contenido)
    {
        var go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(contenedor, false);

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.font = font != null ? font : fuenteNombres;
        tmp.fontSize = tamano;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.text = contenido;
        tmp.enableWordWrapping = true;
        tmp.overflowMode = TextOverflowModes.Overflow;
        tmp.raycastTarget = false;
        return tmp;
    }
}
