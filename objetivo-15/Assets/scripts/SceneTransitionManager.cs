using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager instancia;

    [Header("Estilo retro")]
    public Color colorVerdeNeon = new Color(0f, 1f, 0.25f);
    public Sprite fondoSprite;
    public int segmentosBarra = 20;

    [Header("Animación de carga")]
    public Sprite[] spritesCorriendo;
    public float framerateAnimacion = 0.15f;
    public float escalaJugador = 2f;

    [Header("Colores")]
    public Color colorLoading = new Color(0f, 1f, 0.25f);
    public Color colorPorcentaje = new Color(0f, 1f, 0.25f);
    public Color colorDato = new Color(0.5f, 1f, 0.5f);

    [Header("Posiciones")]
    [Range(0f, 1f)] public float posYLoading = 0.92f;
    [Range(0f, 1f)] public float posYBarra = 0.38f;
    [Range(0f, 1f)] public float posYPorcentaje = 0.30f;
    [Range(0f, 1f)] public float posYDato = 0.12f;
    public float posYJugador = 60f;

    [Header("Datos curiosos")]
    [TextArea(2, 3)]
    public string[] datosCuriosos = new string[]
    {
        "Una botella PET tarda hasta 500 años en degradarse.",
        "Cada año, 8 millones de toneladas de plástico llegan al océano.",
        "Reciclar una tonelada de papel salva 17 árboles.",
        "Un cepillo de dientes de plástico tarda 500 años en desaparecer.",
        "Colombia recicla solo el 17% de sus residuos.",
        "Una lata de aluminio puede reciclarse infinitamente.",
        "Los popotes de plástico tardan 200 años en degradarse.",
        "Cada minuto se compran 1 millón de botellas de plástico en el mundo.",
        "Reciclar vidrio ahorra un 30% de energía comparado con hacerlo nuevo.",
        "El 91% del plástico producido no se ha reciclado nunca.",
        "Un pañal desechable tarda 450 años en degradarse.",
        "Por cada 3 botellas recicladas, se evita la emisión de 1 kg de CO₂."
    };

    [Header("Fuente")]
    public TMP_FontAsset fontCarga;

    [Header("Configuración")]
    public float duracionMinima = 2f;

    static bool FontEsValido(TMP_FontAsset font)
    {
        return font != null && font.atlasTexture != null;
    }

    private GameObject canvasCarga;
    private Image imagenJugador;
    private Image[] segmentos;
    private TMP_Text textoPorcentaje;
    private TMP_Text textoDato;
    private bool cargando = false;
    private float barWidth = 400f;

    void Awake()
    {
        if (instancia != null) { Destroy(gameObject); return; }
        instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        BuscarOCrearCanvas();
        if (canvasCarga != null)
            canvasCarga.SetActive(false);
    }

    void BuscarOCrearCanvas()
    {
        Transform existing = transform.Find("CanvasCarga");
        if (existing != null)
            DestroyImmediate(existing.gameObject);

        CrearCanvasCarga();
    }

    void CrearCanvasCarga()
    {
        canvasCarga = new GameObject("CanvasCarga", typeof(RectTransform));
        Canvas canvas = canvasCarga.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasCarga.AddComponent<CanvasScaler>();
        canvasCarga.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(canvasCarga);

        // ─── Fondo negro ───
        GameObject fondo = new GameObject("Fondo", typeof(RectTransform));
        fondo.transform.SetParent(canvasCarga.transform, false);
        Image imgFondo = fondo.AddComponent<Image>();
        if (fondoSprite != null)
            imgFondo.sprite = fondoSprite;
        imgFondo.color = Color.black;
        RectTransform rtFondo = fondo.GetComponent<RectTransform>();
        rtFondo.anchorMin = Vector2.zero;
        rtFondo.anchorMax = Vector2.one;
        rtFondo.sizeDelta = Vector2.zero;

        // ─── "Loading..." arriba ───
        GameObject txtLoading = new GameObject("TextLoading", typeof(RectTransform));
        txtLoading.transform.SetParent(canvasCarga.transform, false);
        TMP_Text tmpLoading = txtLoading.AddComponent<TextMeshProUGUI>();
        tmpLoading.text = "Loading...";
        tmpLoading.fontSize = 28;
        tmpLoading.alignment = TextAlignmentOptions.Center;
        tmpLoading.color = colorLoading;
        if (FontEsValido(fontCarga)) tmpLoading.font = fontCarga;
        RectTransform rtLoading = txtLoading.GetComponent<RectTransform>();
        rtLoading.anchorMin = new Vector2(0.5f, posYLoading);
        rtLoading.anchorMax = new Vector2(0.5f, posYLoading);
        rtLoading.pivot = new Vector2(0.5f, 0.5f);
        rtLoading.sizeDelta = new Vector2(300, 40);
        rtLoading.anchoredPosition = Vector2.zero;

        // ─── Barra de carga segmentada ───
        GameObject barraGO = new GameObject("BarraCarga", typeof(RectTransform));
        barraGO.transform.SetParent(canvasCarga.transform, false);
        RectTransform rtBarra = barraGO.GetComponent<RectTransform>();
        rtBarra.anchorMin = new Vector2(0.5f, posYBarra);
        rtBarra.anchorMax = new Vector2(0.5f, posYBarra);
        rtBarra.pivot = new Vector2(0.5f, 0.5f);
        float barHeight = 24f;
        rtBarra.sizeDelta = new Vector2(barWidth, barHeight);
        rtBarra.anchoredPosition = Vector2.zero;

        Image fondoBarra = barraGO.AddComponent<Image>();
        fondoBarra.color = new Color(0.03f, 0.06f, 0.03f);

        float gap = 3f;
        float segWidth = (barWidth - (segmentosBarra - 1) * gap) / segmentosBarra;
        segmentos = new Image[segmentosBarra];
        for (int i = 0; i < segmentosBarra; i++)
        {
            GameObject seg = new GameObject("Seg" + i, typeof(RectTransform));
            seg.transform.SetParent(barraGO.transform, false);
            Image segImg = seg.AddComponent<Image>();
            segImg.color = colorVerdeNeon;
            segImg.enabled = false;
            RectTransform rtSeg = seg.GetComponent<RectTransform>();
            rtSeg.anchorMin = new Vector2(0f, 0.5f);
            rtSeg.anchorMax = new Vector2(0f, 0.5f);
            rtSeg.pivot = new Vector2(0f, 0.5f);
            rtSeg.sizeDelta = new Vector2(segWidth, barHeight - 4f);
            rtSeg.anchoredPosition = new Vector2(2f + i * (segWidth + gap), 0);
            segmentos[i] = segImg;
        }

        // ─── Texto porcentaje debajo de la barra ───
        GameObject pctGO = new GameObject("TextoPorcentaje", typeof(RectTransform));
        pctGO.transform.SetParent(canvasCarga.transform, false);
        textoPorcentaje = pctGO.AddComponent<TextMeshProUGUI>();
        textoPorcentaje.text = "0%";
        textoPorcentaje.fontSize = 14;
        textoPorcentaje.alignment = TextAlignmentOptions.Center;
        textoPorcentaje.color = colorPorcentaje;
        if (FontEsValido(fontCarga)) textoPorcentaje.font = fontCarga;
        RectTransform rtPct = pctGO.GetComponent<RectTransform>();
        rtPct.anchorMin = new Vector2(0.5f, posYPorcentaje);
        rtPct.anchorMax = new Vector2(0.5f, posYPorcentaje);
        rtPct.pivot = new Vector2(0.5f, 0.5f);
        rtPct.sizeDelta = new Vector2(200, 30);
        rtPct.anchoredPosition = Vector2.zero;

        // ─── Player corriendo sobre la barra ───
        GameObject jugadorGO = new GameObject("JugadorCorriendo", typeof(RectTransform));
        jugadorGO.transform.SetParent(canvasCarga.transform, false);
        imagenJugador = jugadorGO.AddComponent<Image>();
        if (spritesCorriendo == null || spritesCorriendo.Length == 0)
            imagenJugador.sprite = GenerarSpritePlaceholder(64, 64, colorVerdeNeon);
        else
            imagenJugador.sprite = spritesCorriendo[0];
        imagenJugador.preserveAspect = true;
        RectTransform rtJugador = jugadorGO.GetComponent<RectTransform>();
        rtJugador.anchorMin = new Vector2(0.5f, 0.5f);
        rtJugador.anchorMax = new Vector2(0.5f, 0.5f);
        rtJugador.pivot = new Vector2(0.5f, 0f);
        float pW = 84f * escalaJugador;
        float pH = 133f * escalaJugador;
        rtJugador.sizeDelta = new Vector2(pW, pH);

        // ─── Dato curioso abajo ───
        GameObject datoGO = new GameObject("TextoDato", typeof(RectTransform));
        datoGO.transform.SetParent(canvasCarga.transform, false);
        textoDato = datoGO.AddComponent<TextMeshProUGUI>();
        textoDato.fontSize = 18;
        textoDato.alignment = TextAlignmentOptions.Center;
        textoDato.color = colorDato;
        if (FontEsValido(fontCarga)) textoDato.font = fontCarga;
        textoDato.text = "";
        RectTransform rtDato = datoGO.GetComponent<RectTransform>();
        rtDato.anchorMin = new Vector2(0.5f, posYDato);
        rtDato.anchorMax = new Vector2(0.5f, posYDato);
        rtDato.pivot = new Vector2(0.5f, 0.5f);
        rtDato.sizeDelta = new Vector2(700, 80);
        rtDato.anchoredPosition = Vector2.zero;
    }

    Sprite GenerarSpritePlaceholder(int w, int h, Color color)
    {
        Texture2D tex = new Texture2D(w, h);
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                tex.SetPixel(x, y, color);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f));
    }

    public bool Ocupado => cargando;

    public void CargarEscena(string nombreEscena)
    {
        if (cargando) return;
        if (canvasCarga == null)
            BuscarOCrearCanvas();
        StartCoroutine(CargarEscenaCoroutine(nombreEscena));
    }

    public static void CargarEscenaConFallback(string nombreEscena)
    {
        if (instancia != null && !instancia.cargando)
            instancia.CargarEscena(nombreEscena);
        else
            SceneManager.LoadScene(nombreEscena);
    }

    IEnumerator CargarEscenaCoroutine(string nombreEscena)
    {
        cargando = true;
        AsyncOperation op = null;
        try
        {
            op = IniciarCarga(nombreEscena);
        }
        catch (System.Exception e)
        {
            Debug.LogError("[SceneTransitionManager] Error al iniciar la transicion: " + e);
        }

        if (op != null)
            yield return StartCoroutine(EsperarCargaYActivar(op));

        if (canvasCarga != null)
            canvasCarga.SetActive(false);
        cargando = false;
    }

    AsyncOperation IniciarCarga(string nombreEscena)
    {
        if (canvasCarga != null)
            canvasCarga.SetActive(true);

        string dato = datosCuriosos[Random.Range(0, datosCuriosos.Length)];
        if (textoDato != null) textoDato.text = dato;

        AsyncOperation op = SceneManager.LoadSceneAsync(nombreEscena);
        if (op == null)
        {
            Debug.LogError("[SceneTransitionManager] No se pudo iniciar la carga de la escena '" + nombreEscena + "'.");
            return null;
        }
        op.allowSceneActivation = false;
        return op;
    }

    IEnumerator EsperarCargaYActivar(AsyncOperation op)
    {
        float tiempoInicio = Time.unscaledTime;
        float tiempoMaximo = Mathf.Max(duracionMinima, 3f) + 15f;
        float duracionBase = Mathf.Max(duracionMinima, 0.1f);
        int frameIndex = 0;
        float pW = 84f * escalaJugador;

        while ((op.progress < 0.9f || Time.unscaledTime - tiempoInicio < duracionMinima)
               && Time.unscaledTime - tiempoInicio < tiempoMaximo)
        {
            float progreso = op.progress > 0f ? Mathf.Clamp01(op.progress / 0.9f) : 0f;
            float avanceTiempo = Mathf.Clamp01((Time.unscaledTime - tiempoInicio) / duracionBase);
            float visual = Mathf.Min(progreso, avanceTiempo);

            // Bloques segmentados
            if (segmentos != null)
            {
                int mostrar = Mathf.FloorToInt(visual * segmentos.Length);
                for (int i = 0; i < segmentos.Length; i++)
                    segmentos[i].enabled = i < mostrar;
            }

            // Porcentaje
            if (textoPorcentaje != null)
                textoPorcentaje.text = Mathf.RoundToInt(visual * 100f) + "%";

            // Player corriendo sincronizado
            if (imagenJugador != null)
            {
                if (spritesCorriendo != null && spritesCorriendo.Length > 0)
                {
                    imagenJugador.sprite = spritesCorriendo[frameIndex % spritesCorriendo.Length];
                    frameIndex = (frameIndex + 1) % spritesCorriendo.Length;
                }
                float x = Mathf.Lerp(-barWidth / 2f, barWidth / 2f - pW, visual);
                imagenJugador.rectTransform.anchoredPosition = new Vector2(x, posYJugador);
            }

            yield return new WaitForSecondsRealtime(framerateAnimacion);
        }

        // Completar todo
        if (segmentos != null)
            for (int i = 0; i < segmentos.Length; i++)
                segmentos[i].enabled = true;
        if (textoPorcentaje != null)
            textoPorcentaje.text = "100%";

        op.allowSceneActivation = true;

        yield return new WaitForSecondsRealtime(0.3f);
    }
}
