using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class SetupCambiarUsuario
{
    [MenuItem("Objetivo15/Crear UI Cambiar Usuario")]
    public static void CrearUI()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Mapamundial.unity");
        var canvas = GameObject.Find("Canvas");
        if (canvas == null) canvas = Object.FindFirstObjectByType<Canvas>()?.gameObject;
        if (canvas == null) { Debug.LogError("Canvas no encontrado"); return; }

        if (GameObject.Find("BotonCambiarUsuario") != null) { Debug.Log("BotonCambiarUsuario ya existe"); }
        else
        {
            TMP_FontAsset fuente = null;
            var tmp = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp != null) fuente = tmp.font;

            var btnGO = new GameObject("BotonCambiarUsuario", typeof(RectTransform));
            btnGO.transform.SetParent(canvas.transform, false);
            var rt = btnGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f); rt.anchorMax = new Vector2(0f, 1f); rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(20, -20); rt.sizeDelta = new Vector2(300, 60);
            var img = btnGO.AddComponent<Image>(); img.color = new Color(0.2f, 0.4f, 0.8f, 1f);
            var btn = btnGO.AddComponent<Button>(); btn.targetGraphic = img;
            var txtGO = new GameObject("Text", typeof(RectTransform));
            txtGO.transform.SetParent(btnGO.transform, false);
            var txtRT = txtGO.GetComponent<RectTransform>(); txtRT.anchorMin = Vector2.zero; txtRT.anchorMax = Vector2.one; txtRT.offsetMin = Vector2.zero; txtRT.offsetMax = Vector2.zero;
            var txt = txtGO.AddComponent<TextMeshProUGUI>();
            txt.text = "👤 Jugador"; txt.fontSize = 20; txt.color = Color.white; txt.alignment = TextAlignmentOptions.Center;
            if (fuente != null) txt.font = fuente;
        }

        // Panel input
        GameObject panelInput = GameObject.Find("PanelInputCambiar");
        if (panelInput == null)
        {
            TMP_FontAsset fuente = null;
            var tmp = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp != null) fuente = tmp.font;
            panelInput = new GameObject("PanelInputCambiar", typeof(RectTransform));
            panelInput.transform.SetParent(canvas.transform, false);
            var rt = panelInput.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            var img = panelInput.AddComponent<Image>(); img.color = new Color(0f, 0f, 0f, 0.88f);
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
            var txtTMP = textGO.AddComponent<TextMeshProUGUI>(); txtTMP.text = ""; txtTMP.fontSize = 20; txtTMP.color = Color.black; txtTMP.alignment = TextAlignmentOptions.Left;
            if (fuente != null) txtTMP.font = fuente;
            inputField.textViewport = taRT; inputField.textComponent = txtTMP; inputField.placeholder = phTMP; inputField.targetGraphic = inputBg;

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
        }

        // Panel confirmación cambio
        GameObject panelConf = GameObject.Find("PanelConfirmacionCambiarUsuario");
        if (panelConf == null)
        {
            TMP_FontAsset fuente = null;
            var tmp = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp != null) fuente = tmp.font;
            panelConf = new GameObject("PanelConfirmacionCambiarUsuario", typeof(RectTransform));
            panelConf.transform.SetParent(canvas.transform, false);
            var rt = panelConf.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            var img = panelConf.AddComponent<Image>(); img.color = new Color(0f, 0f, 0f, 0.75f);
            panelConf.transform.SetAsLastSibling();
            var cajaGO = new GameObject("CajaConfirmacion", typeof(RectTransform));
            cajaGO.transform.SetParent(panelConf.transform, false);
            var cajaRT = cajaGO.GetComponent<RectTransform>();
            cajaRT.anchorMin = new Vector2(0.5f, 0.5f); cajaRT.anchorMax = new Vector2(0.5f, 0.5f); cajaRT.sizeDelta = new Vector2(620, 280);
            cajaRT.anchoredPosition = Vector2.zero;
            var cajaImg = cajaGO.AddComponent<Image>(); cajaImg.color = new Color(0.15f, 0.15f, 0.2f, 1f);
            var txtGO = new GameObject("TextoConfirmacionCambiar", typeof(RectTransform));
            txtGO.transform.SetParent(cajaGO.transform, false);
            var txtRT = txtGO.GetComponent<RectTransform>();
            txtRT.anchorMin = new Vector2(0.5f, 0.7f); txtRT.anchorMax = new Vector2(0.5f, 0.7f); txtRT.sizeDelta = new Vector2(560, 90);
            txtRT.anchoredPosition = Vector2.zero;
            var txtTMP = txtGO.AddComponent<TextMeshProUGUI>();
            txtTMP.text = "estas seguro que deseas cambiar de usuario? todo el progreso se borara"; txtTMP.fontSize = 18; txtTMP.color = Color.white; txtTMP.alignment = TextAlignmentOptions.Center; txtTMP.enableWordWrapping = true;
            if (fuente != null) txtTMP.font = fuente;
            var btnSiGO = new GameObject("BotonConfirmarSiCambiar", typeof(RectTransform));
            btnSiGO.transform.SetParent(cajaGO.transform, false);
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
            btnNoGO.transform.SetParent(cajaGO.transform, false);
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
        }

        // Manager
        var mgrGO = GameObject.Find("CambiarUsuarioManager");
        CambiarUsuarioManager mgr = null;
        if (mgrGO == null) { mgrGO = new GameObject("CambiarUsuarioManager"); mgr = mgrGO.AddComponent<CambiarUsuarioManager>(); }
        else mgr = mgrGO.GetComponent<CambiarUsuarioManager>();
        if (mgr == null) mgr = mgrGO.AddComponent<CambiarUsuarioManager>();

        mgr.botonCambiarUsuario = GameObject.Find("BotonCambiarUsuario")?.GetComponent<Button>();
        var txtBtn = GameObject.Find("BotonCambiarUsuario")?.GetComponentInChildren<TextMeshProUGUI>();
        mgr.textoBotonNombre = txtBtn;
        mgr.panelInputCambiar = GameObject.Find("PanelInputCambiar");
        mgr.inputNuevoNombre = GameObject.Find("InputNuevoNombre")?.GetComponent<TMP_InputField>();
        mgr.botonConfirmarNombre = GameObject.Find("BotonConfirmarCambiar")?.GetComponent<Button>();
        var cancBtn = GameObject.Find("BotonCancelarCambiar")?.GetComponent<Button>();
        if (cancBtn != null) cancBtn.onClick.AddListener(mgr.CerrarPanelCambiarNombre);
        mgr.panelConfirmacionCambiar = GameObject.Find("PanelConfirmacionCambiarUsuario");
        mgr.textoConfirmacionCambiar = GameObject.Find("TextoConfirmacionCambiar")?.GetComponent<TextMeshProUGUI>();
        mgr.botonSi = GameObject.Find("BotonConfirmarSiCambiar")?.GetComponent<Button>();
        mgr.botonNo = GameObject.Find("BotonConfirmarNoCambiar")?.GetComponent<Button>();
        if (mgr.botonSi != null) { mgr.botonSi.onClick.RemoveAllListeners(); mgr.botonSi.onClick.AddListener(mgr.SiCambiarUsuario); }
        if (mgr.botonNo != null) { mgr.botonNo.onClick.RemoveAllListeners(); mgr.botonNo.onClick.AddListener(mgr.CerrarConfirmacionCambiar); }

        var btnCambiar = GameObject.Find("BotonCambiarUsuario")?.GetComponent<Button>();
        if (btnCambiar != null) { btnCambiar.onClick.RemoveAllListeners(); btnCambiar.onClick.AddListener(mgr.AbrirPanelCambiarNombre); }

        EditorUtility.SetDirty(mgr.gameObject);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("UI Cambiar Usuario creada en Mapamundial");
    }
}
