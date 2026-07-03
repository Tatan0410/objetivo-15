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

        if (GUILayout.Button("6. Limpiar power-ups directos de nivel1_colegio", GUILayout.Height(40)))
            LimpiarPotenciadoresDirectos();

        EditorGUILayout.Space();

        if (GUILayout.Button("7. Ajustar posicion Y de enemigos en nivel1_colegio", GUILayout.Height(40)))
            AjustarEnemigos();

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
            nav.musicaFondo = musicGO.GetComponent<AudioSource>();

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
        ConfigurarCutsceneSoluciones();
        ConfigurarMapamundial();

        string[] levels = { "nivel1_colegio", "nivel2_hipodromo", "nivel3_mercado",
                            "nivel4_basurero", "nivel5_subterraneo", "nivel6_empresa" };
        foreach (var lv in levels) ConfigurarPausa(lv);

        LimpiarPotenciadoresDirectos();
        AjustarEnemigos();

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
