using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Animations;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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

        // Conectar MunicionManager.contadorMunicion -> ContMunicion existente
        if (mm != null)
        {
            var contMunicion = BuscarContadorHUD(canvasGO.transform, "ContMunicion");
            if (contMunicion != null && mm.contadorMunicion != contMunicion)
            {
                mm.contadorMunicion = contMunicion;
                EditorUtility.SetDirty(mm);
            }
        }

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

    static ContadorHUD BuscarContadorHUD(Transform root, string nombre)
    {
        Transform t = BuscarEn(root, nombre);
        return t != null ? t.GetComponent<ContadorHUD>() : null;
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
