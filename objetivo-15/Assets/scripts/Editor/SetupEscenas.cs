using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Events;
using System.IO;

public class SetupEscenas : EditorWindow
{
    [MenuItem("Tools/Setup Escenas del Juego")]
    public static void ShowWindow()
    {
        var w = GetWindow<SetupEscenas>("Setup Escenas");
        w.minSize = new Vector2(400, 300);
    }

    void OnGUI()
    {
        GUILayout.Label("Configuracion de Escenas", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("1. Configurar MenuPrincipal", GUILayout.Height(40)))
            ConfigurarMenuPrincipal();

        EditorGUILayout.Space();

        if (GUILayout.Button("2. Configurar Pausa en nivel1_colegio", GUILayout.Height(40)))
            ConfigurarPausa("nivel1_colegio");

        EditorGUILayout.Space();

        if (GUILayout.Button("3. Configurar Pausa en TODOS los niveles", GUILayout.Height(40)))
        {
            string[] levels = { "nivel1_colegio", "nivel2_hipodromo", "nivel3_mercado",
                                "nivel4_basurero", "nivel5_subterraneo", "nivel6_empresa" };
            foreach (var lv in levels) ConfigurarPausa(lv);
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Abre cada escena despues de configurar para verificar.", MessageType.Info);
    }

    static TMP_FontAsset FindFont()
    {
        var guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
        if (guids.Length > 0)
            return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(guids[0]));
        return null;
    }

    // ─────────────────────── MENU PRINCIPAL ───────────────────────

    static void ConfigurarMenuPrincipal()
    {
        string path = "Assets/Scenes/menuprincipal.unity";
        if (!File.Exists(path)) { EditorUtility.DisplayDialog("Error", "Falta menuprincipal.unity", "OK"); return; }

        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var font = FindFont();

        // EventSystem
        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        // Canvas
        var cgo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = cgo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var cs = cgo.GetComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cs.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        cs.matchWidthOrHeight = 0.5f;

        // Background
        var bg = MakeUI("imagenFondo", cgo.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Vector2.one * 0.5f);
        var bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.08f, 0.12f, 0.2f, 1f);

        // Logo
        var logo = MakeUI("LogoJuego", cgo.transform, new Vector2(0.5f, 1), new Vector2(0.5f, 1),
            new Vector2(0, -50), new Vector2(500, 120), new Vector2(0.5f, 1));
        var logoImg = logo.AddComponent<Image>();
        logoImg.color = new Color(0.15f, 0.5f, 0.9f, 1f);

        // Panel menu
        var pmp = MakeUI("PanelMenuPrincipal", cgo.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(400, 280), Vector2.one * 0.5f);
        var pmpImg = pmp.AddComponent<Image>();
        pmpImg.color = new Color(0, 0, 0, 0.55f);

        // Button Jugar
        var bJugar = MakeButton("BotonJugar", pmp.transform, new Vector2(0.5f, 0.8f), new Vector2(0.5f, 0.8f),
            Vector2.zero, new Vector2(280, 60), Vector2.one * 0.5f, new Color(0.18f, 0.6f, 0.22f, 1f), "JUGAR", font);

        // Button AcercaDe
        var bAcerca = MakeButton("BotonAcercaDe", pmp.transform, new Vector2(0.5f, 0.4f), new Vector2(0.5f, 0.4f),
            Vector2.zero, new Vector2(280, 60), Vector2.one * 0.5f, new Color(0.18f, 0.35f, 0.6f, 1f), "ACERCA DEL PROYECTO", font);

        // Panel AcercaDe
        var pad = MakeUI("PanelAcercaDe", cgo.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(700, 450), Vector2.one * 0.5f);
        var padImg = pad.AddComponent<Image>();
        padImg.color = new Color(0, 0, 0, 0.75f);

        // Planeta
        var pla = MakeUI("PlanetaTierra", pad.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-220, 0), new Vector2(180, 180), Vector2.one * 0.5f);
        var plaImg = pla.AddComponent<Image>();
        plaImg.color = new Color(0.15f, 0.7f, 0.3f, 1f);

        // PanelVineta
        var pv = MakeUI("PanelVineta", pad.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(80, 0), new Vector2(420, 300), Vector2.one * 0.5f);
        var pvImg = pv.AddComponent<Image>();
        pvImg.color = Color.white;

        // TextoInfo
        var ti = MakeUI("TextoInfo", pv.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(380, 260), Vector2.one * 0.5f);
        var tmp = ti.AddComponent<TextMeshProUGUI>();
        tmp.text = "Objetivo 15 es un juego educativo sobre la contaminacion\nplastica en Soledad, Atlantico. Cada nivel representa un\nlugar real afectado por residuos plasticos.\n\nRecicla, craftea y salva tu ciudad!";
        tmp.fontSize = 20;
        tmp.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        tmp.alignment = TextAlignmentOptions.Left;
        if (font) tmp.font = font;

        // Button Volver
        var bVolver = MakeButton("BotonVolver", pad.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -180), new Vector2(200, 50), Vector2.one * 0.5f, new Color(0.6f, 0.18f, 0.18f, 1f), "VOLVER", font);

        // MenuManager
        var mm = new GameObject("MenuManager");
        var mp = mm.AddComponent<MenuPrincipal>();
        mp.panelMenuPrincipal = pmp;
        mp.panelAcercaDe = pad;

        // Connect OnClick
        var bj = bJugar.GetComponent<Button>();
        var ba = bAcerca.GetComponent<Button>();
        var bv = bVolver.GetComponent<Button>();
        UnityEventTools.AddPersistentListener(bj.onClick, mp.Jugar);
        UnityEventTools.AddPersistentListener(ba.onClick, mp.AbrirAcercaDe);
        UnityEventTools.AddPersistentListener(bv.onClick, mp.VolverAlMenu);

        // Initially hide AcercaDe
        pad.SetActive(false);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
        if (!Application.isBatchMode)
            EditorUtility.DisplayDialog("Listo", "MenuPrincipal configurado.", "OK");
    }

    // ─────────────────────── PAUSA ───────────────────────

    static void ConfigurarPausa(string level)
    {
        string path = $"Assets/Scenes/{level}.unity";
        if (!File.Exists(path)) { Debug.LogWarning($"No existe: {path}"); return; }

        EditorSceneManager.OpenScene(path);
        var font = FindFont();

        // Collect objects to destroy first, then destroy them in batch
        var toDestroy = new System.Collections.Generic.List<GameObject>();
        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.scene != EditorSceneManager.GetActiveScene()) continue;
            if (go.name == "PanelPausa" || go.name == "PausaManager" ||
                go.name == "TextoTitulo" || go.name == "BotonReanudar" ||
                go.name == "BotonReiniciar" || go.name == "SliderVolumen" ||
                go.name == "BotonSalir")
            {
                toDestroy.Add(go);
            }
        }
        foreach (var go in toDestroy) Object.DestroyImmediate(go);

        var canvas = Object.FindFirstObjectByType<Canvas>();
        if (!canvas)
        {
            Debug.Log($"Creando Canvas para {level}");
            var evGO = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            var cvsGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = cvsGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        // PanelPausa (fullscreen overlay)
        var pp = MakeUI("PanelPausa", canvas.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Vector2.one * 0.5f);
        pp.AddComponent<Image>().color = new Color(0, 0, 0, 0.75f);

        // Title
        var title = MakeUI("TextoTitulo", pp.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, 160), new Vector2(300, 60), Vector2.one * 0.5f);
        var t = title.AddComponent<TextMeshProUGUI>();
        t.text = "PAUSA";
        t.fontSize = 48;
        t.color = Color.white;
        t.alignment = TextAlignmentOptions.Center;
        if (font) t.font = font;

        // Resume
        var bRes = MakeButton("BotonReanudar", pp.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, 60), new Vector2(280, 55), Vector2.one * 0.5f, new Color(0.18f, 0.6f, 0.22f, 1f), "REANUDAR", font);

        // Restart
        var bRest = MakeButton("BotonReiniciar", pp.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -10), new Vector2(280, 55), Vector2.one * 0.5f, new Color(0.5f, 0.4f, 0.15f, 1f), "REINICIAR", font);

        // Volume slider
        var slider = MakeSlider("SliderVolumen", pp.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -80), new Vector2(300, 30), Vector2.one * 0.5f, font);

        // Quit
        var bQuit = MakeButton("BotonSalir", pp.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -150), new Vector2(280, 55), Vector2.one * 0.5f, new Color(0.6f, 0.18f, 0.18f, 1f), "SALIR AL MENU", font);

        // PausaManager
        var pm = new GameObject("PausaManager");
        var mp = pm.AddComponent<MenuPausa>();
        mp.panelPausa = pp;
        mp.sliderVolumen = slider.GetComponent<Slider>();

        // Connect
        UnityEventTools.AddPersistentListener(bRes.GetComponent<Button>().onClick, mp.Reanudar);
        UnityEventTools.AddPersistentListener(bRest.GetComponent<Button>().onClick, mp.ReiniciarNivel);
        UnityEventTools.AddPersistentListener(bQuit.GetComponent<Button>().onClick, mp.SalirAlMenu);

        pp.SetActive(false); // starts hidden

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
        Debug.Log($"Pausa agregada a {level}");
    }

    // ─────── UI HELPERS ───────

    static GameObject MakeUI(string name, Transform parent, Vector2 aMin, Vector2 aMax,
        Vector2 pos, Vector2 size, Vector2 pivot)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = aMin;
        rt.anchorMax = aMax;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        rt.pivot = pivot;
        return go;
    }

    static GameObject MakeButton(string name, Transform parent, Vector2 aMin, Vector2 aMax,
        Vector2 pos, Vector2 size, Vector2 pivot, Color color, string label, TMP_FontAsset font)
    {
        var go = MakeUI(name, parent, aMin, aMax, pos, size, pivot);
        go.AddComponent<Image>().color = color;
        go.AddComponent<Button>();

        var txt = MakeUI("Text (TMP)", go.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Vector2.one * 0.5f);
        var tmp = txt.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 22;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        if (font) tmp.font = font;
        return go;
    }

    static GameObject MakeSlider(string name, Transform parent, Vector2 aMin, Vector2 aMax,
        Vector2 pos, Vector2 size, Vector2 pivot, TMP_FontAsset font)
    {
        var go = MakeUI(name, parent, aMin, aMax, pos, size, pivot);
        var slider = go.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;
        slider.direction = Slider.Direction.LeftToRight;

        var bg = MakeUI("Background", go.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Vector2.one * 0.5f);
        bg.AddComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 1f);

        var fa = MakeUI("Fill Area", go.transform, Vector2.zero, Vector2.one, new Vector2(-12, 0), new Vector2(-24, 0), Vector2.one * 0.5f);
        var fill = MakeUI("Fill", fa.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Vector2.one * 0.5f);
        fill.AddComponent<Image>().color = new Color(0.2f, 0.6f, 1f, 1f);

        var ha = MakeUI("Handle Slide Area", go.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Vector2.one * 0.5f);
        var hand = MakeUI("Handle", ha.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(20, 20), Vector2.one * 0.5f);
        var hImg = hand.AddComponent<Image>();
        hImg.color = Color.white;

        slider.targetGraphic = hImg;
        slider.fillRect = fill.GetComponent<RectTransform>();
        slider.handleRect = hand.GetComponent<RectTransform>();

        return go;
    }

    // ─────── BATCH MODE ENTRY POINT ───────

    [MenuItem("Tools/Setup Escenas (Automatico)")]
    public static void RunAllBatch()
    {
        ConfigurarMenuPrincipal();

        string[] levels = { "nivel1_colegio", "nivel2_hipodromo", "nivel3_mercado",
                            "nivel4_basurero", "nivel5_subterraneo", "nivel6_empresa" };
        foreach (var lv in levels) ConfigurarPausa(lv);

        Debug.Log("=== TODAS LAS ESCENAS CONFIGURADAS ===");
        if (Application.isBatchMode) EditorApplication.Exit(0);
    }

    [MenuItem("Tools/Setup Solo Pausa")]
    public static void RunPausaBatch()
    {
        string[] levels = { "nivel2_hipodromo", "nivel3_mercado",
                            "nivel4_basurero", "nivel5_subterraneo", "nivel6_empresa" };
        foreach (var lv in levels) ConfigurarPausa(lv);

        Debug.Log("=== PAUSA AGREGADA A NIVELES RESTANTES ===");
        if (Application.isBatchMode) EditorApplication.Exit(0);
    }
}
