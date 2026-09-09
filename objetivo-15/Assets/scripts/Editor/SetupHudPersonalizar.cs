using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

// Configura el HUD personalizable en todos los niveles:
//  1) Re-genera el prefab TouchControls con HudLayoutManager e instancia en niveles.
//  2) Agrega el boton "PERSONALIZAR HUD" al panel de pausa y el panel editor.
public static class SetupHudPersonalizar
{
    const string BOTON_PAUSA = "BotonPersonalizarHud";
    const string PANEL = "PanelPersonalizarHud";

    static readonly string[] NIVELES =
    {
        "Assets/Scenes/nivel1_colegio.unity",
        "Assets/Scenes/nivel2_hipodromo.unity",
        "Assets/Scenes/nivel3_mercado.unity",
        "Assets/Scenes/nivel4_basurero.unity",
        "Assets/Scenes/nivel5_subterraneo.unity",
        "Assets/Scenes/nivel6_empresa.unity"
    };

    [MenuItem("Objetivo15/Crear HUD Personalizable (todos los niveles)")]
    public static void EjecutarTodoBatch()
    {
        // IMPORTANTE: NO se regenera el prefab TouchControls (eso borraria la
        // customizacion manual). Solo se agrega HudLayoutManager al prefab actual.
        ConfigurarPrefab();

        int ok = 0;
        foreach (string escena in NIVELES)
        {
            if (ConfigurarNivel(escena)) ok++;
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[SetupHudPersonalizar] HUD personalizable en {ok} de {NIVELES.Length} niveles.");
    }

    // Agrega HudLayoutManager al prefab existente SIN regenerarlo, para no perder
    // posiciones/colores/sprites personalizados por el usuario.
    public static void ConfigurarPrefab()
    {
        string path = "Assets/prefabs/TouchControls.prefab";
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null) { Debug.LogError("[SetupHudPersonalizar] No existe " + path); return; }

        if (prefab.GetComponent<HudLayoutManager>() == null)
        {
            prefab.AddComponent<HudLayoutManager>();
            EditorUtility.SetDirty(prefab);
            AssetDatabase.SaveAssets();
            Debug.Log("[SetupHudPersonalizar] HudLayoutManager agregado al prefab (sin regenerar).");
        }
        else
        {
            Debug.Log("[SetupHudPersonalizar] HudLayoutManager ya estaba en el prefab.");
        }
    }

    public static bool ConfigurarNivel(string escena)
    {
        if (!File.Exists(escena)) { Debug.LogWarning($"[SetupHudPersonalizar] No existe: {escena}"); return false; }

        EditorSceneManager.OpenScene(escena, OpenSceneMode.Single);
        var font = FindFont();

        var mp = Object.FindFirstObjectByType<MenuPausa>();
        if (mp == null || mp.panelPausa == null)
        {
            Debug.LogWarning($"[SetupHudPersonalizar] Sin MenuPausa/panelPausa en {escena}; se omite.");
            return false;
        }

        var panelPausa = mp.panelPausa;

        // Limpiar versiones previas
        Limpiar(panelPausa.transform);

        var canvas = panelPausa.transform.root.GetComponent<Canvas>();
        if (canvas == null) canvas = Object.FindFirstObjectByType<Canvas>();

        // ── Boton PERSONALIZAR HUD dentro del panel de pausa ──
        var btn = MakeButton(BOTON_PAUSA, panelPausa.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -230), new Vector2(280, 55), Vector2.one * 0.5f,
            new Color(0.2f, 0.45f, 0.7f, 1f), "PERSONALIZAR HUD", font);
        UnityEventTools.AddPersistentListener(btn.GetComponent<Button>().onClick, mp.AbrirPersonalizarHud);

        // ── Panel editor ──
        var editor = ConstruirPanelEditor(canvas.transform, font);

        mp.panelPersonalizarHud = editor;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), escena);
        Debug.Log($"[SetupHudPersonalizar] HUD personalizable agregado a {escena}");
        return true;
    }

    static void Limpiar(Transform panelPausa)
    {
        var aBorrar = new System.Collections.Generic.List<GameObject>();
        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.scene != EditorSceneManager.GetActiveScene()) continue;
            if (go.name == BOTON_PAUSA || go.name == PANEL)
                aBorrar.Add(go);
        }
        foreach (var go in aBorrar) Object.DestroyImmediate(go);
    }

    static GameObject ConstruirPanelEditor(Transform parent, TMP_FontAsset font)
    {
        var panel = MakeUI(PANEL, parent, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Vector2.one * 0.5f);
        panel.AddComponent<Image>().color = new Color(0, 0, 0, 0.55f);

        var hudPanel = panel.AddComponent<HudPersonalizarPanel>();

        // Titulo
        MakeText("TextoTituloHUD", panel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, 380), new Vector2(600, 60), Vector2.one * 0.5f, "PERSONALIZAR HUD", 44, Color.white, font);

        // Selector de elemento (prev / nombre / next)
        var btnPrev = MakeButton("BtnElementoPrev", panel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-150, 300), new Vector2(70, 55), Vector2.one * 0.5f, new Color(0.3f, 0.3f, 0.3f, 1f), "◀", font);
        var txtElemento = MakeText("TextoElemento", panel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, 300), new Vector2(260, 55), Vector2.one * 0.5f, "Dpad", 26, Color.white, font);
        var btnNext = MakeButton("BtnElementoNext", panel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(150, 300), new Vector2(70, 55), Vector2.one * 0.5f, new Color(0.3f, 0.3f, 0.3f, 1f), "▶", font);

        hudPanel.btnPrev = btnPrev.GetComponent<Button>();
        hudPanel.btnNext = btnNext.GetComponent<Button>();
        hudPanel.textoElemento = txtElemento.GetComponent<TextMeshProUGUI>();

        UnityEventTools.AddPersistentListener(hudPanel.btnPrev.onClick, hudPanel.SeleccionarAnterior);
        UnityEventTools.AddPersistentListener(hudPanel.btnNext.onClick, hudPanel.SeleccionarSiguiente);

        // Cruce de movimiento
        var btnArr = MakeButton("BtnMoverArriba", panel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, 230), new Vector2(90, 60), Vector2.one * 0.5f, new Color(0.3f, 0.3f, 0.3f, 1f), "▲", font);
        var btnAba = MakeButton("BtnMoverAbajo", panel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, 80), new Vector2(90, 60), Vector2.one * 0.5f, new Color(0.3f, 0.3f, 0.3f, 1f), "▼", font);
        var btnIzq = MakeButton("BtnMoverIzquierda", panel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-100, 155), new Vector2(90, 60), Vector2.one * 0.5f, new Color(0.3f, 0.3f, 0.3f, 1f), "◀", font);
        var btnDer = MakeButton("BtnMoverDerecha", panel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(100, 155), new Vector2(90, 60), Vector2.one * 0.5f, new Color(0.3f, 0.3f, 0.3f, 1f), "▶", font);

        hudPanel.btnArriba = btnArr.GetComponent<Button>();
        hudPanel.btnAbajo = btnAba.GetComponent<Button>();
        hudPanel.btnIzquierda = btnIzq.GetComponent<Button>();
        hudPanel.btnDerecha = btnDer.GetComponent<Button>();

        UnityEventTools.AddPersistentListener(hudPanel.btnArriba.onClick, hudPanel.MoverArriba);
        UnityEventTools.AddPersistentListener(hudPanel.btnAbajo.onClick, hudPanel.MoverAbajo);
        UnityEventTools.AddPersistentListener(hudPanel.btnIzquierda.onClick, hudPanel.MoverIzquierda);
        UnityEventTools.AddPersistentListener(hudPanel.btnDerecha.onClick, hudPanel.MoverDerecha);

        // Paleta de colores
        MakeText("TextoTituloColor", panel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, 20), new Vector2(400, 30), Vector2.one * 0.5f, "COLOR", 24, Color.white, font);

        Color[] colores =
        {
            new Color(0.95f, 0.85f, 0.1f, 0.9f), // amarillo
            new Color(0.2f, 0.75f, 0.3f, 0.9f),  // verde
            new Color(1f, 1f, 1f, 0.9f),         // blanco
        };

        for (int i = 0; i < colores.Length; i++)
        {
            float x = (i - 1) * 70; // fila centrada con 3 colores
            var sw = MakeButton("BtnColor" + i, panel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(x, -45), new Vector2(56, 56), Vector2.one * 0.5f, colores[i], "", font);

            var colorBoton = sw.AddComponent<HudColorBoton>();
            colorBoton.panel = hudPanel;
            UnityEventTools.AddPersistentListener(sw.GetComponent<Button>().onClick, colorBoton.Click);
        }

        // Acciones
        var btnReset = MakeButton("BtnRestablecerHUD", panel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -160), new Vector2(280, 55), Vector2.one * 0.5f,
            new Color(0.6f, 0.4f, 0.15f, 1f), "RESTABLECER", font);
        var btnCerrar = MakeButton("BtnCerrarHUD", panel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -240), new Vector2(280, 55), Vector2.one * 0.5f,
            new Color(0.6f, 0.18f, 0.18f, 1f), "VOLVER", font);

        hudPanel.btnRestablecer = btnReset.GetComponent<Button>();
        hudPanel.btnCerrar = btnCerrar.GetComponent<Button>();

        UnityEventTools.AddPersistentListener(hudPanel.btnRestablecer.onClick, hudPanel.Restablecer);
        UnityEventTools.AddPersistentListener(hudPanel.btnCerrar.onClick, hudPanel.Cerrar);

        panel.SetActive(false);
        return panel;
    }

    // ── Helpers de UI ──────────────────────────────────────────────────────

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

    static GameObject MakeText(string name, Transform parent, Vector2 aMin, Vector2 aMax,
        Vector2 pos, Vector2 size, Vector2 pivot, string texto, float fontSize, Color color, TMP_FontAsset font)
    {
        var go = MakeUI(name, parent, aMin, aMax, pos, size, pivot);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = texto;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        if (font != null) tmp.font = font;
        return go;
    }

    static GameObject MakeButton(string name, Transform parent, Vector2 aMin, Vector2 aMax,
        Vector2 pos, Vector2 size, Vector2 pivot, Color color, string label, TMP_FontAsset font)
    {
        var go = MakeUI(name, parent, aMin, aMax, pos, size, pivot);
        go.AddComponent<Image>().color = color;
        go.AddComponent<Button>();

        var txt = MakeText("Text (TMP)", go.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
            Vector2.one * 0.5f, label, 22, Color.white, font);
        txt.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        txt.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        return go;
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
}