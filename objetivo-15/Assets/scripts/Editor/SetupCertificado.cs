using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.IO;

public static class SetupCertificado
{
    [MenuItem("Objetivo15/Crear Escena Certificado")]
    public static void CrearEscena()
    {
        string path = "Assets/Scenes/certificado.unity";
        // Crear escena vacía
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Cámara
        var camGO = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        camGO.tag = "MainCamera";
        camGO.transform.position = new Vector3(0, 0, -10);

        // Luz
        var lightGO = new GameObject("Directional Light", typeof(Light));
        lightGO.GetComponent<Light>().type = LightType.Directional;
        lightGO.transform.rotation = Quaternion.Euler(50, -30, 0);

        // EventSystem
        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        // Canvas
        var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        TMP_FontAsset fuente = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/fonts/PressStart2P-Regular SDF.asset");
        if (fuente == null)
            fuente = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Resources/Transicion/fuente.asset");
        if (fuente == null)
        {
            string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
            if (guids.Length > 0)
                fuente = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        // Fondo
        var fondoGO = new GameObject("FondoCertificado", typeof(RectTransform));
        fondoGO.transform.SetParent(canvasGO.transform, false);
        var fondoRT = fondoGO.GetComponent<RectTransform>();
        fondoRT.anchorMin = Vector2.zero; fondoRT.anchorMax = Vector2.one; fondoRT.offsetMin = Vector2.zero; fondoRT.offsetMax = Vector2.zero;
        var fondoImg = fondoGO.AddComponent<Image>();
        fondoImg.color = new Color(0.98f, 0.96f, 0.88f, 1f); // pergamino
        fondoImg.raycastTarget = false;
        fondoGO.transform.SetAsFirstSibling();

        // Título
        var tituloGO = new GameObject("TituloCertificado", typeof(RectTransform));
        tituloGO.transform.SetParent(canvasGO.transform, false);
        var tituloRT = tituloGO.GetComponent<RectTransform>();
        tituloRT.anchorMin = new Vector2(0.5f, 0.85f); tituloRT.anchorMax = new Vector2(0.5f, 0.85f); tituloRT.sizeDelta = new Vector2(1000, 100);
        tituloRT.anchoredPosition = Vector2.zero;
        var tituloTMP = tituloGO.AddComponent<TextMeshProUGUI>();
        tituloTMP.text = "CERTIFICADO DE IMPACTO AMBIENTAL"; tituloTMP.fontSize = 42; tituloTMP.color = new Color(0.15f, 0.35f, 0.15f, 1f);
        tituloTMP.alignment = TextAlignmentOptions.Center; tituloTMP.fontStyle = FontStyles.Bold;
        if (fuente != null) tituloTMP.font = fuente;

        // Nombre jugador
        var nombreGO = new GameObject("TextoNombreJugador", typeof(RectTransform));
        nombreGO.transform.SetParent(canvasGO.transform, false);
        var nombreRT = nombreGO.GetComponent<RectTransform>();
        nombreRT.anchorMin = new Vector2(0.5f, 0.7f); nombreRT.anchorMax = new Vector2(0.5f, 0.7f); nombreRT.sizeDelta = new Vector2(900, 80);
        nombreRT.anchoredPosition = Vector2.zero;
        var nombreTMP = nombreGO.AddComponent<TextMeshProUGUI>();
        nombreTMP.text = "Jugador"; nombreTMP.fontSize = 48; nombreTMP.color = new Color(0.2f, 0.2f, 0.6f, 1f);
        nombreTMP.alignment = TextAlignmentOptions.Center; nombreTMP.fontStyle = FontStyles.Bold;
        if (fuente != null) nombreTMP.font = fuente;

        // Panel estadísticas
        var panelGO = new GameObject("PanelEstadisticas", typeof(RectTransform));
        panelGO.transform.SetParent(canvasGO.transform, false);
        var panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0.5f, 0.5f); panelRT.anchorMax = new Vector2(0.5f, 0.5f); panelRT.sizeDelta = new Vector2(800, 200);
        panelRT.anchoredPosition = new Vector2(0, -20);
        var panelImg = panelGO.AddComponent<Image>(); panelImg.color = new Color(1f, 1f, 1f, 0.5f);
        var layout = panelGO.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 20; layout.childAlignment = TextAnchor.MiddleCenter; layout.childForceExpandWidth = true;

        TMP_Text txtPlasticos = null, txtEnemigos = null, txtTiempo = null;
        string[] labels = { "plásticos reciclados", "amenazas derrotadas", "tiempo de juego" };
        string[] nombres = { "FilaPlasticos", "FilaEnemigos", "FilaTiempo" };
        for (int i = 0; i < 3; i++)
        {
            var filaGO = new GameObject(nombres[i], typeof(RectTransform));
            filaGO.transform.SetParent(panelGO.transform, false);
            var filaRT = filaGO.GetComponent<RectTransform>();
            filaRT.sizeDelta = new Vector2(240, 180);
            var filaLayout = filaGO.AddComponent<VerticalLayoutGroup>();
            filaLayout.childAlignment = TextAnchor.MiddleCenter; filaLayout.spacing = 5;
            var valGO = new GameObject("Valor", typeof(RectTransform));
            valGO.transform.SetParent(filaGO.transform, false);
            var valRT = valGO.GetComponent<RectTransform>(); valRT.sizeDelta = new Vector2(200, 60);
            var valTMP = valGO.AddComponent<TextMeshProUGUI>();
            valTMP.text = "0"; valTMP.fontSize = 48; valTMP.color = new Color(0.15f, 0.35f, 0.15f, 1f);
            valTMP.alignment = TextAlignmentOptions.Center; valTMP.fontStyle = FontStyles.Bold;
            if (fuente != null) valTMP.font = fuente;
            var labelGO = new GameObject("Label", typeof(RectTransform));
            labelGO.transform.SetParent(filaGO.transform, false);
            var labelRT = labelGO.GetComponent<RectTransform>(); labelRT.sizeDelta = new Vector2(200, 40);
            var labelTMP = labelGO.AddComponent<TextMeshProUGUI>();
            labelTMP.text = labels[i]; labelTMP.fontSize = 18; labelTMP.color = Color.black;
            labelTMP.alignment = TextAlignmentOptions.Center;
            if (fuente != null) labelTMP.font = fuente;
            if (i == 0) txtPlasticos = valTMP;
            else if (i == 1) txtEnemigos = valTMP;
            else txtTiempo = valTMP;
        }

        // Botón Descargar
        var btnDescGO = new GameObject("BotonDescargar", typeof(RectTransform));
        btnDescGO.transform.SetParent(canvasGO.transform, false);
        var btnDescRT = btnDescGO.GetComponent<RectTransform>();
        btnDescRT.anchorMin = new Vector2(0.5f, 0.25f); btnDescRT.anchorMax = new Vector2(0.5f, 0.25f); btnDescRT.sizeDelta = new Vector2(350, 60);
        btnDescRT.anchoredPosition = new Vector2(0, 0);
        var btnDescImg = btnDescGO.AddComponent<Image>(); btnDescImg.color = new Color(0.2f, 0.6f, 0.3f, 1f);
        var btnDesc = btnDescGO.AddComponent<Button>(); btnDesc.targetGraphic = btnDescImg;
        var btnDescTxtGO = new GameObject("Text", typeof(RectTransform));
        btnDescTxtGO.transform.SetParent(btnDescGO.transform, false);
        var btnDescTxtRT = btnDescTxtGO.GetComponent<RectTransform>(); btnDescTxtRT.anchorMin = Vector2.zero; btnDescTxtRT.anchorMax = Vector2.one; btnDescTxtRT.offsetMin = Vector2.zero; btnDescTxtRT.offsetMax = Vector2.zero;
        var btnDescTMP = btnDescTxtGO.AddComponent<TextMeshProUGUI>();
        btnDescTMP.text = "📥 Descargar Certificado"; btnDescTMP.fontSize = 22; btnDescTMP.color = Color.white; btnDescTMP.alignment = TextAlignmentOptions.Center;
        if (fuente != null) btnDescTMP.font = fuente;

        // Panel confirmación
        var panelConfGO = new GameObject("PanelConfirmacion", typeof(RectTransform));
        panelConfGO.transform.SetParent(canvasGO.transform, false);
        var panelConfRT = panelConfGO.GetComponent<RectTransform>();
        panelConfRT.anchorMin = new Vector2(0.5f, 0.15f); panelConfRT.anchorMax = new Vector2(0.5f, 0.15f); panelConfRT.sizeDelta = new Vector2(700, 50);
        panelConfRT.anchoredPosition = Vector2.zero;
        var panelConfImg = panelConfGO.AddComponent<Image>(); panelConfImg.color = new Color(0.15f, 0.35f, 0.15f, 0.9f);
        var txtConfGO = new GameObject("TextoConfirmacion", typeof(RectTransform));
        txtConfGO.transform.SetParent(panelConfGO.transform, false);
        var txtConfRT = txtConfGO.GetComponent<RectTransform>(); txtConfRT.anchorMin = Vector2.zero; txtConfRT.anchorMax = Vector2.one; txtConfRT.offsetMin = new Vector2(10, 5); txtConfRT.offsetMax = new Vector2(-10, -5);
        var txtConfTMP = txtConfGO.AddComponent<TextMeshProUGUI>();
        txtConfTMP.text = ""; txtConfTMP.fontSize = 16; txtConfTMP.color = Color.white; txtConfTMP.alignment = TextAlignmentOptions.Center;
        if (fuente != null) txtConfTMP.font = fuente;
        panelConfGO.SetActive(false);

        // Botón Continuar
        var btnContGO = new GameObject("BotonContinuar", typeof(RectTransform));
        btnContGO.transform.SetParent(canvasGO.transform, false);
        var btnContRT = btnContGO.GetComponent<RectTransform>();
        btnContRT.anchorMin = new Vector2(0.5f, 0.08f); btnContRT.anchorMax = new Vector2(0.5f, 0.08f); btnContRT.sizeDelta = new Vector2(250, 50);
        btnContRT.anchoredPosition = Vector2.zero;
        var btnContImg = btnContGO.AddComponent<Image>(); btnContImg.color = new Color(0.2f, 0.4f, 0.6f, 1f);
        var btnCont = btnContGO.AddComponent<Button>(); btnCont.targetGraphic = btnContImg;
        var btnContTxtGO = new GameObject("Text", typeof(RectTransform));
        btnContTxtGO.transform.SetParent(btnContGO.transform, false);
        var btnContTxtRT = btnContTxtGO.GetComponent<RectTransform>(); btnContTxtRT.anchorMin = Vector2.zero; btnContTxtRT.anchorMax = Vector2.one; btnContTxtRT.offsetMin = Vector2.zero; btnContTxtRT.offsetMax = Vector2.zero;
        var btnContTMP = btnContTxtGO.AddComponent<TextMeshProUGUI>();
        btnContTMP.text = "Continuar →"; btnContTMP.fontSize = 22; btnContTMP.color = Color.white; btnContTMP.alignment = TextAlignmentOptions.Center;
        if (fuente != null) btnContTMP.font = fuente;

        // Manager
        var mgrGO = new GameObject("CertificadoManagerObj");
        var mgr = mgrGO.AddComponent<CertificadoManager>();
        mgr.textoNombreJugador = nombreTMP;
        mgr.textoPlasticos = txtPlasticos;
        mgr.textoEnemigos = txtEnemigos;
        mgr.textoTiempo = txtTiempo;
        mgr.botonDescargar = btnDescGO;
        mgr.textoBotonDescargar = btnDescTMP;
        mgr.panelConfirmacion = panelConfGO;
        mgr.textoConfirmacion = txtConfTMP;

        UnityEditor.Events.UnityEventTools.AddPersistentListener(btnDesc.onClick, mgr.DescargarCertificado);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btnCont.onClick, mgr.Continuar);

        EditorSceneManager.SaveScene(scene, path);
        // Agregar a BuildSettings
        var scenes = EditorBuildSettings.scenes;
        bool yaExiste = false;
        foreach (var s in scenes) if (s.path == path) yaExiste = true;
        if (!yaExiste)
        {
            var list = new System.Collections.Generic.List<EditorBuildSettingsScene>(scenes);
            list.Add(new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = list.ToArray();
        }
        Debug.Log("Escena certificado creada: " + path);
    }
}
