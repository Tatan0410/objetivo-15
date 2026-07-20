using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuCrafteo : MonoBehaviour
{
    public static MenuCrafteo instancia;
    public KeyCode teclaCrafteo = KeyCode.Tab;

    private GameObject panelCrafteo;
    private bool menuAbierto = false;

    void Awake() => instancia = this;

    void Start() => CrearMenuUI();

    void Update()
    {
        if (Input.GetKeyDown(teclaCrafteo))
            ToggleMenu();
    }

    void ToggleMenu()
    {
        menuAbierto = !menuAbierto;
        panelCrafteo.SetActive(menuAbierto);
        Time.timeScale = menuAbierto ? 0f : 1f;
    }

    public void CerrarMenu()
    {
        menuAbierto = false;
        panelCrafteo.SetActive(false);
        Time.timeScale = 1f;
    }

    void CrearMenuUI()
    {
        Canvas canvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();
        if (canvas == null) canvas = FindObjectOfType<Canvas>();

        panelCrafteo = new GameObject("PanelCrafteo");
        panelCrafteo.transform.SetParent(canvas.transform, false);
        Image bg = panelCrafteo.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        RectTransform rt = panelCrafteo.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(400, 250);

        CrearTexto(panelCrafteo, "CRAFTEO", 20, new Vector2(0, 80),
                   new Vector2(380, 40), Color.white);

        CrearTexto(panelCrafteo, "Presiona TAB para cerrar",
                   12, new Vector2(0, 55),
                   new Vector2(380, 25), new Color(0.7f, 0.7f, 0.7f));

        CrearBotonArma(panelCrafteo,
            "Lanzador", "Costo: 3 PET + 2 Bolsas + 1 Icopor",
            new Vector2(0, -10), Color.cyan,
            () => { SistemaCrafteo.instancia.CraftearLanzador(); CerrarMenu(); });

        panelCrafteo.SetActive(false);
    }

    void CrearBotonArma(GameObject padre, string nombre, string costo,
                        Vector2 pos, Color color, UnityEngine.Events.UnityAction accion)
    {
        GameObject btn = new GameObject(nombre);
        btn.transform.SetParent(padre.transform, false);
        Image img = btn.AddComponent<Image>();
        img.color = new Color(color.r * 0.3f, color.g * 0.3f, color.b * 0.3f, 1f);

        RectTransform rt = btn.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(360, 55);

        Button b = btn.AddComponent<Button>();
        ColorBlock cb = b.colors;
        cb.highlightedColor = color;
        b.colors = cb;
        b.onClick.AddListener(accion);

        CrearTexto(btn, nombre, 16, new Vector2(-60, 8),
                   new Vector2(240, 25), color);
        CrearTexto(btn, costo, 12, new Vector2(-60, -10),
                   new Vector2(240, 20), new Color(0.8f, 0.8f, 0.8f));
    }

    void CrearTexto(GameObject padre, string texto, int size,
                    Vector2 pos, Vector2 tam, Color color)
    {
        GameObject obj = new GameObject("txt");
        obj.transform.SetParent(padre.transform, false);
        TMP_Text t = obj.AddComponent<TextMeshProUGUI>();
        t.text = texto;
        t.fontSize = size;
        t.color = color;
        t.alignment = TextAlignmentOptions.MidlineLeft;
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = tam;
    }
}
