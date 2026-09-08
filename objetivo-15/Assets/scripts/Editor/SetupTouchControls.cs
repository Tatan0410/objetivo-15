using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.IO;

public static class SetupTouchControls
{
    const string PREFAB_PATH = "Assets/prefabs/TouchControls.prefab";

    static readonly string[] NIVELES =
    {
        "Assets/Scenes/nivel1_colegio.unity",
        "Assets/Scenes/nivel2_hipodromo.unity",
        "Assets/Scenes/nivel3_mercado.unity",
        "Assets/Scenes/nivel4_basurero.unity",
        "Assets/Scenes/nivel5_subterraneo.unity",
        "Assets/Scenes/nivel6_empresa.unity"
    };

    [MenuItem("Objetivo15/Crear Prefab Controles Tactiles")]
    public static void CrearPrefab()
    {
        CrearOActualizarPrefab();
    }

    public static void CrearOActualizarPrefab()
    {
        GameObject prefab = CrearPrefabEnMemoria();
        PrefabUtility.SaveAsPrefabAsset(prefab, PREFAB_PATH);
        Object.DestroyImmediate(prefab);
        AssetDatabase.SaveAssets();
        Debug.Log("[SetupTouchControls] Prefab actualizado: " + PREFAB_PATH);
    }

    public static void InstanciarEnNiveles()
    {
        int ok = 0;
        foreach (string escena in NIVELES)
        {
            EditorSceneManager.OpenScene(escena, OpenSceneMode.Single);
            bool hecho = false;

            // Eliminar instancias viejas del prefab para regenerarlas con el wiring nuevo
            var viejos = new System.Collections.Generic.List<GameObject>();
            foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            {
                if (go.name == "TouchControls")
                    viejos.Add(go);
            }
            foreach (var go in viejos)
                Object.DestroyImmediate(go);

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH);
            if (prefab != null)
            {
                PrefabUtility.InstantiatePrefab(prefab);
                hecho = true;
            }

            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                new GameObject("EventSystem",
                    typeof(EventSystem),
                    typeof(StandaloneInputModule));
                SeleccionUI.AsegurarEventSystem();
                hecho = true;
            }

            if (hecho)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), escena);
                ok++;
            }
            Debug.Log($"[SetupTouchControls] {escena}: {(hecho ? "controles + EventSystem OK" : "sin cambios")}");
        }
        AssetDatabase.Refresh();
        Debug.Log($"[SetupTouchControls] Controles instanciados en {ok} de {NIVELES.Length} escenas.");
    }

    public static void EjecutarTodoBatch()
    {
        CrearOActualizarPrefab();
        InstanciarEnNiveles();
    }

    public static void EjecutarTodoConSafeArea()
    {
        EjecutarTodoBatch();
        SetupSafeArea.EjecutarTodoBatch();
    }

    static GameObject CrearPrefabEnMemoria()
    {
        GameObject root = new GameObject("TouchControls");

        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        root.AddComponent<GraphicRaycaster>();

        ControlTouch controlTouch = root.AddComponent<ControlTouch>();

        TMP_FontAsset font = EncontrarFuente();

        // ── D-pad izquierdo ─────────────────────────────
        GameObject dpad = CrearContenedor(root.transform, "Dpad", new Vector2(-700, -320), new Vector2(360, 200));
        GameObject btnIzq = CrearBotonTactil(dpad.transform, "BtnIzquierda", "◀", new Vector2(-100, 0), new Vector2(140, 140), font);
        GameObject btnDer = CrearBotonTactil(dpad.transform, "BtnDerecha", "▶", new Vector2(100, 0), new Vector2(140, 140), font);

        btnIzq.GetComponent<TouchButton>().accion = TouchButton.TipoAccion.Izquierda;
        btnDer.GetComponent<TouchButton>().accion = TouchButton.TipoAccion.Derecha;

        // ── Botones derecho ─────────────────────────────
        GameObject cluster = CrearContenedor(root.transform, "Botones", new Vector2(720, -320), new Vector2(420, 320));

        GameObject btnSaltar = CrearBotonTactil(cluster.transform, "BtnSaltar", "SALTAR", new Vector2(130, 100), new Vector2(150, 130), font);
        GameObject btnAtaque = CrearBotonTactil(cluster.transform, "BtnAtaque", "ATACAR", new Vector2(-130, 100), new Vector2(150, 130), font);
        GameObject btnDisparo = CrearBotonTactil(cluster.transform, "BtnDisparo", "DISPARAR", new Vector2(130, -110), new Vector2(150, 130), font);
        GameObject btnBala = CrearBotonTactil(cluster.transform, "BtnCambioBala", "BALA", new Vector2(-130, -110), new Vector2(150, 130), font);

        btnSaltar.GetComponent<TouchButton>().accion = TouchButton.TipoAccion.Saltar;
        btnAtaque.GetComponent<TouchButton>().accion = TouchButton.TipoAccion.Ataque;
        btnDisparo.GetComponent<TouchButton>().accion = TouchButton.TipoAccion.Disparo;
        btnBala.GetComponent<TouchButton>().accion = TouchButton.TipoAccion.CambioBala;

        // Disparo y cambio de bala arrancan ocultos (solo al tener arma)
        btnDisparo.SetActive(false);
        btnBala.SetActive(false);
        controlTouch.botonDisparo = btnDisparo;
        controlTouch.botonCambioBala = btnBala;

        return root;
    }

    static GameObject CrearContenedor(Transform parent, string nombre, Vector2 pos, Vector2 tamano)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = tamano;
        return go;
    }

    static GameObject CrearBotonTactil(Transform parent, string nombre, string etiqueta, Vector2 pos, Vector2 tamano, TMP_FontAsset font)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = tamano;

        Image img = go.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.15f);
        img.raycastTarget = true;

        go.AddComponent<TouchButton>();

        GameObject label = new GameObject("Etiqueta");
        label.transform.SetParent(go.transform, false);
        RectTransform lrt = label.AddComponent<RectTransform>();
        lrt.anchorMin = lrt.anchorMax = new Vector2(0.5f, 0.5f);
        lrt.pivot = new Vector2(0.5f, 0.5f);
        lrt.anchoredPosition = Vector2.zero;
        lrt.sizeDelta = tamano;
        TextMeshProUGUI tmp = label.AddComponent<TextMeshProUGUI>();
        tmp.text = etiqueta;
        tmp.fontSize = 30;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.raycastTarget = false;
        if (font != null) tmp.font = font;

        return go;
    }

    static TMP_FontAsset EncontrarFuente()
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