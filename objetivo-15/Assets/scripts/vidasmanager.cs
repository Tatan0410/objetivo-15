using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class VidasManager : MonoBehaviour
{
    public static VidasManager instancia;

    [Header("Configuración")]
    public int vidasMaximas = 5;
    public int vidasIniciales = 3;
    public int vidasActuales;
    public float separacionCorazones = 36f;
    public Vector2 tamanoCorazon = new Vector2(32, 32);

    [Header("UI - Corazon")]
    public Sprite corazonLleno;
    public string rutaCorazonResources = "Sprites/corazonmaincra";
    public GameObject panelHUD;

    private List<Image> corazones = new List<Image>();

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private bool corazonesCreados = false;

    void Start()
    {
        CargarSpriteCorazon();
        vidasActuales = vidasIniciales;
        if (EsNivelDeJuego())
            ActualizarUI();
    }

    void Update()
    {
        if (!corazonesCreados && EsNivelDeJuego())
        {
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                ActualizarUI();
                corazonesCreados = true;
            }
        }
    }

    void CargarSpriteCorazon()
    {
        if (corazonLleno != null) return;

        // 1. Resources.Load<Sprite>
        if (!string.IsNullOrEmpty(rutaCorazonResources))
            corazonLleno = Resources.Load<Sprite>(rutaCorazonResources);

        // 2. Resources.LoadAll<Sprite> (mas confiable en algunas versiones de Unity)
        if (corazonLleno == null && !string.IsNullOrEmpty(rutaCorazonResources))
        {
            Sprite[] all = Resources.LoadAll<Sprite>(rutaCorazonResources);
            if (all != null && all.Length > 0)
                corazonLleno = all[0];
        }

        // 3. Fallback procedural
        if (corazonLleno == null)
        {
            corazonLleno = IconoUtils.GenerarCorazon(32, 100, new Color(1f, 0.2f, 0.2f));
            Debug.LogWarning("[Vidas] Usando corazon procedural (Resources.Load fallo para: " + rutaCorazonResources + ")");
        }
        else
        {
            Debug.Log("[Vidas] Sprite cargado: " + corazonLleno.name);
        }
    }

    public void PerderVida(bool ignorarInmortalidad = false)
    {
        if (!ignorarInmortalidad)
        {
            PlayerController pc = GameManager.instancia?.jugador?
                .GetComponent<PlayerController>();
            if (pc != null && pc.EsInmortal()) { Debug.Log("[Vidas DEBUG] PerderVida BLOQUEADO por inmortalidad"); return; }
        }

        Debug.Log($"[Vidas DEBUG] PerderVida: vidasActuales ANTES={vidasActuales} ignorarInmortalidad={ignorarInmortalidad}");
        vidasActuales--;
        Debug.Log($"[Vidas DEBUG] PerderVida: vidasActuales DESPUÉS={vidasActuales}");
        ActualizarUI();

        if (vidasActuales <= 0)
            MorirJugador();
    }

    public void AgregarVida()
    {
        if (vidasActuales < vidasMaximas)
        {
            vidasActuales++;
            ActualizarUI();
            Debug.Log("VIDA GANADA! Vidas: " + vidasActuales);
        }
        else
        {
            Debug.Log("Ya tienes el maximo de vidas: " + vidasMaximas);
        }
    }

    void MorirJugador()
    {
        vidasActuales = vidasIniciales;
        ActualizarUI();

        if (GameManager.instancia != null)
            GameManager.instancia.ResetearCheckpoint();

        if (GameOverManager.instancia != null)
            GameOverManager.instancia.MostrarGameOver();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnEscenaCargada;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnEscenaCargada;
    }

    void OnEscenaCargada(Scene escena, LoadSceneMode modo)
    {
        corazonesCreados = false;
        if (!EsNivelDeJuego())
        {
            LimpiarCorazones(); return;
        }
        corazones.Clear();
        vidasActuales = vidasIniciales;
        ActualizarUI();
        corazonesCreados = true;
    }

    bool EsNivelDeJuego()
    {
        return EsNivelDeJuego(SceneManager.GetActiveScene());
    }

    bool EsNivelDeJuego(Scene escena)
    {
        return escena.name.StartsWith("nivel");
    }

    void LimpiarCorazones()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return;
        for (int i = canvas.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = canvas.transform.GetChild(i);
            if (child.name.StartsWith("Corazon"))
                Destroy(child.gameObject);
        }
    }

    void ActualizarUI()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("[Vidas] No se encontro Canvas");
            return;
        }

        // ─── Limpiar corazones viejos que esten colgando directo del Canvas (sistema anterior) ───
        for (int i = canvas.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = canvas.transform.GetChild(i);
            if (child.name.StartsWith("Corazon"))
            {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
        }

        // ─── Contenedor ───
        GameObject contenedor = GameObject.Find("ContenedorCorazones");
        if (contenedor == null)
        {
            contenedor = new GameObject("ContenedorCorazones", typeof(RectTransform));
            contenedor.transform.SetParent(canvas.transform, false);
            RectTransform rt = contenedor.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(10, -10);
            rt.sizeDelta = new Vector2(180, 36);
        }

        for (int i = contenedor.transform.childCount - 1; i >= 0; i--)
            Destroy(contenedor.transform.GetChild(i).gameObject);

        CargarSpriteCorazon();
        corazones.Clear();

        // ─── Crear corazones dinámicos ───
        for (int i = 0; i < vidasActuales; i++)
        {
            GameObject go = new GameObject("Corazon" + (i + 1), typeof(RectTransform), typeof(CanvasRenderer));
            go.transform.SetParent(contenedor.transform, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0.5f);
            rt.anchorMax = new Vector2(0f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(i * separacionCorazones, 0);
            rt.sizeDelta = tamanoCorazon;

            Image img = go.AddComponent<Image>();
            img.sprite = corazonLleno;
            img.preserveAspect = true;
            img.color = Color.white;

            corazones.Add(img);
        }
    }
}
