using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instancia;

    [Header("UI")]
    public string textoGameOver = "GAME OVER";
    public Color colorFondo = new Color(0f, 0f, 0f, 0.7f);
    public Color colorTextoGameOver = new Color(0.9f, 0.2f, 0.2f);
    public string textoReintentar = "Reintentar";
    public string textoMenuPrincipal = "Menú Principal";

    private GameObject canvasGameOver;
    private string escenaActual;

    void Awake()
    {
        if (instancia != null) { Destroy(gameObject); return; }
        instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        BuscarOCrearCanvas();
        if (canvasGameOver != null)
            canvasGameOver.SetActive(false);
    }

    void BuscarOCrearCanvas()
    {
        Transform existing = transform.Find("CanvasGameOver");
        if (existing != null)
        {
            canvasGameOver = existing.gameObject;
            DontDestroyOnLoad(canvasGameOver);
            return;
        }
        CrearCanvasGameOver();
    }

    void CrearCanvasGameOver()
    {
        canvasGameOver = new GameObject("CanvasGameOver");
        Canvas canvas = canvasGameOver.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 998;
        canvasGameOver.AddComponent<CanvasScaler>();
        canvasGameOver.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(canvasGameOver);

        // Fondo semitransparente
        GameObject fondo = new GameObject("Fondo");
        fondo.transform.SetParent(canvasGameOver.transform, false);
        Image imgFondo = fondo.AddComponent<Image>();
        imgFondo.color = colorFondo;
        RectTransform rtFondo = fondo.GetComponent<RectTransform>();
        rtFondo.anchorMin = Vector2.zero;
        rtFondo.anchorMax = Vector2.one;
        rtFondo.sizeDelta = Vector2.zero;

        // Texto "GAME OVER"
        GameObject txtGO = new GameObject("TextoGameOver");
        txtGO.transform.SetParent(canvasGameOver.transform, false);
        TMP_Text tmpGO = txtGO.AddComponent<TextMeshProUGUI>();
        tmpGO.text = textoGameOver;
        tmpGO.fontSize = 56;
        tmpGO.alignment = TextAlignmentOptions.Center;
        tmpGO.color = colorTextoGameOver;
        tmpGO.fontStyle = TMPro.FontStyles.Bold;
        RectTransform rtGO = txtGO.GetComponent<RectTransform>();
        rtGO.anchorMin = new Vector2(0.5f, 0.7f);
        rtGO.anchorMax = new Vector2(0.5f, 0.7f);
        rtGO.pivot = new Vector2(0.5f, 0.5f);
        rtGO.sizeDelta = new Vector2(400, 70);
        rtGO.anchoredPosition = Vector2.zero;

        // Botón Reintentar
        GameObject btnReintentar = CrearBoton(
            canvasGameOver.transform,
            textoReintentar,
            new Vector2(0.5f, 0.45f),
            new Color(0.2f, 0.6f, 0.2f),
            Reintentar
        );

        // Botón Menú Principal
        GameObject btnMenu = CrearBoton(
            canvasGameOver.transform,
            textoMenuPrincipal,
            new Vector2(0.5f, 0.3f),
            new Color(0.6f, 0.2f, 0.2f),
            IrAlMenu
        );
    }

    GameObject CrearBoton(Transform parent, string texto, Vector2 anchorPos, Color colorBase, UnityEngine.Events.UnityAction accion)
    {
        GameObject btnGO = new GameObject("Btn_" + texto);
        btnGO.transform.SetParent(parent, false);

        RectTransform rt = btnGO.AddComponent<RectTransform>();
        rt.anchorMin = anchorPos;
        rt.anchorMax = anchorPos;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(260, 50);
        rt.anchoredPosition = Vector2.zero;

        Image img = btnGO.AddComponent<Image>();
        img.color = colorBase;

        Button btn = btnGO.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(accion);

        GameObject txtGO = new GameObject("Texto");
        txtGO.transform.SetParent(btnGO.transform, false);
        TMP_Text tmp = txtGO.AddComponent<TextMeshProUGUI>();
        tmp.text = texto;
        tmp.fontSize = 24;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.fontStyle = TMPro.FontStyles.Bold;

        RectTransform rtTxt = txtGO.GetComponent<RectTransform>();
        rtTxt.anchorMin = Vector2.zero;
        rtTxt.anchorMax = Vector2.one;
        rtTxt.sizeDelta = Vector2.zero;

        // Navigation estilo visual
        ColorBlock cb = btn.colors;
        cb.highlightedColor = colorBase * 1.3f;
        cb.pressedColor = colorBase * 0.7f;
        btn.colors = cb;

        return btnGO;
    }

    public void MostrarGameOver()
    {
        escenaActual = SceneManager.GetActiveScene().name;
        canvasGameOver.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Reintentar()
    {
        canvasGameOver.SetActive(false);
        Time.timeScale = 1f;
        if (SceneTransitionManager.instancia != null)
            SceneTransitionManager.instancia.CargarEscena(escenaActual);
    }

    public void IrAlMenu()
    {
        canvasGameOver.SetActive(false);
        Time.timeScale = 1f;
        if (SceneTransitionManager.instancia != null)
            SceneTransitionManager.instancia.CargarEscena("menuprincipal");
    }
}
