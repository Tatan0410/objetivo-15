using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Animations;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.IO;

public static class SetupBalas
{
    const string CONTROLLER_PATH = "Assets/newanimation/jugador 3.controller";

    static readonly string[] ESCENAS =
    {
        "Assets/Scenes/nivel1_colegio.unity",
        "Assets/Scenes/nivel2_hipodromo.unity",
        "Assets/Scenes/nivel3_mercado.unity",
        "Assets/Scenes/nivel4_basurero.unity",
        "Assets/Scenes/nivel5_subterraneo.unity",
        "Assets/Scenes/nivel6_empresa.unity",
        "Assets/Scenes/menuprincipal.unity"
    };

    public static void AgregarTriggersAnimator()
    {
        AnimatorController ctrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(CONTROLLER_PATH);
        if (ctrl == null)
        {
            Debug.LogError("No se encontro el controller: " + CONTROLLER_PATH);
            return;
        }

        bool dirty = false;
        if (ctrl.parameters.All(p => p.name != "Disparar"))
        {
            ctrl.AddParameter("Disparar", AnimatorControllerParameterType.Trigger);
            dirty = true;
        }
        if (ctrl.parameters.All(p => p.name != "Craftear"))
        {
            ctrl.AddParameter("Craftear", AnimatorControllerParameterType.Trigger);
            dirty = true;
        }

        EditorUtility.SetDirty(ctrl);
        AssetDatabase.SaveAssets();
        Debug.Log(dirty
            ? "Triggers 'Disparar' y 'Craftear' agregados al controller del jugador."
            : "Los triggers 'Disparar' y 'Craftear' ya existian.");
    }

    public static void ConfigurarBalasEnEscenas()
    {
        int ok = 0;
        foreach (string escena in ESCENAS)
        {
            EditorSceneManager.OpenScene(escena, OpenSceneMode.Single);
            bool configurada = ConfigurarEscena();
            EditorSceneManager.SaveOpenScenes();
            if (configurada) ok++;
            Debug.Log($"[SetupBalas] {escena}: {(configurada ? "OK" : "sin cambios")}");
        }
        AssetDatabase.Refresh();
        Debug.Log($"[SetupBalas] HUD configurado en {ok} de {ESCENAS.Length} escenas.");
    }

    public static void HacerTodo()
    {
        AgregarTriggersAnimator();
        ConfigurarBalasEnEscenas();
    }

    public static void EjecutarTodoBatch()
    {
        HacerTodo();
    }

    public static void EliminarContMunicionObsoleto()
    {
        int eliminados = 0;
        foreach (string escena in ESCENAS)
        {
            EditorSceneManager.OpenScene(escena, OpenSceneMode.Single);
            bool hecho = false;

            var cont = GameObject.Find("ContMunicion");
            if (cont != null)
            {
                Object.DestroyImmediate(cont);
                hecho = true;
            }

            if (hecho)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), escena);
                eliminados++;
            }
            Debug.Log($"[SetupBalas] {escena}: {(hecho ? "ContMunicion obsoleto eliminado" : "sin cambios")}");
        }
        AssetDatabase.Refresh();
        Debug.Log($"[SetupBalas] ContMunicion obsoleto eliminado en {eliminados} de {ESCENAS.Length} escenas.");
    }

    public static void EjecutarEliminarContMunicionBatch()
    {
        EliminarContMunicionObsoleto();
    }

    public static void AsignarSpritesPanelBalas()
    {
        const string prefabPath = "Assets/prefabs/ContenedorBalas.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError("[SetupBalas] No existe ContenedorBalas.prefab");
            return;
        }

        PanelBalasHUD panel = prefab.GetComponent<PanelBalasHUD>();
        if (panel == null)
        {
            Debug.LogError("[SetupBalas] ContenedorBalas.prefab no tiene PanelBalasHUD");
            return;
        }

        if (panel.contadorComun != null && panel.contadorComun.icono != null)
            panel.spriteComun = panel.contadorComun.icono.sprite;
        if (panel.contadorEpica != null && panel.contadorEpica.icono != null)
            panel.spriteEpica = panel.contadorEpica.icono.sprite;
        if (panel.contadorLegendaria != null && panel.contadorLegendaria.icono != null)
            panel.spriteLegendaria = panel.contadorLegendaria.icono.sprite;

        EditorUtility.SetDirty(prefab);
        AssetDatabase.SaveAssets();
        Debug.Log("[SetupBalas] Sprites del tipo seleccionado asignados desde las filas del prefab.");
    }

    public static void EjecutarAsignarSpritesBatch()
    {
        AsignarSpritesPanelBalas();
    }

    public static void EjecutarRepararHUDMunicionBatch()
    {
        AsignarSpritesPanelBalas();
        EliminarContMunicionObsoleto();
    }

    public static void CrearContMunicionPistola()
    {
        const string spritePath = "Assets/sprites/pitolaloco-removebg-preview.png";
        Sprite spritePistola = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        TMP_FontAsset font = EncontrarFuente();
        int ok = 0;

        foreach (string escena in ESCENAS)
        {
            EditorSceneManager.OpenScene(escena, OpenSceneMode.Single);
            bool hecho = false;

            var res = GameObject.Find("ContenedorRecursos");
            if (res != null)
            {
                var viejo = res.transform.Find("ContMunicion");
                if (viejo != null) Object.DestroyImmediate(viejo.gameObject);

                GameObject fila = new GameObject("ContMunicion");
                fila.transform.SetParent(res.transform, false);
                RectTransform rt = fila.AddComponent<RectTransform>();
                rt.anchorMin = new Vector2(0f, 1f);
                rt.anchorMax = new Vector2(0f, 1f);
                rt.pivot = new Vector2(0f, 1f);
                rt.anchoredPosition = new Vector2(0, -84);
                rt.sizeDelta = new Vector2(200, 24);

                Image icono = new GameObject("Icono").AddComponent<Image>();
                icono.transform.SetParent(fila.transform, false);
                RectTransform iconRt = (RectTransform)icono.transform;
                iconRt.anchorMin = new Vector2(0f, 0.5f);
                iconRt.anchorMax = new Vector2(0f, 0.5f);
                iconRt.pivot = new Vector2(0.5f, 0.5f);
                iconRt.anchoredPosition = Vector2.zero;
                iconRt.sizeDelta = new Vector2(80, 80);
                icono.sprite = spritePistola;
                icono.preserveAspect = true;

                GameObject txtGO = new GameObject("Numero");
                txtGO.transform.SetParent(fila.transform, false);
                RectTransform txtRt = txtGO.AddComponent<RectTransform>();
                txtRt.anchorMin = new Vector2(0f, 0.5f);
                txtRt.anchorMax = new Vector2(0f, 0.5f);
                txtRt.pivot = new Vector2(0f, 0.5f);
                txtRt.anchoredPosition = new Vector2(26, 0);
                txtRt.sizeDelta = new Vector2(80, 24);
                TextMeshProUGUI tmp = txtGO.AddComponent<TextMeshProUGUI>();
                tmp.text = "0";
                tmp.fontSize = 18;
                tmp.color = Color.white;
                tmp.alignment = TextAlignmentOptions.Left;
                if (font != null) tmp.font = font;

                ContadorHUD cont = fila.AddComponent<ContadorHUD>();
                cont.icono = icono;
                cont.texto = tmp;
                cont.Actualizar(0);
                fila.AddComponent<ContadorMunicionSeleccionado>();

                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), escena);
                hecho = true;
                ok++;
            }
            Debug.Log($"[SetupBalas] {escena}: {(hecho ? "pistolita ContMunicion creada" : "sin ContenedorRecursos")}");
        }
        AssetDatabase.Refresh();
        Debug.Log($"[SetupBalas] Pistolita ContMunicion creada en {ok} escenas.");
    }

    public static void EjecutarCrearContMunicionPistolaBatch()
    {
        CrearContMunicionPistola();
    }

    public static Vector2 posicionTxtBala = new Vector2(0f, 0f);
    public static float tamanoTxtBala = 24f;
    public static int grosorTxtBala = 400;

    public static void CargarValoresTxtBala()
    {
        const string prefabPath = "Assets/prefabs/ContenedorBalas.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null) return;
        foreach (var t in prefab.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (t.gameObject.name == "TxtBalaSeleccionada")
            {
                posicionTxtBala = t.rectTransform.anchoredPosition;
                tamanoTxtBala = t.fontSize;
                grosorTxtBala = (int)t.fontWeight;
                return;
            }
        }
    }

    public static void AjustarTxtBalaSeleccionada()
    {
        const string prefabPath = "Assets/prefabs/ContenedorBalas.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError("[SetupBalas] No existe ContenedorBalas.prefab");
            return;
        }

        TextMeshProUGUI txt = null;
        foreach (var t in prefab.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (t.gameObject.name == "TxtBalaSeleccionada")
            {
                txt = t;
                break;
            }
        }
        if (txt == null)
        {
            Debug.LogError("[SetupBalas] No se encontro TxtBalaSeleccionada en el prefab");
            return;
        }

        RectTransform rt = (RectTransform)txt.transform;
        rt.anchoredPosition = posicionTxtBala;
        txt.fontSize = tamanoTxtBala;
        txt.fontWeight = (FontWeight)grosorTxtBala;

        LayoutElement le = txt.GetComponent<LayoutElement>();
        if (le == null) le = txt.gameObject.AddComponent<LayoutElement>();
        le.ignoreLayout = true;

        EditorUtility.SetDirty(prefab);
        AssetDatabase.SaveAssets();
        Debug.Log($"[SetupBalas] TxtBalaSeleccionada ajustada: pos={posicionTxtBala}, tamano={tamanoTxtBala}, grosor={grosorTxtBala}");
    }

    public static void EjecutarAjustarTxtBalaSeleccionadaBatch()
    {
        AjustarTxtBalaSeleccionada();
    }

    public static void RenombrarPrefabContenedorBalas()
    {
        const string prefabPathViejo = "Assets/prefabs/ContenedorBalas 1.prefab";
        const string prefabPathNuevo = "Assets/prefabs/ContenedorBalas.prefab";

        if (File.Exists(prefabPathViejo))
        {
            string error = AssetDatabase.RenameAsset(prefabPathViejo, "ContenedorBalas");
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError("[SetupBalas] No se pudo renombrar el prefab: " + error);
                return;
            }
            AssetDatabase.Refresh();
            Debug.Log("[SetupBalas] Asset renombrado a ContenedorBalas.prefab");
        }
        else if (!File.Exists(prefabPathNuevo))
        {
            Debug.LogError("[SetupBalas] No existe ContenedorBalas.prefab ni ContenedorBalas 1.prefab");
            return;
        }

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPathNuevo);
        if (prefab == null)
        {
            Debug.LogError("[SetupBalas] No se pudo cargar ContenedorBalas.prefab");
            return;
        }

        if (prefab.name != "ContenedorBalas")
        {
            prefab.name = "ContenedorBalas";
            EditorUtility.SetDirty(prefab);
        }

        EditorUtility.SetDirty(prefab);
        AssetDatabase.SaveAssets();
        Debug.Log($"[SetupBalas] Prefab raiz '{prefab.name}' listo en {prefabPathNuevo}");
    }

    public static void EjecutarRenombrarPrefabBatch()
    {
        RenombrarPrefabContenedorBalas();
    }

    public static void VerificarHUDMunicion()
    {
        foreach (string escena in ESCENAS)
        {
            EditorSceneManager.OpenScene(escena, OpenSceneMode.Single);
            var panel = Object.FindFirstObjectByType<PanelBalasHUD>();
            if (panel == null)
            {
                Debug.LogError($"[SetupBalas] {escena}: NO se encontro PanelBalasHUD (ContenedorBalas)");
                continue;
            }
            var tmp = panel.textoSeleccionado;
            if (tmp == null)
            {
                Debug.LogError($"[SetupBalas] {escena}: PanelBalasHUD sin textoSeleccionado");
                continue;
            }
            Debug.Log($"[SetupBalas] {escena}: OK texto pos={tmp.rectTransform.anchoredPosition} size={tmp.fontSize} peso={(int)tmp.fontWeight} texto='{tmp.text}'");
        }
        AssetDatabase.Refresh();
        Debug.Log("[SetupBalas] Verificacion HUD municion completada.");
    }

    public static void EjecutarVerificarHUDMunicionBatch()
    {
        VerificarHUDMunicion();
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

    static bool ConfigurarEscena()
    {
        var mm = Object.FindFirstObjectByType<MunicionManager>();
        if (mm == null)
        {
            Debug.LogWarning("No hay MunicionManager en la escena; se omiten los contadores.");
        }

        GameObject canvasGO = GameObject.Find("Canvas");
        if (canvasGO == null)
        {
            var canvas = Object.FindFirstObjectByType<Canvas>();
            canvasGO = canvas != null ? canvas.gameObject : null;
        }
        if (canvasGO == null) return false;

        // Crear/actualizar contenedor de balas
        Transform contenedor = BuscarEn(canvasGO.transform, "ContenedorBalas");
        if (contenedor == null)
        {
            GameObject contGO = new GameObject("ContenedorBalas");
            contGO.transform.SetParent(canvasGO.transform, false);
            contGO.AddComponent<RectTransform>();

            VerticalLayoutGroup vlg = contGO.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 4;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = false;
            vlg.childForceExpandHeight = false;

            ContentSizeFitter csf = contGO.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var imagenFondo = contGO.AddComponent<Image>();
            imagenFondo.color = new Color(0, 0, 0, 0.6f);

            contenedor = contGO.transform;

            RectTransform rt = (RectTransform)contenedor;
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(10, -10);
            rt.sizeDelta = new Vector2(190, 150);
        }
        else
        {
            var vlg = contenedor.GetComponent<VerticalLayoutGroup>();
            if (vlg == null)
            {
                vlg = contenedor.gameObject.AddComponent<VerticalLayoutGroup>();
                vlg.spacing = 4;
            }
        }

        // Limpiar contenido previo del contenedor para reconstruirlo (idempotente)
        var hijosViejos = new System.Collections.Generic.List<Transform>();
        foreach (Transform child in contenedor)
            hijosViejos.Add(child);
        foreach (var child in hijosViejos)
            Object.DestroyImmediate(child.gameObject);

        // Etiqueta del tipo seleccionado
        TextMeshProUGUI tmpSeleccionado = CrearTMP("TxtBalaSeleccionada", contenedor, "COMUN", 24, Color.white);
        tmpSeleccionado.alignment = TextAlignmentOptions.Center;

        // Icono del tipo seleccionado
        Image iconoSeleccionado = CrearImagen("IconBalaSeleccionada", contenedor, Color.white);
        iconoSeleccionado.rectTransform.sizeDelta = new Vector2(28, 28);
        iconoSeleccionado.rectTransform.localScale = Vector3.one;

        // 3 filas de contadores
        ContadorHUD cComun = CrearFilaContador(contenedor, "ContMunComun");
        ContadorHUD cEpica = CrearFilaContador(contenedor, "ContMunEpica");
        ContadorHUD cLegendaria = CrearFilaContador(contenedor, "ContMunLegendaria");

        // Boton de cambiar bala
        Button boton = CrearBoton(contenedor, "BtnCambiarBala", "Cambiar");

        // PanelBalasHUD que cablea todo
        PanelBalasHUD panel = contenedor.gameObject.GetComponent<PanelBalasHUD>();
        if (panel == null)
            panel = contenedor.gameObject.AddComponent<PanelBalasHUD>();

        panel.contadorComun = cComun;
        panel.contadorEpica = cEpica;
        panel.contadorLegendaria = cLegendaria;
        panel.textoSeleccionado = tmpSeleccionado;
        panel.iconoSeleccionado = iconoSeleccionado;
        panel.botonCambiar = boton;

        EditorUtility.SetDirty(contenedor.gameObject);
        return true;
    }

    static ContadorHUD CrearFilaContador(Transform parent, string nombre)
    {
        GameObject fila = new GameObject(nombre);
        fila.transform.SetParent(parent, false);
        RectTransform rt = fila.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(120, 24);

        HorizontalLayoutGroup hlg = fila.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 6;
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childControlHeight = false;

        ContadorHUD cont = fila.AddComponent<ContadorHUD>();

        Image icono = CrearImagen("Icono", fila.transform, Color.white);
        icono.rectTransform.sizeDelta = new Vector2(20, 20);

        TextMeshProUGUI txt = CrearTMP("Texto", fila.transform, "0", 22, Color.white);

        cont.icono = icono;
        cont.texto = txt;
        cont.Actualizar(0);
        return cont;
    }

    static Button CrearBoton(Transform parent, string nombre, string texto)
    {
        GameObject btnGO = new GameObject(nombre);
        btnGO.transform.SetParent(parent, false);
        RectTransform rt = btnGO.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(120, 30);

        Image img = btnGO.AddComponent<Image>();
        img.color = new Color(0.2f, 0.5f, 0.9f, 1f);

        Button btn = btnGO.AddComponent<Button>();
        btn.targetGraphic = img;

        TextMeshProUGUI txt = CrearTMP("Texto", btnGO.transform, texto, 20, Color.white);
        txt.alignment = TextAlignmentOptions.Center;
        return btn;
    }

    static TextMeshProUGUI CrearTMP(string nombre, Transform parent, string texto, float size, Color color)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = texto;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        tmp.raycastTarget = false;
        return tmp;
    }

    static Image CrearImagen(string nombre, Transform parent, Color color)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        Image img = go.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
        return img;
    }

    static Transform BuscarEn(Transform parent, string nombre)
    {
        if (parent == null) return null;
        Transform t = parent.Find(nombre);
        if (t != null) return t;
        foreach (Transform child in parent)
        {
            t = BuscarEn(child, nombre);
            if (t != null) return t;
        }
        return null;
    }
}
