using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Events;
using UnityEngine.TextCore.LowLevel;
using System.IO;
using System.Collections.Generic;

public class SetupEscenas : EditorWindow
{
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

        if (GUILayout.Button("2. Configurar Cutscene de Soluciones", GUILayout.Height(40)))
            ConfigurarCutsceneSoluciones();

        EditorGUILayout.Space();

        if (GUILayout.Button("3. Configurar Mapamundial", GUILayout.Height(40)))
            ConfigurarMapamundial();

        EditorGUILayout.Space();

        if (GUILayout.Button("4. Configurar Pausa en nivel1_colegio", GUILayout.Height(40)))
            ConfigurarPausa("nivel1_colegio");

        EditorGUILayout.Space();

        if (GUILayout.Button("5. Configurar Pausa en TODOS los niveles", GUILayout.Height(40)))
        {
            string[] levels = { "nivel1_colegio", "nivel2_hipodromo", "nivel3_mercado",
                                "nivel4_basurero", "nivel5_subterraneo", "nivel6_empresa" };
            foreach (var lv in levels) ConfigurarPausa(lv);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("5b. Agregar botones Crafteo/Pausa + panel crafteo fijo en TODOS los niveles", GUILayout.Height(40)))
        {
            string[] levels = { "nivel1_colegio", "nivel2_hipodromo", "nivel3_mercado",
                                "nivel4_basurero", "nivel5_subterraneo", "nivel6_empresa" };
            foreach (var lv in levels) ConfigurarBotonesUI(lv);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("6. Limpiar power-ups directos de nivel1_colegio", GUILayout.Height(40)))
            LimpiarPotenciadoresDirectos();

        EditorGUILayout.Space();

        if (GUILayout.Button("7. Ajustar posicion Y de enemigos en nivel1_colegio", GUILayout.Height(40)))
            AjustarEnemigos();

        EditorGUILayout.Space();

        if (GUILayout.Button("8. Colocar Municion en nivel1_colegio", GUILayout.Height(40)))
            ColocarMunicion();

        EditorGUILayout.Space();

        if (GUILayout.Button("9. Reparar HUD (corazones + iconos)", GUILayout.Height(40)))
            RepararHUD();

        EditorGUILayout.Space();

        if (GUILayout.Button("10. Normalizar escala de enemigos (fix pisada)", GUILayout.Height(40)))
            NormalizarEnemigos();

        EditorGUILayout.Space();

        if (GUILayout.Button("11. Configurar Escena Final (tras 6 niveles)", GUILayout.Height(40)))
            ConfigurarEscenaFinal();

        EditorGUILayout.Space();

        if (GUILayout.Button("12. Configurar Creditos", GUILayout.Height(40)))
            ConfigurarCreditos();

        EditorGUILayout.Space();

        if (GUILayout.Button("13. Boton 'Rejugar desde 0' en Mapamundial", GUILayout.Height(40)))
            AgregarBotonRejugarMapamundial();

        EditorGUILayout.Space();

        if (GUILayout.Button("14. Fijar numeroNivel en FinNivel (1-6)", GUILayout.Height(40)))
            FijarNumeroNivelesFin();

        EditorGUILayout.Space();

        if (GUILayout.Button("15. Ejecutar todo lo nuevo (finales + creditos + rejugar)", GUILayout.Height(40)))
            RunFinalesBatch();

        EditorGUILayout.Space();

        if (GUILayout.Button("16. Agregar Boton Creditos en MenuPrincipal", GUILayout.Height(40)))
            AgregarBotonCreditosMenu();

        EditorGUILayout.Space();

        if (GUILayout.Button("17. Crear fuente Roboto SDF (espanol)", GUILayout.Height(40)))
            CrearFuenteRobotoEspanol();

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Abre cada escena despues de configurar para verificar.", MessageType.Info);
    }

    static TMP_FontAsset FindFont()
    {
        var guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
        foreach (var g in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(g);
            if (Path.GetFileNameWithoutExtension(path).Contains("PixelifySans"))
                return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
        }
        if (guids.Length > 0)
            return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(guids[0]));
        return null;
    }

    // ─────────────────────── FUENTE ROBOTO SDF (ESPAÑOL) ───────────────────────

    public static void CrearFuenteRobotoEspanol()
    {
        string pathFuente = "Assets/fonts/Roboto-VariableFont_wdth,wght.ttf";
        string pathAsset = "Assets/fonts/Roboto SDF.asset";
        var font = AssetDatabase.LoadAssetAtPath<Font>(pathFuente);
        if (font == null)
        {
            Debug.LogError("No se encontro la fuente: " + pathFuente);
            return;
        }

        var existente = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(pathAsset);
        if (existente != null)
            AssetDatabase.DeleteAsset(pathAsset);

        var fontAsset = TMP_FontAsset.CreateFontAsset(font, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, true);
        if (fontAsset == null)
        {
            Debug.LogError("No se pudo crear el font asset SDF para Roboto.");
            return;
        }

        string espanol = " abcdefghijklmnñopqrstuvwxyzABCDEFGHIJKLMNÑOPQRSTUVWXYZ" +
                         "áéíóúüÁÉÍÓÚÜ¡¿·—.,;:!?()[]{}<>/-_+*=@#$%&\"'0123456789";
        fontAsset.TryAddCharacters(espanol, out string faltantes, true);
        if (!string.IsNullOrEmpty(faltantes))
            Debug.LogWarning("Caracteres no disponibles en Roboto: [" + faltantes + "]");

        fontAsset.name = "Roboto SDF";
        AssetDatabase.CreateAsset(fontAsset, pathAsset);
        EditorUtility.SetDirty(fontAsset);
        AssetDatabase.SaveAssets();

        Debug.Log("Fuente Roboto SDF creada en " + pathAsset + " (glyphs=" + fontAsset.glyphTable.Count + ")");
        if (!Application.isBatchMode)
            EditorUtility.DisplayDialog("Listo", "Roboto SDF (espanol) creada.", "OK");
    }

    // ─────────────────────── MENU PRINCIPAL ───────────────────────

    static void ConfigurarMenuPrincipal()
    {
        string path = "Assets/Scenes/menuprincipal.unity";
        if (!File.Exists(path)) { EditorUtility.DisplayDialog("Error", "Falta menuprincipal.unity", "OK"); return; }

        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var font = FindFont();

        // Camera
        var camGO = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        camGO.tag = "MainCamera";
        camGO.transform.position = new Vector3(0, 0, -10);

        // Light
        var lightGO = new GameObject("Directional Light", typeof(Light));
        lightGO.GetComponent<Light>().type = LightType.Directional;
        lightGO.transform.rotation = Quaternion.Euler(50, -30, 0);

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

        var spriteFondo = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/portadajuego.png");
        var spriteBoton = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/botonescopia.png");
        var spriteLogoColegio = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/logocolegio.png");

        // Background (portadajuego.png covers full screen with logo integrated)
        var bg = MakeUI("imagenFondo", cgo.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Vector2.one * 0.5f);
        var bgImg = bg.AddComponent<Image>();
        bgImg.sprite = spriteFondo;
        bgImg.color = Color.white;

        // School logo (top-right corner)
        var logoCol = MakeUI("LogoColegio", cgo.transform, new Vector2(1, 1), new Vector2(1, 1),
            new Vector2(-60, -60), new Vector2(100, 100), new Vector2(0.5f, 0.5f));
        var logoColImg = logoCol.AddComponent<Image>();
        logoColImg.sprite = spriteLogoColegio;
        logoColImg.color = Color.white;
        logoColImg.preserveAspect = true;

        // Panel menu (semi-transparent background for buttons)
        var pmp = MakeUI("PanelMenuPrincipal", cgo.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(400, 280), Vector2.one * 0.5f);
        var pmpImg = pmp.AddComponent<Image>();
        pmpImg.color = new Color(0, 0, 0, 0.55f);

        // Button Jugar
        var bJugar = MakeButton("BotonJugar", pmp.transform, new Vector2(0.5f, 0.8f), new Vector2(0.5f, 0.8f),
            Vector2.zero, new Vector2(280, 60), Vector2.one * 0.5f, Color.white, "JUGAR", font);
        if (spriteBoton) bJugar.GetComponent<Image>().sprite = spriteBoton;

        // Button Soluciones
        var bSol = MakeButton("BotonSoluciones", pmp.transform, new Vector2(0.5f, 0.4f), new Vector2(0.5f, 0.4f),
            Vector2.zero, new Vector2(280, 60), Vector2.one * 0.5f, Color.white, "SOLUCIONES", font);
        if (spriteBoton) bSol.GetComponent<Image>().sprite = spriteBoton;

        // MenuManager
        var mm = new GameObject("MenuManager");
        var mp = mm.AddComponent<MenuPrincipal>();
        mp.panelMenuPrincipal = pmp;

        // Connect OnClick
        var bj = bJugar.GetComponent<Button>();
        var bs = bSol.GetComponent<Button>();
        UnityEventTools.AddPersistentListener(bj.onClick, mp.Jugar);
        UnityEventTools.AddPersistentListener(bs.onClick, mp.AbrirSoluciones);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
        if (!Application.isBatchMode)
            EditorUtility.DisplayDialog("Listo", "MenuPrincipal configurado.", "OK");
    }

    // ─────────────────────── PAUSA ───────────────────────

    public static void ConfigurarPausa(string level)
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
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
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
        Debug.Log($"Pausa fija agregada a {level}");
    }

    public static void ConfigurarBotonesUI(string level)
    {
        string path = $"Assets/Scenes/{level}.unity";
        if (!File.Exists(path)) { Debug.LogWarning($"No existe: {path}"); return; }

        EditorSceneManager.OpenScene(path);
        var font = FindFont();

        // ─── Limpiar objetos previos de esta configuracion ───
        var toDestroy = new System.Collections.Generic.List<GameObject>();
        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.scene != EditorSceneManager.GetActiveScene()) continue;
            if (go.name == "BtnPausa" || go.name == "BtnCrafteo" ||
                go.name == "PanelCrafteo" || go.name == "PanelCrafteoFijo")
            {
                toDestroy.Add(go);
            }
        }
        foreach (var go in toDestroy) Object.DestroyImmediate(go);

        var canvas = Object.FindFirstObjectByType<Canvas>();
        if (!canvas)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            var cvsGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = cvsGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        // ─── Boton PAUSA (arriba-derecha) ───
        var bPausa = MakeButton("BtnPausa", canvas.transform,
            new Vector2(1f, 1f), new Vector2(1f, 1f),
            new Vector2(-10, -10), new Vector2(60, 60), new Vector2(1f, 1f),
            new Color(0.2f, 0.2f, 0.5f, 0.9f), "PAUSA", font);

        // ─── Boton CRAFTEO (abajo-izquierda) ───
        var bCrafteo = MakeButton("BtnCrafteo", canvas.transform,
            new Vector2(0f, 0f), new Vector2(0f, 0f),
            new Vector2(10, 10), new Vector2(60, 60), new Vector2(0f, 0f),
            new Color(0.2f, 0.6f, 0.3f, 0.9f), "CRAFT", font);

        // ─── Panel Crafteo FIJO (fullscreen overlay, hijo del canvas) ───
        var panel = MakeUI("PanelCrafteoFijo", canvas.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Vector2.one * 0.5f);
        panel.AddComponent<Image>().color = new Color(0, 0, 0, 0.85f);

        var btnLanzador = MakeButton("Btn_CrafteoLanzador", panel.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, 40), new Vector2(260, 70), Vector2.one * 0.5f,
            new Color(0.2f, 0.6f, 0.3f, 1f), "CREAR LANZADOR", font);

        var btnCerrar = MakeButton("BtnCerrarCrafteo", panel.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -60), new Vector2(260, 55), Vector2.one * 0.5f,
            new Color(0.6f, 0.18f, 0.18f, 1f), "CERRAR", font);

        var titulo = MakeUI("TextoSubtitulo", panel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, 130), new Vector2(400, 30), Vector2.one * 0.5f);
        var tmpTitulo = titulo.AddComponent<TextMeshProUGUI>();
        tmpTitulo.text = "MENU DE CRAFTEO";
        tmpTitulo.fontSize = 30;
        tmpTitulo.color = Color.white;
        tmpTitulo.alignment = TextAlignmentOptions.Center;
        if (font) tmpTitulo.font = font;

        var costo = MakeUI("TextoCosto", panel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -5), new Vector2(400, 25), Vector2.one * 0.5f);
        var tmpCosto = costo.AddComponent<TextMeshProUGUI>();
        tmpCosto.text = "Costo: 3 PET + 2 Bolsas + 1 Icopor";
        tmpCosto.fontSize = 16;
        tmpCosto.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        tmpCosto.alignment = TextAlignmentOptions.Center;
        if (font) tmpCosto.font = font;

        panel.SetActive(false); // starts hidden

        // ─── Conectar con MenuCrafteo (ya existente en la escena) ───
        var mc = Object.FindFirstObjectByType<MenuCrafteo>();
        if (mc == null)
        {
            var mcGO = new GameObject("MenuCrafteo");
            mc = mcGO.AddComponent<MenuCrafteo>();
        }
        mc.panelCrafteo = panel;
        mc.gameObject.name = "MenuCrafteo";

        // Conectar botones
        UnityEventTools.AddPersistentListener(bPausa.GetComponent<Button>().onClick, PausarJuego);
        UnityEventTools.AddPersistentListener(bCrafteo.GetComponent<Button>().onClick, mc.AbrirCrafteo);
        UnityEventTools.AddPersistentListener(btnLanzador.GetComponent<Button>().onClick, mc.CraftearYCerrar);
        UnityEventTools.AddPersistentListener(btnCerrar.GetComponent<Button>().onClick, mc.CerrarMenu);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
        Debug.Log($"Botones UI y panel crafteo agregados a {level}");
    }

    static void PausarJuego()
    {
        if (MenuPausa.instancia != null)
            MenuPausa.instancia.Pausar();
    }

    // ─────────────────────── CUTSCENE SOLUCIONES ───────────────────────

    static void ConfigurarCutsceneSoluciones()
    {
        string path = "Assets/Scenes/cutscene_soluciones.unity";
        if (!File.Exists(path)) { Debug.Log("Creando cutscene_soluciones.unity"); }

        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var font = FindFont();

        var spriteFondo = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/sagradoxdddddd.jpeg");
        var spriteGlobo = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/nubecitadialogo-removebg-preview.png");
        var spriteBocaAbierta = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/AMBIENTALINNOJODA-removebg-preview.png");
        var spriteBocaCerrada = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/AMBIENTALINBEMBA-removebg-preview.png");

        // Camera
        var camGO = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        camGO.tag = "MainCamera";
        camGO.transform.position = new Vector3(0, 0, -10);

        // Light
        var lightGO = new GameObject("Directional Light", typeof(Light));
        lightGO.GetComponent<Light>().type = LightType.Directional;
        lightGO.transform.rotation = Quaternion.Euler(50, -30, 0);

        // EventSystem
        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        // Canvas 1920x1080
        var cgo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = cgo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var cs = cgo.GetComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cs.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        cs.matchWidthOrHeight = 0.5f;

        // Background image (fullscreen, white tint, sprite)
        var bg = MakeUI("imagenfondo", cgo.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Vector2.one * 0.5f);
        var bgImg = bg.AddComponent<Image>();
        bgImg.sprite = spriteFondo;
        bgImg.color = Color.white;

        // Speech bubble panel (center, exact position from cutscene_0)
        var panel = MakeUI("paneldialogo", cgo.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(198, -49), new Vector2(944.259f, 653.167f), new Vector2(0.5f, 0.5f));
        var panelImg = panel.AddComponent<Image>();
        panelImg.sprite = spriteGlobo;
        panelImg.type = Image.Type.Sliced;
        panelImg.color = new Color(0.9641509f, 1, 0.9641509f, 1);

        // Planet Earth character (bottom-left relative to panel, exact from cutscene_0)
        var pla = MakeUI("PlanetaTierra", cgo.transform, new Vector2(0.5f, 0), new Vector2(0.5f, 0),
            new Vector2(-268, -89), new Vector2(400, 400), new Vector2(0.5f, 0));
        var plaImg = pla.AddComponent<Image>();
        plaImg.sprite = spriteBocaCerrada;
        plaImg.color = Color.white;
        plaImg.preserveAspect = true;

        // Dialog text (child of paneldialogo, fill parent with margins)
        var txt = MakeUI("textodialogo", panel.transform, new Vector2(0, 0), new Vector2(1, 1),
            Vector2.zero, new Vector2(-120, -200), new Vector2(0.5f, 0.5f));
        var tmp = txt.AddComponent<TextMeshProUGUI>();
        tmp.text = "";
        tmp.fontSize = 32;
        tmp.color = new Color(0.111569025f, 0.07929504f, 0.97735846f, 1);
        tmp.alignment = TextAlignmentOptions.TopLeft;
        tmp.fontStyle = TMPro.FontStyles.Bold | TMPro.FontStyles.Italic;
        if (font) tmp.font = font;

        // Next button (bottom-right, exact from cutscene_0)
        var bNext = MakeButton("BotonSiguiente", cgo.transform, new Vector2(1f, 0), new Vector2(1f, 0),
            new Vector2(-100, 60), new Vector2(180, 55), new Vector2(0.5f, 0.5f), new Color(0.19607843f, 0.5882353f, 0.19607843f, 1), "Siguiente →", font);
        var btnText = bNext.GetComponentInChildren<TextMeshProUGUI>();
        btnText.fontSize = 24;

        // Skip button (top-right, exact from cutscene_0)
        var bSkip = MakeButton("BotonSkip", cgo.transform, new Vector2(1f, 1), new Vector2(1f, 1),
            new Vector2(-80, -40), new Vector2(130, 50), new Vector2(0.5f, 0.5f), new Color(0.7843138f, 0.19607843f, 0.19607843f, 1), "Saltar", font);
        var skipText = bSkip.GetComponentInChildren<TextMeshProUGUI>();
        skipText.fontSize = 22;

        // DialogoManager
        var dmGO = new GameObject("DialogoManager");
        var dm = dmGO.AddComponent<DialogoManager>();
        dm.imagenPlaneta = plaImg;
        dm.spriteBocaCerrada = spriteBocaCerrada;
        dm.spriteBocaAbierta = spriteBocaAbierta;
        dm.textoDialogo = tmp;
        dm.botonSiguiente = bNext;
        dm.textoBotonSiguiente = btnText;
        dm.botonSkip = bSkip;
        dm.escenaDestino = "menuprincipal";
        dm.textoSiguienteUltimo = "Volver al Menu →";
        dm.velocidadTexto = 0.06f;

        // Audio source
        var audioGO = new GameObject("Audio Source");
        audioGO.transform.SetParent(dmGO.transform);
        var audioSrc = audioGO.AddComponent<AudioSource>();
        audioSrc.playOnAwake = false;
        dm.audioSource = audioSrc;

        // Dialogos (12 solutions focused on Colombian Caribbean)
        dm.dialogos = new Dialogo[12];
        dm.dialogos[0] = new Dialogo { texto = "¡Hola! Soy Ambientalin. El ODS 15 busca proteger la vida de ecosistemas terrestres. En la region Caribe colombiana, enfrentamos graves problemas de contaminacion plastica y deforestacion. ¡Pero hay soluciones!" };
        dm.dialogos[1] = new Dialogo { texto = "REDUCE los plasticos de un solo uso. En el Atlantico, cada persona genera mas de 1 kg de residuos al dia. Usa bolsas reutilizables, botellas de vidrio y evita los envases desechables. ¡Pequeños cambios, grandes resultados!" };
        dm.dialogos[2] = new Dialogo { texto = "RECICLA correctamente. Separa el plastico, el vidrio y el papel en tu hogar. En Soledad, el reciclaje adecuado evita que los residuos terminen contaminando el suelo y las fuentes de agua." };
        dm.dialogos[3] = new Dialogo { texto = "APOYA la ganaderia sostenible. El proyecto Ganaderia Colombiana Sostenible integra arboles nativos en la produccion ganadera, protegiendo los bosques secos tropicales del Caribe y capturando carbono." };
        dm.dialogos[4] = new Dialogo { texto = "RESTAURA los ecosistemas. La siembra de arboles nativos en el Atlantico ayuda a recuperar el bosque seco tropical. Mas del 70% de estos bosques han desaparecido en la region. ¡Planta un arbol!" };
        dm.dialogos[5] = new Dialogo { texto = "ADOPTA la economia circular. Transforma los residuos en nuevos productos en lugar de desecharlos. En el Caribe, iniciativas de reciclaje comunitario convierten plasticos en materiales utiles." };
        dm.dialogos[6] = new Dialogo { texto = "ELIGE agricultura sostenible. Los sistemas agroecologicos conservan la biodiversidad y protegen el suelo de la degradacion. Consume alimentos locales y de temporada en Soledad." };
        dm.dialogos[7] = new Dialogo { texto = "PROMUEVE la bioeconomia. Los bioplasticos y la bioenergia son alternativas a los derivados del petroleo. La region Caribe tiene un gran potencial para desarrollar productos biologicos renovables." };
        dm.dialogos[8] = new Dialogo { texto = "PROTEGE la biodiversidad del Caribe. El bosque seco tropical alberga especies unicas que solo existen en Colombia. Cuidar estos ecosistemas es proteger nuestro patrimonio natural." };
        dm.dialogos[9] = new Dialogo { texto = "CONSUME responsablemente. Reduce tu huella ecologica eligiendo productos con menos empaques y menor impacto ambiental. Cada decision de compra es un voto por el planeta." };
        dm.dialogos[10] = new Dialogo { texto = "PARTICIPA en tu comunidad. En Soledad y el Atlantico, hay jornadas de limpieza, reciclaje y reforestacion. El cambio comienza cuando la comunidad se une por un mismo objetivo." };
        dm.dialogos[11] = new Dialogo { texto = "Recuerda: la naturaleza es la base de nuestra vida en la Tierra. En el Caribe colombiano, tu eres parte del cambio. ¡Protege los ecosistemas terrestres y juntos lograremos el ODS 15!" };

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
        Debug.Log("Cutscene Soluciones creada");
    }

    // ─────────────────────── MAPAMUNDIAL ───────────────────────

    static void ConfigurarMapamundial()
    {
        string path = "Assets/Scenes/Mapamundial.unity";
        if (!File.Exists(path)) { Debug.LogWarning($"No existe: {path}"); return; }

        EditorSceneManager.OpenScene(path);
        var font = FindFont();

        // Collect existing Canvas/EventSystem to avoid duplicates
        var toDestroy = new System.Collections.Generic.List<GameObject>();
        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.scene != EditorSceneManager.GetActiveScene()) continue;
            if (go.name == "Canvas" || go.name == "EventSystem" ||
                go.name == "BotonMenuPrincipal" || go.name == "NavegacionUI")
            {
                toDestroy.Add(go);
            }
        }
        foreach (var go in toDestroy) Object.DestroyImmediate(go);

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

        // BotonMenuPrincipal (top-left corner)
        var bVolver = MakeButton("BotonMenuPrincipal", cgo.transform, new Vector2(0, 1), new Vector2(0, 1),
            new Vector2(20, -20), new Vector2(180, 55), Vector2.one * 0.5f, new Color(0.18f, 0.35f, 0.6f, 1f), "← MENU", font);

        // NavegacionUI
        var navGO = new GameObject("NavegacionUI");
        var nav = navGO.AddComponent<NavegacionUI>();

        // Find musicafondo in the scene and connect
        var musicGO = GameObject.Find("musicafondo");
        if (musicGO != null)
        {
            nav.musicaFondo = musicGO.GetComponent<AudioSource>();
            if (musicGO.GetComponent<AplicarVolumenMusica>() == null)
                musicGO.AddComponent<AplicarVolumenMusica>();
        }

        // Connect OnClick
        UnityEventTools.AddPersistentListener(bVolver.GetComponent<Button>().onClick, nav.VolverAlMenu);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
        Debug.Log("Mapamundial configurado");
    }

    // ─────────────────────── LIMPIAR POWER-UPS DIRECTOS ───────────────────────

    static void LimpiarPotenciadoresDirectos()
    {
        string path = "Assets/Scenes/nivel1_colegio.unity";
        if (!File.Exists(path)) { Debug.LogWarning($"No existe: {path}"); return; }

        EditorSceneManager.OpenScene(path);

        string[] nombres = { "potenciadorvidas", "potenciadorvelocidad", "potenciadorinmortalidad" };
        var toDestroy = new System.Collections.Generic.List<GameObject>();

        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.scene != EditorSceneManager.GetActiveScene()) continue;
            foreach (var nombre in nombres)
            {
                if (go.name.StartsWith(nombre))
                {
                    toDestroy.Add(go);
                    break;
                }
            }
        }

        foreach (var go in toDestroy)
        {
            Debug.Log($"Eliminando {go.name} de {path}");
            Object.DestroyImmediate(go);
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
        Debug.Log("Potenciadores directos eliminados de nivel1_colegio");
    }

    // ─────────────────────── AJUSTAR ENEMIGOS ───────────────────────

    static void AjustarEnemigos()
    {
        string path = "Assets/Scenes/nivel1_colegio.unity";
        if (!File.Exists(path)) { Debug.LogWarning($"No existe: {path}"); return; }

        EditorSceneManager.OpenScene(path);

        int ajustados = 0;
        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.scene != EditorSceneManager.GetActiveScene()) continue;
            if (go.GetComponent<Enemigo>() == null) continue;

            Vector3 pos = go.transform.position;

            if (go.name.StartsWith("enemigo2"))
                pos.y += 0.83f;
            else if (go.name.StartsWith("enemigo"))
                pos.y += 0.86f;
            else
                continue;

            go.transform.position = pos;
            ajustados++;
            Debug.Log($"Ajustado {go.name}: pos.y → {pos.y}");
        }

        if (ajustados > 0)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
        }
        Debug.Log($"Enemigos ajustados: {ajustados}");
    }

    // ─────────────────────── NORMALIZAR ENEMIGOS ───────────────────────

    static void NormalizarEnemigos()
    {
        string path = "Assets/Scenes/nivel1_colegio.unity";
        if (!File.Exists(path)) { Debug.LogWarning($"No existe: {path}"); return; }

        EditorSceneManager.OpenScene(path);

        var targets = new System.Collections.Generic.Dictionary<string, Vector2>
        {
            { "enemigo (2)", new Vector2(0.72114f, 0.6f) },
            { "enemigo (1)", new Vector2(0.3f, 0.47f) },
            { "enemigo2", new Vector2(0.6f, 0.92f) },
        };

        int ajustados = 0;

        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.scene != EditorSceneManager.GetActiveScene()) continue;
            if (go.GetComponent<Enemigo>() == null) continue;
            if (!targets.TryGetValue(go.name, out var origScale)) continue;

            float uniforme = origScale.x;
            Vector3 oldScale = go.transform.localScale;

            go.transform.localScale = new Vector3(uniforme, uniforme, oldScale.z);

            // Adjust children's localPosition to maintain world position
            foreach (Transform child in go.transform)
            {
                Vector3 childPos = child.localPosition;
                childPos.x *= (oldScale.x / uniforme);
                childPos.y *= (oldScale.y / uniforme);
                child.localPosition = new Vector3(
                    (float)System.Math.Round(childPos.x, 4),
                    (float)System.Math.Round(childPos.y, 4),
                    childPos.z);
            }

            foreach (var col in go.GetComponents<BoxCollider2D>())
            {
                Vector2 size = col.size;
                Vector2 offset = col.offset;

                size.x *= (oldScale.x / uniforme);
                size.y *= (oldScale.y / uniforme);
                offset.x *= (oldScale.x / uniforme);
                offset.y *= (oldScale.y / uniforme);

                col.size = new Vector2(
                    (float)System.Math.Round(size.x, 4),
                    (float)System.Math.Round(size.y, 4));
                col.offset = new Vector2(
                    (float)System.Math.Round(offset.x, 4),
                    (float)System.Math.Round(offset.y, 4));
            }

            ajustados++;
            Debug.Log($"Normalizado {go.name}: escala {oldScale} → ({uniforme}, {uniforme}), colliders ajustados.");
        }

        if (ajustados > 0)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
        }
        Debug.Log($"Enemigos normalizados: {ajustados}");
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

    // ─────── APLICAR VOLUMEN MUSICA ───────

    static void AgregarAplicarVolumenANiveles()
    {
        string[] levels = { "nivel1_colegio", "nivel2_hipodromo", "nivel3_mercado",
                            "nivel4_basurero", "nivel5_subterraneo", "nivel6_empresa",
                            "Mapamundial" };
        int agregados = 0;
        foreach (var lv in levels)
        {
            string path = $"Assets/Scenes/{lv}.unity";
            if (!File.Exists(path)) { Debug.LogWarning($"No existe: {path}"); continue; }

            EditorSceneManager.OpenScene(path);
            var musicGO = GameObject.Find("musicafondo");
            if (musicGO != null && musicGO.GetComponent<AplicarVolumenMusica>() == null)
            {
                musicGO.AddComponent<AplicarVolumenMusica>();
                agregados++;
                Debug.Log($"AplicarVolumenMusica agregado a {lv}");
            }
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }
        Debug.Log($"AplicarVolumenMusica agregado a {agregados} escenas.");
        if (Application.isBatchMode) EditorApplication.Exit(0);
    }

    // ─────── BATCH MODE ENTRY POINT ───────

    // ─────── COLOCAR MUNICION ───────

    static void ColocarMunicion()
    {
        string path = "Assets/Scenes/nivel1_colegio.unity";
        if (!File.Exists(path)) { Debug.LogWarning($"No existe: {path}"); return; }

        EditorSceneManager.OpenScene(path);

        // Remove existing Municion items group
        var oldGroup = GameObject.Find("Municion");
        if (oldGroup != null) Object.DestroyImmediate(oldGroup);

        GameObject parent = new GameObject("Municion");

        Vector3[] posiciones = new Vector3[]
        {
            new Vector3(6f, -1f, 0),
            new Vector3(15f, 0.5f, 0),
            new Vector3(22f, -1.5f, 0),
            new Vector3(30f, 2f, 0),
            new Vector3(38f, -1f, 0),
            new Vector3(45f, 0f, 0),
            new Vector3(52f, -1.5f, 0),
            new Vector3(60f, 1f, 0),
        };

        for (int i = 0; i < posiciones.Length; i++)
        {
            GameObject mun = new GameObject($"Municion_{i + 1}");
            mun.transform.SetParent(parent.transform);
            mun.transform.position = posiciones[i];
            mun.AddComponent<SpriteRenderer>();
            CircleCollider2D col = mun.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.3f;
            mun.AddComponent<ItemMunicion>();
        }

        string prefabPath = "Assets/prefabs/Municion.prefab";
        GameObject first = parent.transform.GetChild(0).gameObject;
        PrefabUtility.SaveAsPrefabAsset(first, prefabPath);
        Debug.Log($"Prefab creado en {prefabPath}");

        foreach (Transform child in parent.transform)
            Object.DestroyImmediate(child.gameObject);

        for (int i = 0; i < posiciones.Length; i++)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent.transform);
            instance.name = $"Municion_{i + 1}";
            instance.transform.position = posiciones[i];
        }

        // ── Create MunicionManager if not present ──
        var mmGO = GameObject.Find("MunicionManager");
        if (mmGO == null)
        {
            mmGO = new GameObject("MunicionManager");
            var mm = mmGO.AddComponent<MunicionManager>();
            mm.balasComunes = 15;
            mm.maximoPorTipo = 30;
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
        Debug.Log($"Municion colocada: {posiciones.Length} items en {path}");
    }

    // ─────── RECONSTRUIR HUD ───────

    // ─────── LIMPIAR Y ACTUALIZAR SISTEMA DE ITEMS ───────

    static void LimpiarItemsObsoletos()
    {
        string path = "Assets/Scenes/nivel1_colegio.unity";
        if (!File.Exists(path)) { Debug.LogWarning($"No existe: {path}"); return; }

        EditorSceneManager.OpenScene(path);

        int eliminados = 0;
        var toDestroy = new System.Collections.Generic.List<GameObject>();
        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.scene != EditorSceneManager.GetActiveScene()) continue;
            if (go.name.StartsWith("tarroplastico") || go.name.StartsWith("tuboplastico") ||
                go.name.StartsWith("Red") || go.name.StartsWith("Escudo") || go.name.StartsWith("LanzaTubos"))
            {
                toDestroy.Add(go);
                eliminados++;
            }
        }
        foreach (var go in toDestroy) Object.DestroyImmediate(go);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
        Debug.Log($"Items obsoletos eliminados: {eliminados}");
    }

    static void CrearPrefabIcopor()
    {
        GameObject go = new GameObject("icopor");

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        var spriteIcopor = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/icopor-removebg-preview.png");
        if (spriteIcopor != null) sr.sprite = spriteIcopor;

        PlasticoItem item = go.AddComponent<PlasticoItem>();
        item.tipo = TipoPlastico.Icopor;

        PrefabUtility.SaveAsPrefabAsset(go, "Assets/prefabs/icopor.prefab");
        Object.DestroyImmediate(go);

        Debug.Log("Prefab de icopor creado en Assets/prefabs/icopor.prefab");
    }

    static void AsignarSkinLanzador()
    {
        string prefabPath = "Assets/prefabs/Lanzador.prefab";
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null) { Debug.LogError("No existe Lanzador.prefab"); return; }

        var arma = prefab.GetComponent<ArmaPlaceholder>();
        if (arma == null) arma = prefab.AddComponent<ArmaPlaceholder>();

        var skin = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/pitolaloco-removebg-preview.png");
        if (skin == null) { Debug.LogError("No se encontro pitolaloco sprite"); return; }

        arma.skinSprite = skin;
        var sr = prefab.GetComponent<SpriteRenderer>();
        if (sr == null) sr = prefab.AddComponent<SpriteRenderer>();
        sr.sprite = skin;

        EditorUtility.SetDirty(prefab);
        AssetDatabase.SaveAssets();
        Debug.Log("Skin pitolaloco asignada a Lanzador.prefab");
    }

    static void EliminarPrefabsObsoletos()
    {
        string[] prefabs = {
            "Assets/prefabs/tarroplastico.prefab",
            "Assets/prefabs/tuboplastico.prefab",
            "Assets/prefabs/Red.prefab",
            "Assets/prefabs/Escudo.prefab",
            "Assets/prefabs/LanzaTubos.prefab",
        };
        int eliminados = 0;
        foreach (var p in prefabs)
        {
            if (File.Exists(p)) { AssetDatabase.DeleteAsset(p); eliminados++; Debug.Log($"Eliminado: {p}"); }
            else Debug.Log($"No existe: {p}");
        }
        AssetDatabase.Refresh();
        Debug.Log($"Prefabs eliminados: {eliminados}");
    }

    // ─────── REPARAR HUD ───────

    static void RepararHUD()
    {
        string path = "Assets/Scenes/nivel1_colegio.unity";
        if (!File.Exists(path)) { Debug.LogWarning($"No existe: {path}"); return; }

        EditorSceneManager.OpenScene(path);
        var font = FindFont();

        var canvas = Object.FindFirstObjectByType<Canvas>();
        if (!canvas)
        {
            Debug.LogError("No hay Canvas en la escena");
            return;
        }

        // ─── Eliminar HUD viejo ───
        var oldHeart = GameObject.Find("ContenedorCorazones");
        if (oldHeart != null) Object.DestroyImmediate(oldHeart);
        var oldRes = GameObject.Find("ContenedorRecursos");
        if (oldRes != null) Object.DestroyImmediate(oldRes);

        // ─── Cargar sprites ───
        var sprLleno = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/corazonmaincra-removebg-preview.png");
        var sprPET = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/botellasinfondo.png");
        var sprBolsa = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/bolsasinfondo.png");
        var sprIcopor = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/icopor-removebg-preview.png");
        var sprMunicion = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/pitolaloco-removebg-preview.png");

        // Fallback procedural si falta algun sprite
        if (sprLleno == null) sprLleno = IconoUtils.GenerarCorazon(32, 100, new Color(1f, 0.2f, 0.2f));
        if (sprPET == null) sprPET = IconoUtils.GenerarCirculo(16, 100, new Color(0.2f, 0.8f, 0.2f));
        if (sprBolsa == null) sprBolsa = IconoUtils.GenerarCirculo(16, 100, new Color(0.2f, 0.5f, 0.9f));
        if (sprIcopor == null) sprIcopor = IconoUtils.GenerarCirculo(16, 100, new Color(0.9f, 0.8f, 0.2f));
        if (sprMunicion == null) sprMunicion = IconoUtils.GenerarCirculo(16, 100, new Color(1f, 0.6f, 0.1f));

        // ─── CONTENEDOR DE CORAZONES (los corazones se crean dinamicamente en runtime) ───
        var heartParent = MakeUI("ContenedorCorazones", canvas.transform,
            new Vector2(0f, 1f), new Vector2(0f, 1f),
            new Vector2(10, -10), new Vector2(180, 36), new Vector2(0f, 1f));

        var vm = Object.FindFirstObjectByType<VidasManager>();
        if (vm == null)
        {
            var vmGO = new GameObject("VidasManager");
            vm = vmGO.AddComponent<VidasManager>();
            Debug.Log("VidasManager creado en la escena");
        }
        vm.corazonLleno = sprLleno;
        Debug.Log("Corazon sprite asignado a VidasManager");

        // ─── CONTADORES DE RECURSOS ───
        var resParent = MakeUI("ContenedorRecursos", canvas.transform,
            new Vector2(0f, 1f), new Vector2(0f, 1f),
            new Vector2(10, -55), new Vector2(200, 120), new Vector2(0f, 1f));

        System.Func<string, Sprite, int, ContadorHUD> crearContador =
            (nombre, iconSprite, index) =>
        {
            var row = MakeUI(nombre, resParent.transform,
                new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(0, -index * 28), new Vector2(200, 24), new Vector2(0f, 1f));
            var iconGO = MakeUI("Icono", row.transform,
                new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
                new Vector2(0, 0), new Vector2(80, 80), new Vector2(0.5f, 0.5f));
            var iconImg = iconGO.AddComponent<Image>();
            iconImg.sprite = iconSprite;
            iconImg.preserveAspect = true;
            var txtGO = MakeUI("Numero", row.transform,
                new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
                new Vector2(26, 0), new Vector2(80, 24), new Vector2(0f, 0.5f));
            var tmp = txtGO.AddComponent<TextMeshProUGUI>();
            tmp.text = "0";
            tmp.fontSize = 18;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Left;
            if (font) tmp.font = font;
            var cont = row.AddComponent<ContadorHUD>();
            cont.icono = iconImg;
            cont.texto = tmp;
            return cont;
        };

        var contPET = crearContador("ContPET", sprPET, 0);
        var contBolsa = crearContador("ContBolsa", sprBolsa, 1);
        var contIcopor = crearContador("ContIcopor", sprIcopor, 2);
        var contMunicion = crearContador("ContMunicion", sprMunicion, 3);

        var inv = Object.FindFirstObjectByType<Inventario>();
        if (inv != null)
        {
            inv.contadorPET = contPET;
            inv.contadorBolsa = contBolsa;
            inv.contadorIcopor = contIcopor;
            Debug.Log("Contadores conectados a Inventario");
        }
        else Debug.LogError("No se encontro Inventario");

        var mm = Object.FindFirstObjectByType<MunicionManager>();
        if (mm != null)
        {
            mm.AsignarContador(contMunicion);
            Debug.Log("Contador de municion conectado");
        }
        else Debug.LogError("No se encontro MunicionManager");

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
        Debug.Log("HUD reparado correctamente");
    }

    static void RepararNivel(string path, string nombre)
    {
        if (!File.Exists(path)) { Debug.LogWarning($"No existe: {path}"); return; }

        EditorSceneManager.OpenScene(path);
        var font = FindFont();

        // ─── 1. Eliminar items obsoletos ───
        int eliminados = 0;
        var toDestroy = new System.Collections.Generic.List<GameObject>();
        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.scene != EditorSceneManager.GetActiveScene()) continue;
            if (go.name.StartsWith("tarroplastico") || go.name.StartsWith("tuboplastico") ||
                go.name.StartsWith("Red") || go.name.StartsWith("Escudo") || go.name.StartsWith("LanzaTubos"))
            {
                toDestroy.Add(go);
                eliminados++;
            }
        }
        foreach (var go in toDestroy) Object.DestroyImmediate(go);
        if (eliminados > 0) Debug.Log($"{nombre}: Items obsoletos eliminados: {eliminados}");

        // ─── 2. Cargar sprites ───
        var sprLleno = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/corazonmaincra-removebg-preview.png");
        var sprPET = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/botellasinfondo.png");
        var sprBolsa = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/bolsasinfondo.png");
        var sprIcopor = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/icopor-removebg-preview.png");
        var sprMunicion = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/pitolaloco-removebg-preview.png");

        if (sprLleno == null) sprLleno = IconoUtils.GenerarCorazon(32, 100, new Color(1f, 0.2f, 0.2f));
        if (sprPET == null) sprPET = IconoUtils.GenerarCirculo(16, 100, new Color(0.2f, 0.8f, 0.2f));
        if (sprBolsa == null) sprBolsa = IconoUtils.GenerarCirculo(16, 100, new Color(0.2f, 0.5f, 0.9f));
        if (sprIcopor == null) sprIcopor = IconoUtils.GenerarCirculo(16, 100, new Color(0.9f, 0.8f, 0.2f));
        if (sprMunicion == null) sprMunicion = IconoUtils.GenerarCirculo(16, 100, new Color(1f, 0.6f, 0.1f));

        var canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) { Debug.LogError($"{nombre}: No hay Canvas"); return; }

        // ─── 3. VidasManager ───
        var vm = Object.FindFirstObjectByType<VidasManager>();
        if (vm == null)
        {
            var vmGO = new GameObject("VidasManager");
            vm = vmGO.AddComponent<VidasManager>();
            Debug.Log($"{nombre}: VidasManager creado en la escena");
        }
        vm.corazonLleno = sprLleno;

        // ─── 4. ContenedorCorazones (solo si no existe) ───
        var heartParent = GameObject.Find("ContenedorCorazones");
        if (heartParent == null)
        {
            heartParent = MakeUI("ContenedorCorazones", canvas.transform,
                new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(10, -10), new Vector2(180, 36), new Vector2(0f, 1f));
        }

        // ─── 5. ContenedorRecursos (solo si no existe) ───
        var resParent = GameObject.Find("ContenedorRecursos");
        if (resParent == null)
        {
            resParent = MakeUI("ContenedorRecursos", canvas.transform,
                new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(10, -55), new Vector2(200, 120), new Vector2(0f, 1f));

            System.Func<string, Sprite, int, ContadorHUD> crearContador =
                (nom, iconSprite, index) =>
            {
                var row = MakeUI(nom, resParent.transform,
                    new Vector2(0f, 1f), new Vector2(0f, 1f),
                    new Vector2(0, -index * 28), new Vector2(200, 24), new Vector2(0f, 1f));
                var iconGO = MakeUI("Icono", row.transform,
                    new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
                    new Vector2(0, 0), new Vector2(80, 80), new Vector2(0.5f, 0.5f));
                var iconImg = iconGO.AddComponent<Image>();
                iconImg.sprite = iconSprite;
                iconImg.preserveAspect = true;
                var txtGO = MakeUI("Numero", row.transform,
                    new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
                    new Vector2(26, 0), new Vector2(80, 24), new Vector2(0f, 0.5f));
                var tmp = txtGO.AddComponent<TextMeshProUGUI>();
                tmp.text = "0";
                tmp.fontSize = 18;
                tmp.color = Color.white;
                tmp.alignment = TextAlignmentOptions.Left;
                if (font) tmp.font = font;
                var cont = row.AddComponent<ContadorHUD>();
                cont.icono = iconImg;
                cont.texto = tmp;
                return cont;
            };

            var contPET = crearContador("ContPET", sprPET, 0);
            var contBolsa = crearContador("ContBolsa", sprBolsa, 1);
            var contIcopor = crearContador("ContIcopor", sprIcopor, 2);
            var contMunicion = crearContador("ContMunicion", sprMunicion, 3);

            var inv = Object.FindFirstObjectByType<Inventario>();
            if (inv != null)
            {
                inv.contadorPET = contPET;
                inv.contadorBolsa = contBolsa;
                inv.contadorIcopor = contIcopor;
            }
            else Debug.LogError($"{nombre}: No se encontro Inventario");

            var mm = Object.FindFirstObjectByType<MunicionManager>();
            if (mm != null) mm.AsignarContador(contMunicion);
            else Debug.LogError($"{nombre}: No se encontro MunicionManager");
        }

        // ─── 6. Agregar AplicarVolumenMusica si falta ───
        var musicGO = GameObject.Find("musicafondo");
        if (musicGO != null && musicGO.GetComponent<AplicarVolumenMusica>() == null)
        {
            musicGO.AddComponent<AplicarVolumenMusica>();
            Debug.Log($"{nombre}: AplicarVolumenMusica agregado");
        }

        // ─── 7. Reparar prefabsPlasticos de enemigos ───
        var prefabBotella = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/prefabs/botella.prefab");
        var prefabBolsa = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/prefabs/bolsaplastica.prefab");
        var prefabIcopor = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/prefabs/icopor.prefab");
        GameObject[] plasticosActuales = { prefabBotella, prefabBolsa, prefabIcopor };

        int enemigosArreglados = 0;
        foreach (var enemigo in Object.FindObjectsByType<Enemigo>(FindObjectsSortMode.None))
        {
            enemigo.prefabsPlasticos = plasticosActuales;
            enemigosArreglados++;
        }
        foreach (var volador in Object.FindObjectsByType<EnemigoVolador>(FindObjectsSortMode.None))
        {
            volador.prefabsPlasticos = plasticosActuales;
            enemigosArreglados++;
        }
        if (enemigosArreglados > 0)
            Debug.Log($"{nombre}: Prefabs de plasticos asignados a {enemigosArreglados} enemigos");

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
        Debug.Log($"=== {nombre} REPARADO COMPLETAMENTE ===");
    }

    static void RepararNivel2() => RepararNivel("Assets/Scenes/nivel2_hipodromo.unity", "nivel2_hipodromo");

    static void RepararNiveles2a6()
    {
        string[] levels = { "nivel2_hipodromo", "nivel3_mercado", "nivel4_basurero",
                            "nivel5_subterraneo", "nivel6_empresa" };
        foreach (var lv in levels)
            RepararNivel($"Assets/Scenes/{lv}.unity", lv);
        Debug.Log("=== TODOS LOS NIVELES 2-6 REPARADOS ===");
    }

    // ─────── ACTUALIZAR CONTADORES INVENTARIO ───────

    static void ActualizarContadoresInventario()
    {
        string path = "Assets/Scenes/nivel1_colegio.unity";
        if (!File.Exists(path)) { Debug.LogWarning($"No existe: {path}"); return; }

        EditorSceneManager.OpenScene(path);

        var inv = Object.FindFirstObjectByType<Inventario>();
        if (inv == null) { Debug.LogError("No hay Inventario en la escena"); return; }

        // Buscar ContadorHUD por nombre, NO destruir nada
        var todos = Resources.FindObjectsOfTypeAll<ContadorHUD>();
        foreach (var c in todos)
        {
            if (c.gameObject.scene != EditorSceneManager.GetActiveScene()) continue;
            string name = c.gameObject.name;
            if (name == "ContPET") inv.contadorPET = c;
            else if (name == "ContBolsa") inv.contadorBolsa = c;
            else if (name == "ContIcopor") inv.contadorIcopor = c;
        }

        // Si no existe ContIcopor, crearlo
        if (inv.contadorIcopor == null)
        {
            var resParent = GameObject.Find("ContenedorRecursos") ?? GameObject.Find("Canvas");
            if (resParent != null)
            {
                var idx = 2;
                var row = MakeUI("ContIcopor", resParent.transform,
                    new Vector2(0f, 1f), new Vector2(0f, 1f),
                    new Vector2(0, -idx * 28), new Vector2(200, 24), new Vector2(0f, 1f));
                var iconGO = MakeUI("Icono", row.transform,
                    new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
                    new Vector2(0, 0), new Vector2(20, 20), new Vector2(0.5f, 0.5f));
                var iconImg = iconGO.AddComponent<Image>();
                iconImg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/icopor-removebg-preview.png");
                var txtGO = MakeUI("Numero", row.transform,
                    new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
                    new Vector2(26, 0), new Vector2(80, 24), new Vector2(0f, 0.5f));
                var tmp = txtGO.AddComponent<TextMeshProUGUI>();
                tmp.text = "0";
                tmp.fontSize = 18;
                tmp.color = new Color(0.9f, 0.8f, 0.2f);
                tmp.alignment = TextAlignmentOptions.Left;
                var font = FindFont();
                if (font) tmp.font = font;
                var cont = row.AddComponent<ContadorHUD>();
                cont.icono = iconImg;
                cont.texto = tmp;
                inv.contadorIcopor = cont;
            }
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
        Debug.Log("Contadores del Inventario actualizados");
    }

    static void GuardarTexturaComoPNG(string path, Texture2D tex)
    {
        byte[] bytes = tex.EncodeToPNG();

        File.WriteAllBytes(path, bytes);
        Debug.Log("Textura guardada: " + path);
    }

    public static void RunAllBatch()
    {
        ConfigurarMenuPrincipal();
        ConfigurarCutsceneSoluciones();
        ConfigurarMapamundial();

        string[] levels = { "nivel1_colegio", "nivel2_hipodromo", "nivel3_mercado",
                            "nivel4_basurero", "nivel5_subterraneo", "nivel6_empresa" };
        foreach (var lv in levels) ConfigurarPausa(lv);

        LimpiarPotenciadoresDirectos();
        AjustarEnemigos();
        ColocarMunicion();
        AgregarAplicarVolumenANiveles();

        EliminarPrefabsObsoletos();
        CrearPrefabIcopor();
        AsignarSkinLanzador();
        RepararHUD();

        Debug.Log("=== TODAS LAS ESCENAS CONFIGURADAS ===");
        if (Application.isBatchMode) EditorApplication.Exit(0);
    }

    // ─────────────────────── ESCENA FINAL ───────────────────────

    static void ConfigurarEscenaFinal()
    {
        string path = "Assets/Scenes/escena_final.unity";
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var font = FindFont();

        // Camera
        var camGO = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        camGO.tag = "MainCamera";
        camGO.transform.position = new Vector3(0, 0, -10);

        // EventSystem
        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        // Canvas 1920x1080
        var cgo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = cgo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var cs = cgo.GetComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cs.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        cs.matchWidthOrHeight = 0.5f;

        // Fondo
        var bg = MakeUI("imagenFondo", cgo.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Vector2.one * 0.5f);
        bg.AddComponent<Image>().color = new Color(0.04f, 0.09f, 0.05f);

        // Titulo
        var title = MakeUI("TextoTitulo", cgo.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, 310), new Vector2(1400, 90), Vector2.one * 0.5f);
        var tmpTitle = title.AddComponent<TextMeshProUGUI>();
        tmpTitle.text = "¡COMPLETASTE LOS 6 NIVELES!";
        tmpTitle.fontSize = 52;
        tmpTitle.color = Color.white;
        tmpTitle.alignment = TextAlignmentOptions.Center;
        if (font) tmpTitle.font = font;

        // Subtitulo
        var sub = MakeUI("TextoSubtitulo", cgo.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, 225), new Vector2(1400, 50), Vector2.one * 0.5f);
        var tmpSub = sub.AddComponent<TextMeshProUGUI>();
        tmpSub.text = "Has ayudado a proteger los ecosistemas terrestres de la region Caribe.";
        tmpSub.fontSize = 26;
        tmpSub.color = new Color(0.7f, 1f, 0.7f);
        tmpSub.alignment = TextAlignmentOptions.Center;
        if (font) tmpSub.font = font;

        // Espacio para la imagen
        var img = MakeUI("EspacioImagen", cgo.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -45), new Vector2(900, 480), Vector2.one * 0.5f);
        var imgImg = img.AddComponent<Image>();
        imgImg.color = new Color(1f, 1f, 1f, 0.12f);
        imgImg.raycastTarget = false;

        var ph = MakeUI("EtiquetaImagen", img.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(800, 80), Vector2.one * 0.5f);
        var tmpPh = ph.AddComponent<TextMeshProUGUI>();
        tmpPh.text = "COLOCA AQUÍ TU IMAGEN\n(Asigna el sprite al campo 'imagen' de EscenaFinalManager)";
        tmpPh.fontSize = 22;
        tmpPh.color = new Color(1f, 1f, 1f, 0.55f);
        tmpPh.alignment = TextAlignmentOptions.Center;
        if (font) tmpPh.font = font;

        // Boton Continuar
        var bNext = MakeButton("BotonContinuar", cgo.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -350), new Vector2(300, 60), Vector2.one * 0.5f,
            new Color(0.18f, 0.6f, 0.22f, 1f), "CONTINUAR →", font);

        // EscenaFinalManager
        var mgrGO = new GameObject("EscenaFinalManager");
        var mgr = mgrGO.AddComponent<EscenaFinalManager>();
        mgr.imagen = imgImg;
        mgr.etiqueta = tmpPh;
        UnityEventTools.AddPersistentListener(bNext.GetComponent<Button>().onClick, mgr.Continuar);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
        RegistrarEscenasNuevasEnBuild();
        if (!Application.isBatchMode)
            EditorUtility.DisplayDialog("Listo", "Escena final configurada.", "OK");
    }

    // ─────────────────────── CREDITOS ───────────────────────

    static void ConfigurarCreditos()
    {
        string path = "Assets/Scenes/creditos.unity";
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var font = FindFont();

        // Camera
        var camGO = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        camGO.tag = "MainCamera";
        camGO.transform.position = new Vector3(0, 0, -10);

        // EventSystem
        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        // Canvas 1920x1080
        var cgo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = cgo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var cs = cgo.GetComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cs.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        cs.matchWidthOrHeight = 0.5f;

        // Fondo (paleta tierra/urbano del juego)
        var bg = MakeUI("imagenFondo", cgo.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Vector2.one * 0.5f);
        bg.AddComponent<Image>().color = new Color(0.10f, 0.08f, 0.055f);

        // Contenedor de creditos (se mueve en el scroll, sube desde abajo)
        var cont = MakeUI("ContenedorCreditos", cgo.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(1920, 0), Vector2.one * 0.5f);
        var contRt = cont.GetComponent<RectTransform>();

        // Layout vertical: un TextMeshPro por linea (titulo/persona), permite fuente por titulo sin tags crudos
        var vlg = cont.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.spacing = 18f;
        vlg.padding = new RectOffset(60, 60, 0, 0);
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = false;
        vlg.childForceExpandHeight = false;

        var fitterCont = cont.AddComponent<ContentSizeFitter>();
        fitterCont.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Boton Saltar (esquina superior derecha)
        var bSkip = MakeButton("BotonSaltar", cgo.transform, new Vector2(1f, 1f), new Vector2(1f, 1f),
            new Vector2(-80, -40), new Vector2(130, 50), Vector2.one * 0.5f,
            new Color(0.784f, 0.196f, 0.196f, 1f), "SALTAR", font);

        // CreditosManager
        var mgrGO = new GameObject("CreditosManager");
        var mgr = mgrGO.AddComponent<CreditosManager>();
        mgr.contenedor = contRt;
        mgr.botonSaltar = bSkip;
        mgr.velocidadScroll = 40f;
        mgr.colorRol = new Color(0.074f, 0.706f, 0.035f);
        mgr.fuenteNombres = font;
        mgr.fuenteTitulo = null;
        mgr.secciones = new SeccionCreditos[]
        {
            new SeccionCreditos { titulo = "Un juego de", personas = new PersonaCreditos[]
                { new PersonaCreditos { nombre = "[Nombre del estudio]", rol = "Desarrollo" } } },
            new SeccionCreditos { titulo = "Programacion", personas = new PersonaCreditos[]
                { new PersonaCreditos { nombre = "[Nombre 1]", rol = "Programacion" },
                  new PersonaCreditos { nombre = "[Nombre 2]", rol = "Programacion" } } },
            new SeccionCreditos { titulo = "Arte y Diseno", personas = new PersonaCreditos[]
                { new PersonaCreditos { nombre = "[Nombre 1]", rol = "Arte" },
                  new PersonaCreditos { nombre = "[Nombre 2]", rol = "Diseno" } } },
            new SeccionCreditos { titulo = "Agradecimientos especiales", personas = new PersonaCreditos[]
                { new PersonaCreditos { nombre = "A la comunidad educativa", rol = "Agradecimiento" } } },
            new SeccionCreditos { titulo = "Objetivo 15 · 2026", personas = new PersonaCreditos[]
                { new PersonaCreditos { nombre = "Protegiendo los ecosistemas terrestres", rol = "Gracias por jugar" } } },
        };
        UnityEventTools.AddPersistentListener(bSkip.GetComponent<Button>().onClick, mgr.Saltar);

        // Vista previa de los creditos en el editor
        mgr.ReconstruirCreditos();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
        RegistrarEscenasNuevasEnBuild();
        if (!Application.isBatchMode)
            EditorUtility.DisplayDialog("Listo", "Creditos con scroll configurados.", "OK");
    }

    // ─────────────────────── BOTON CREDITOS EN MENU PRINCIPAL ───────────────────────

    static void AgregarBotonCreditosMenu()
    {
        string path = "Assets/Scenes/menuprincipal.unity";
        if (!File.Exists(path)) { Debug.LogWarning($"No existe: {path}"); return; }

        EditorSceneManager.OpenScene(path);
        var font = FindFont();
        var spriteBoton = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/sprites/botonescopia.png");

        var mp = Object.FindFirstObjectByType<MenuPrincipal>();
        if (mp == null || mp.panelMenuPrincipal == null)
        {
            Debug.LogError($"MenuPrincipal: no se encontro MenuPrincipal (mp={mp != null}) / panelMenuPrincipal.");
            return;
        }

        // El "panel" del menu es el root del canvas (menuprincipal.unity no tiene PanelMenuPrincipal)
        var panel = mp.panelMenuPrincipal.transform;

        // Limpiar version previa
        var oldBtn = GameObject.Find("BotonCreditos");
        if (oldBtn != null) Object.DestroyImmediate(oldBtn);

        // Boton Creditos: zona visible con sprite del juego, centrado abajo
        var bCreditos = MakeButton("BotonCreditos", panel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -480), new Vector2(300, 60), Vector2.one * 0.5f, Color.white, "CRÉDITOS", font);
        if (spriteBoton != null)
            bCreditos.GetComponent<Image>().sprite = spriteBoton;

        // Sin listener persistente: MenuPrincipal.Start() engancha el OnClick por nombre (patron de la escena)

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
        if (!Application.isBatchMode)
            EditorUtility.DisplayDialog("Listo", "Boton Creditos agregado al menu principal.", "OK");
    }

    // ─────────────────────── BOTON REJUGAR EN MAPAMUNDIAL ───────────────────────

    static void AgregarBotonRejugarMapamundial()
    {
        string path = "Assets/Scenes/Mapamundial.unity";
        if (!File.Exists(path)) { Debug.LogWarning($"No existe: {path}"); return; }

        EditorSceneManager.OpenScene(path);
        var font = FindFont();

        // Canvas UI: el que contiene a SelectorNivelesMapa / BotonMenuPrincipal
        var selector = Object.FindFirstObjectByType<SelectorNivelesMapa>();
        Transform canvasT = selector != null && selector.transform.parent != null
            ? selector.transform.parent
            : (Object.FindFirstObjectByType<Canvas>() != null ? Object.FindFirstObjectByType<Canvas>().transform : null);
        if (canvasT == null) { Debug.LogError("No hay Canvas en Mapamundial"); return; }

        // Limpiar versiones previas
        var toDestroy = new List<GameObject>();
        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.scene != EditorSceneManager.GetActiveScene()) continue;
            if (go.name == "BotonRejugar" || go.name == "PanelConfirmacionReinicio" || go.name == "ReiniciarProgreso")
                toDestroy.Add(go);
        }
        foreach (var go in toDestroy) Object.DestroyImmediate(go);

        // Manager
        var mgrGO = new GameObject("ReiniciarProgreso");
        var mgr = mgrGO.AddComponent<ReiniciarProgreso>();

        // Boton Rejugar (esquina inferior derecha)
        var bReiniciar = MakeButton("BotonRejugar", canvasT,
            new Vector2(1f, 0f), new Vector2(1f, 0f),
            new Vector2(-20, 20), new Vector2(200, 55), new Vector2(1f, 0f),
            new Color(0.6f, 0.18f, 0.18f, 1f), "REJUGAR DESDE 0", font);
        var tmpB = bReiniciar.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpB != null) tmpB.fontSize = 18;

        // Panel de confirmacion (oculto, tapando toda la pantalla)
        var panel = MakeUI("PanelConfirmacionReinicio", canvasT, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Vector2.one * 0.5f);
        panel.AddComponent<Image>().color = new Color(0, 0, 0, 0.75f);

        var pnBox = MakeUI("CajaConfirmacion", panel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(620, 280), Vector2.one * 0.5f);
        pnBox.AddComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f, 1f);

        var txtConfirm = MakeUI("TextoConfirmacion", pnBox.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, 70), new Vector2(560, 90), Vector2.one * 0.5f);
        var tmpConf = txtConfirm.AddComponent<TextMeshProUGUI>();
        tmpConf.text = "¿Seguro que quieres borrar todo el progreso y empezar de cero?";
        tmpConf.fontSize = 24;
        tmpConf.color = Color.white;
        tmpConf.alignment = TextAlignmentOptions.Center;
        if (font) tmpConf.font = font;

        var bSi = MakeButton("BotonConfirmarSi", pnBox.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-80, -50), new Vector2(210, 55), Vector2.one * 0.5f,
            new Color(0.6f, 0.18f, 0.18f, 1f), "SÍ, BORRAR", font);
        var bNo = MakeButton("BotonConfirmarNo", pnBox.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(120, -50), new Vector2(210, 55), Vector2.one * 0.5f,
            new Color(0.2f, 0.5f, 0.2f, 1f), "NO, CANCELAR", font);

        panel.SetActive(false);

        // Conectar eventos
        UnityEventTools.AddPersistentListener(bReiniciar.GetComponent<Button>().onClick, mgr.AbrirConfirmacion);
        UnityEventTools.AddPersistentListener(bSi.GetComponent<Button>().onClick, mgr.Reiniciar);
        UnityEventTools.AddPersistentListener(bNo.GetComponent<Button>().onClick, mgr.CerrarConfirmacion);
        mgr.panelConfirmacion = panel;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
        if (!Application.isBatchMode)
            EditorUtility.DisplayDialog("Listo", "Boton 'Rejugar desde 0' agregado a Mapamundial.", "OK");
    }

    // ─────────────────────── FIJAR numeroNivel EN FINNIVEL ───────────────────────

    static void FijarNumeroNivelesFin()
    {
        string[] levels = { "nivel1_colegio", "nivel2_hipodromo", "nivel3_mercado",
                            "nivel4_basurero", "nivel5_subterraneo", "nivel6_empresa" };

        for (int i = 0; i < levels.Length; i++)
        {
            string path = $"Assets/Scenes/{levels[i]}.unity";
            if (!File.Exists(path)) { Debug.LogWarning($"No existe: {path}"); continue; }

            EditorSceneManager.OpenScene(path);
            int numeroEsperado = i + 1;
            int fijados = 0;

            foreach (var fn in Resources.FindObjectsOfTypeAll<FinNivel>())
            {
                if (fn.gameObject.scene != EditorSceneManager.GetActiveScene()) continue;
                fn.numeroNivel = numeroEsperado;
                fijados++;
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
            Debug.Log($"{levels[i]}: FinNivel numeroNivel = {numeroEsperado} ({fijados} fijados)");
        }
    }

    // ─────────────────────── REGISTRAR ESCENAS EN BUILD ───────────────────────

    static void RegistrarEscenasNuevasEnBuild()
    {
        var escenas = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        string[] nuevas = { "Assets/Scenes/escena_final.unity", "Assets/Scenes/creditos.unity" };
        bool cambio = false;

        foreach (var p in nuevas)
        {
            if (!escenas.Exists(e => e.path == p))
            {
                escenas.Add(new EditorBuildSettingsScene(p, true));
                cambio = true;
            }
        }

        if (cambio)
        {
            EditorBuildSettings.scenes = escenas.ToArray();
            Debug.Log("Escenas nuevas registradas en Build Settings.");
        }
    }

    // ─────────────────────── BATCH NUEVAS FUNCIONES ───────────────────────

    public static void RunFinalesBatch()
    {
        ConfigurarEscenaFinal();
        ConfigurarCreditos();
        AgregarBotonRejugarMapamundial();
        AgregarBotonCreditosMenu();
        FijarNumeroNivelesFin();

        Debug.Log("=== ESCENA FINAL, CREDITOS Y REJUGAR CONFIGURADOS ===");
        if (Application.isBatchMode) EditorApplication.Exit(0);
    }

    public static void CopiarLayoutHUD()
    {
        string[] levels = { "nivel1_colegio", "nivel2_hipodromo", "nivel3_mercado",
                            "nivel4_basurero", "nivel5_subterraneo", "nivel6_empresa" };

        // ─── 1. Leer layout de nivel1 ───
        string n1 = $"Assets/Scenes/{levels[0]}.unity";
        EditorSceneManager.OpenScene(n1);

        var canvas1 = Object.FindFirstObjectByType<Canvas>();
        if (canvas1 == null) { Debug.LogError("No hay Canvas en nivel1"); return; }

        var rootData = new Dictionary<string, LayoutData>();
        RecolectarLayout(canvas1.transform, canvas1.name, rootData);

        // ─── 2. Aplicar a niveles 2-6 ───
        for (int i = 1; i < levels.Length; i++)
        {
            string path = $"Assets/Scenes/{levels[i]}.unity";
            EditorSceneManager.OpenScene(path);

            var canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null) { Debug.LogWarning($"No Canvas en {levels[i]}"); continue; }

            int aplicados = 0;
            foreach (var kv in rootData)
            {
                var target = FindByPath(canvas.transform, kv.Key);
                if (target != null)
                {
                    var rt = target.GetComponent<RectTransform>();
                    var d = kv.Value;
                    rt.anchorMin = d.anchorMin;
                    rt.anchorMax = d.anchorMax;
                    rt.anchoredPosition = d.anchoredPosition;
                    rt.sizeDelta = d.sizeDelta;
                    rt.pivot = d.pivot;
                    aplicados++;
                }
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
            Debug.Log($"{levels[i]}: {aplicados} elementos actualizados");
        }

        Debug.Log("=== LAYOUT COPIADO A NIVELES 2-6 ===");
    }

    struct LayoutData
    {
        public Vector2 anchorMin, anchorMax, anchoredPosition, sizeDelta, pivot;
    }

    static void RecolectarLayout(Transform parent, string path, Dictionary<string, LayoutData> data)
    {
        var rt = parent.GetComponent<RectTransform>();
        if (rt != null)
        {
            data[path] = new LayoutData
            {
                anchorMin = rt.anchorMin,
                anchorMax = rt.anchorMax,
                anchoredPosition = rt.anchoredPosition,
                sizeDelta = rt.sizeDelta,
                pivot = rt.pivot
            };
        }
        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            RecolectarLayout(child, path + "/" + child.name, data);
        }
    }

    static Transform FindByPath(Transform root, string path)
    {
        var parts = path.Split('/');
        var current = root;
        for (int i = 1; i < parts.Length; i++)
        {
            var child = current.Find(parts[i]);
            if (child == null) return null;
            current = child;
        }
        return current;
    }

    public static void RunPausaBatch()
    {
        string[] levels = { "nivel2_hipodromo", "nivel3_mercado",
                            "nivel4_basurero", "nivel5_subterraneo", "nivel6_empresa" };
        foreach (var lv in levels) ConfigurarPausa(lv);

        Debug.Log("=== PAUSA AGREGADA A NIVELES RESTANTES ===");
        if (Application.isBatchMode) EditorApplication.Exit(0);
    }
}
