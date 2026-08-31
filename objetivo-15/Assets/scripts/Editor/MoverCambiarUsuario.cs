using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class MoverCambiarUsuario
{
    [MenuItem("Objetivo15/Mover CambiarUsuario a MenuPrincipal")]
    public static void Mover()
    {
        // 1. Limpiar Mapamundial
        var sceneMapa = EditorSceneManager.OpenScene("Assets/Scenes/Mapamundial.unity");
        var toDestroy = new System.Collections.Generic.List<GameObject>();
        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.scene.name != "Mapamundial") continue;
            if (go.name == "BotonCambiarUsuario" || go.name == "PanelInputCambiar" || go.name == "PanelConfirmacionCambiarUsuario" || go.name == "CambiarUsuarioManager")
                toDestroy.Add(go);
        }
        foreach (var go in toDestroy) Object.DestroyImmediate(go);
        EditorSceneManager.MarkSceneDirty(sceneMapa);
        EditorSceneManager.SaveScene(sceneMapa);
        Debug.Log($"Mapamundial limpiado: {toDestroy.Count} GOs eliminados");

        // 2. Crear en menuprincipal
        var sceneMenu = EditorSceneManager.OpenScene("Assets/Scenes/menuprincipal.unity");
        var canvas = GameObject.Find("Canvas");
        if (canvas == null) canvas = Object.FindFirstObjectByType<Canvas>()?.gameObject;
        if (canvas == null) { Debug.LogError("Canvas no encontrado en menuprincipal"); return; }

        // Limpiar duplicados previos en menuprincipal
        {
            var dupList = new System.Collections.Generic.List<GameObject>();
            foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (go.scene.name != "menuprincipal") continue;
                if (go.name == "BotonCambiarUsuario" || go.name == "PanelInputCambiar" || go.name == "PanelConfirmacionCambiarUsuario" || go.name == "CambiarUsuarioManager")
                    dupList.Add(go);
            }
            foreach (var go in dupList) Object.DestroyImmediate(go);
            if (dupList.Count > 0) Debug.Log($"Menuprincipal duplicados eliminados: {dupList.Count}");
        }
        // Crear de nuevo limpio
        {
            TMP_FontAsset fuente = null;
            var tmp = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp != null) fuente = tmp.font;

            var btnGO = new GameObject("BotonCambiarUsuario", typeof(RectTransform));
            btnGO.transform.SetParent(canvas.transform, false);
            var rt = btnGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f); rt.anchorMax = new Vector2(0f, 1f); rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(20, -80); rt.sizeDelta = new Vector2(300, 60);
            var img = btnGO.AddComponent<Image>(); img.color = new Color(0.2f, 0.4f, 0.8f, 1f);
            var btn = btnGO.AddComponent<Button>(); btn.targetGraphic = img;
            var txtGO = new GameObject("Text", typeof(RectTransform));
            txtGO.transform.SetParent(btnGO.transform, false);
            var txtRT = txtGO.GetComponent<RectTransform>(); txtRT.anchorMin = Vector2.zero; txtRT.anchorMax = Vector2.one; txtRT.offsetMin = Vector2.zero; txtRT.offsetMax = Vector2.zero;
            var txt = txtGO.AddComponent<TextMeshProUGUI>();
            txt.text = "Jugador"; txt.fontSize = 20; txt.color = Color.white; txt.alignment = TextAlignmentOptions.Center;
            if (fuente != null) txt.font = fuente;

            // Panel input
            var panelInput = new GameObject("PanelInputCambiar", typeof(RectTransform));
            panelInput.transform.SetParent(canvas.transform, false);
            var rtP = panelInput.GetComponent<RectTransform>();
            rtP.anchorMin = Vector2.zero; rtP.anchorMax = Vector2.one; rtP.offsetMin = Vector2.zero; rtP.offsetMax = Vector2.zero;
            var imgP = panelInput.AddComponent<Image>(); imgP.color = new Color(0f, 0f, 0f, 0.88f);
            panelInput.transform.SetAsLastSibling();
            var cajaGO = new GameObject("CajaInput", typeof(RectTransform));
            cajaGO.transform.SetParent(panelInput.transform, false);
            var cajaRT = cajaGO.GetComponent<RectTransform>();
            cajaRT.anchorMin = new Vector2(0.5f, 0.5f); cajaRT.anchorMax = new Vector2(0.5f, 0.5f); cajaRT.sizeDelta = new Vector2(600, 300);
            cajaRT.anchoredPosition = Vector2.zero;
            var cajaImg = cajaGO.AddComponent<Image>(); cajaImg.color = new Color(0.15f, 0.15f, 0.2f, 1f);
            var tituloGO = new GameObject("TituloCambiar", typeof(RectTransform));
            tituloGO.transform.SetParent(cajaGO.transform, false);
            var tituloRT = tituloGO.GetComponent<RectTransform>();
            tituloRT.anchorMin = new Vector2(0.5f, 0.85f); tituloRT.anchorMax = new Vector2(0.5f, 0.85f); tituloRT.sizeDelta = new Vector2(500, 50);
            tituloRT.anchoredPosition = Vector2.zero;
            var tituloTMP = tituloGO.AddComponent<TextMeshProUGUI>();
            tituloTMP.text = "Cambiar nombre"; tituloTMP.fontSize = 26; tituloTMP.color = Color.white; tituloTMP.alignment = TextAlignmentOptions.Center;
            if (fuente != null) tituloTMP.font = fuente;
            var inputGO = new GameObject("InputNuevoNombre", typeof(RectTransform));
            inputGO.transform.SetParent(cajaGO.transform, false);
            var inputRT = inputGO.GetComponent<RectTransform>();
            inputRT.anchorMin = new Vector2(0.5f, 0.55f); inputRT.anchorMax = new Vector2(0.5f, 0.55f); inputRT.sizeDelta = new Vector2(450, 55);
            inputRT.anchoredPosition = Vector2.zero;
            var inputBg = inputGO.AddComponent<Image>(); inputBg.color = new Color(1f, 1f, 1f, 0.95f);
            var inputField = inputGO.AddComponent<TMP_InputField>();
            var textArea = new GameObject("Text Area", typeof(RectTransform));
            textArea.transform.SetParent(inputGO.transform, false);
            var taRT = textArea.GetComponent<RectTransform>(); taRT.anchorMin = Vector2.zero; taRT.anchorMax = Vector2.one; taRT.offsetMin = new Vector2(10, 6); taRT.offsetMax = new Vector2(-10, -6);
            var phGO = new GameObject("Placeholder", typeof(RectTransform));
            phGO.transform.SetParent(textArea.transform, false);
            var phRT = phGO.GetComponent<RectTransform>(); phRT.anchorMin = Vector2.zero; phRT.anchorMax = Vector2.one; phRT.offsetMin = Vector2.zero; phRT.offsetMax = Vector2.zero;
            var phTMP = phGO.AddComponent<TextMeshProUGUI>(); phTMP.text = "Nuevo nombre..."; phTMP.fontSize = 20; phTMP.color = new Color(0.5f, 0.5f, 0.5f, 0.5f); phTMP.alignment = TextAlignmentOptions.Left;
            if (fuente != null) phTMP.font = fuente;
            var textGO = new GameObject("Text", typeof(RectTransform));
            textGO.transform.SetParent(textArea.transform, false);
            var tRT = textGO.GetComponent<RectTransform>(); tRT.anchorMin = Vector2.zero; tRT.anchorMax = Vector2.one; tRT.offsetMin = Vector2.zero; tRT.offsetMax = Vector2.zero;
            var txtTMP2 = textGO.AddComponent<TextMeshProUGUI>(); txtTMP2.text = ""; txtTMP2.fontSize = 20; txtTMP2.color = Color.black; txtTMP2.alignment = TextAlignmentOptions.Left;
            if (fuente != null) txtTMP2.font = fuente;
            inputField.textViewport = taRT; inputField.textComponent = txtTMP2; inputField.placeholder = phTMP; inputField.targetGraphic = inputBg;
            var btnConfGO = new GameObject("BotonConfirmarCambiar", typeof(RectTransform));
            btnConfGO.transform.SetParent(cajaGO.transform, false);
            var btnConfRT = btnConfGO.GetComponent<RectTransform>();
            btnConfRT.anchorMin = new Vector2(0.5f, 0.2f); btnConfRT.anchorMax = new Vector2(0.5f, 0.2f); btnConfRT.sizeDelta = new Vector2(200, 45);
            btnConfRT.anchoredPosition = new Vector2(-110, 0);
            var btnConfImg = btnConfGO.AddComponent<Image>(); btnConfImg.color = new Color(0.2f, 0.6f, 0.3f, 1f);
            var btnConf = btnConfGO.AddComponent<Button>(); btnConf.targetGraphic = btnConfImg;
            var btnConfTxtGO = new GameObject("Text", typeof(RectTransform));
            btnConfTxtGO.transform.SetParent(btnConfGO.transform, false);
            var bctRT = btnConfTxtGO.GetComponent<RectTransform>(); bctRT.anchorMin = Vector2.zero; bctRT.anchorMax = Vector2.one; bctRT.offsetMin = Vector2.zero; bctRT.offsetMax = Vector2.zero;
            var btnConfTMP = btnConfTxtGO.AddComponent<TextMeshProUGUI>(); btnConfTMP.text = "Confirmar"; btnConfTMP.fontSize = 20; btnConfTMP.color = Color.white; btnConfTMP.alignment = TextAlignmentOptions.Center;
            if (fuente != null) btnConfTMP.font = fuente;
            var btnCancGO = new GameObject("BotonCancelarCambiar", typeof(RectTransform));
            btnCancGO.transform.SetParent(cajaGO.transform, false);
            var btnCancRT = btnCancGO.GetComponent<RectTransform>();
            btnCancRT.anchorMin = new Vector2(0.5f, 0.2f); btnCancRT.anchorMax = new Vector2(0.5f, 0.2f); btnCancRT.sizeDelta = new Vector2(200, 45);
            btnCancRT.anchoredPosition = new Vector2(110, 0);
            var btnCancImg = btnCancGO.AddComponent<Image>(); btnCancImg.color = new Color(0.6f, 0.2f, 0.2f, 1f);
            var btnCanc = btnCancGO.AddComponent<Button>(); btnCanc.targetGraphic = btnCancImg;
            var btnCancTxtGO = new GameObject("Text", typeof(RectTransform));
            btnCancTxtGO.transform.SetParent(btnCancGO.transform, false);
            var bcaRT = btnCancTxtGO.GetComponent<RectTransform>(); bcaRT.anchorMin = Vector2.zero; bcaRT.anchorMax = Vector2.one; bcaRT.offsetMin = Vector2.zero; bcaRT.offsetMax = Vector2.zero;
            var btnCancTMP = btnCancTxtGO.AddComponent<TextMeshProUGUI>(); btnCancTMP.text = "Cancelar"; btnCancTMP.fontSize = 20; btnCancTMP.color = Color.white; btnCancTMP.alignment = TextAlignmentOptions.Center;
            if (fuente != null) btnCancTMP.font = fuente;
            panelInput.SetActive(false);

            var panelConf = new GameObject("PanelConfirmacionCambiarUsuario", typeof(RectTransform));
            panelConf.transform.SetParent(canvas.transform, false);
            var rtC = panelConf.GetComponent<RectTransform>();
            rtC.anchorMin = Vector2.zero; rtC.anchorMax = Vector2.one; rtC.offsetMin = Vector2.zero; rtC.offsetMax = Vector2.zero;
            var imgC = panelConf.AddComponent<Image>(); imgC.color = new Color(0f, 0f, 0f, 0.75f);
            panelConf.transform.SetAsLastSibling();
            var cajaCGO = new GameObject("CajaConfirmacion", typeof(RectTransform));
            cajaCGO.transform.SetParent(panelConf.transform, false);
            var cajaCRT = cajaCGO.GetComponent<RectTransform>();
            cajaCRT.anchorMin = new Vector2(0.5f, 0.5f); cajaCRT.anchorMax = new Vector2(0.5f, 0.5f); cajaCRT.sizeDelta = new Vector2(620, 280);
            cajaCRT.anchoredPosition = Vector2.zero;
            var cajaCImg = cajaCGO.AddComponent<Image>(); cajaCImg.color = new Color(0.15f, 0.15f, 0.2f, 1f);
            var txtCGO = new GameObject("TextoConfirmacionCambiar", typeof(RectTransform));
            txtCGO.transform.SetParent(cajaCGO.transform, false);
            var txtCRT = txtCGO.GetComponent<RectTransform>();
            txtCRT.anchorMin = new Vector2(0.5f, 0.7f); txtCRT.anchorMax = new Vector2(0.5f, 0.7f); txtCRT.sizeDelta = new Vector2(560, 90);
            txtCRT.anchoredPosition = Vector2.zero;
            var txtTMPc = txtCGO.AddComponent<TextMeshProUGUI>();
            txtTMPc.text = "estas seguro que deseas cambiar de usuario? todo el progreso se borara"; txtTMPc.fontSize = 18; txtTMPc.color = Color.white; txtTMPc.alignment = TextAlignmentOptions.Center; txtTMPc.enableWordWrapping = true;
            if (fuente != null) txtTMPc.font = fuente;
            var btnSiGO = new GameObject("BotonConfirmarSiCambiar", typeof(RectTransform));
            btnSiGO.transform.SetParent(cajaCGO.transform, false);
            var btnSiRT = btnSiGO.GetComponent<RectTransform>();
            btnSiRT.anchorMin = new Vector2(0.5f, 0.3f); btnSiRT.anchorMax = new Vector2(0.5f, 0.3f); btnSiRT.sizeDelta = new Vector2(200, 60);
            btnSiRT.anchoredPosition = new Vector2(-110, 0);
            var btnSiImg = btnSiGO.AddComponent<Image>(); btnSiImg.color = new Color(0.2f, 0.6f, 0.3f, 1f);
            var btnSi = btnSiGO.AddComponent<Button>(); btnSi.targetGraphic = btnSiImg;
            var btnSiTxtGO = new GameObject("Text", typeof(RectTransform));
            btnSiTxtGO.transform.SetParent(btnSiGO.transform, false);
            var bsiRT = btnSiTxtGO.GetComponent<RectTransform>(); bsiRT.anchorMin = Vector2.zero; bsiRT.anchorMax = Vector2.one; bsiRT.offsetMin = Vector2.zero; bsiRT.offsetMax = Vector2.zero;
            var btnSiTMP = btnSiTxtGO.AddComponent<TextMeshProUGUI>(); btnSiTMP.text = "Sí"; btnSiTMP.fontSize = 22; btnSiTMP.color = Color.white; btnSiTMP.alignment = TextAlignmentOptions.Center;
            if (fuente != null) btnSiTMP.font = fuente;
            var btnNoGO = new GameObject("BotonConfirmarNoCambiar", typeof(RectTransform));
            btnNoGO.transform.SetParent(cajaCGO.transform, false);
            var btnNoRT = btnNoGO.GetComponent<RectTransform>();
            btnNoRT.anchorMin = new Vector2(0.5f, 0.3f); btnNoRT.anchorMax = new Vector2(0.5f, 0.3f); btnNoRT.sizeDelta = new Vector2(200, 60);
            btnNoRT.anchoredPosition = new Vector2(110, 0);
            var btnNoImg = btnNoGO.AddComponent<Image>(); btnNoImg.color = new Color(0.6f, 0.2f, 0.2f, 1f);
            var btnNo = btnNoGO.AddComponent<Button>(); btnNo.targetGraphic = btnNoImg;
            var btnNoTxtGO = new GameObject("Text", typeof(RectTransform));
            btnNoTxtGO.transform.SetParent(btnNoGO.transform, false);
            var bnoRT = btnNoTxtGO.GetComponent<RectTransform>(); bnoRT.anchorMin = Vector2.zero; bnoRT.anchorMax = Vector2.one; bnoRT.offsetMin = Vector2.zero; bnoRT.offsetMax = Vector2.zero;
            var btnNoTMP = btnNoTxtGO.AddComponent<TextMeshProUGUI>(); btnNoTMP.text = "No"; btnNoTMP.fontSize = 22; btnNoTMP.color = Color.white; btnNoTMP.alignment = TextAlignmentOptions.Center;
            if (fuente != null) btnNoTMP.font = fuente;
            panelConf.SetActive(false);

            var mgrGO = new GameObject("CambiarUsuarioManager");
            mgrGO.transform.SetParent(canvas.transform, false);
            var mgr = mgrGO.AddComponent<CambiarUsuarioManager>();
            mgr.botonCambiarUsuario = btnGO.GetComponent<Button>();
            mgr.textoBotonNombre = txt;
            mgr.panelInputCambiar = panelInput;
            mgr.inputNuevoNombre = inputField;
            mgr.botonConfirmarNombre = btnConf;
            var cancBtn = btnCanc;
            UnityEditor.Events.UnityEventTools.AddPersistentListener(cancBtn.onClick, mgr.CerrarPanelCambiarNombre);
            mgr.panelConfirmacionCambiar = panelConf;
            mgr.textoConfirmacionCambiar = txtTMPc;
            mgr.botonSi = btnSi;
            mgr.botonNo = btnNo;
            UnityEditor.Events.UnityEventTools.AddPersistentListener(btnSi.onClick, mgr.SiCambiarUsuario);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(btnNo.onClick, mgr.CerrarConfirmacionCambiar);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(btnGO.GetComponent<Button>().onClick, mgr.AbrirPanelCambiarNombre);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(btnConf.onClick, mgr.ConfirmarNuevoNombre);
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("CambiarUsuario movido a menuprincipal");
    }
}
