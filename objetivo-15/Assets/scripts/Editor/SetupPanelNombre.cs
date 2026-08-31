using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class SetupPanelNombre
{
    [MenuItem("Objetivo15/Crear Panel Nombre")]
    public static void CrearPanel()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/menuprincipal.unity");
        var canvas = GameObject.Find("Canvas");
        if (canvas == null) canvas = Object.FindFirstObjectByType<Canvas>()?.gameObject;
        if (canvas == null) { Debug.LogError("Canvas no encontrado"); return; }

        // Evitar duplicado
        var existente = GameObject.Find("PanelInputNombre");
        if (existente != null) { Debug.Log("PanelInputNombre ya existe"); return; }

        // Fuente del juego
        TMP_FontAsset fuente = null;
        var tmpGo = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmpGo != null) fuente = tmpGo.font;

        GameObject panel = new GameObject("PanelInputNombre", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(canvas.transform, false);
        var rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        panel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.85f);
        panel.transform.SetAsLastSibling();

        // Título
        var tituloGO = new GameObject("TituloBienvenida", typeof(RectTransform));
        tituloGO.transform.SetParent(panel.transform, false);
        var tituloRT = tituloGO.GetComponent<RectTransform>();
        tituloRT.anchorMin = new Vector2(0.5f, 0.7f); tituloRT.anchorMax = new Vector2(0.5f, 0.7f); tituloRT.pivot = new Vector2(0.5f, 0.5f);
        tituloRT.anchoredPosition = Vector2.zero; tituloRT.sizeDelta = new Vector2(800, 80);
        var tituloTMP = tituloGO.AddComponent<TextMeshProUGUI>();
        tituloTMP.text = "¡Bienvenido a Objetivo 15! ¿Cómo te llamas?";
        tituloTMP.fontSize = 32; tituloTMP.color = Color.white; tituloTMP.alignment = TextAlignmentOptions.Center;
        if (fuente != null) tituloTMP.font = fuente;

        // InputField
        var inputGO = new GameObject("InputNombre", typeof(RectTransform));
        inputGO.transform.SetParent(panel.transform, false);
        var inputRT = inputGO.GetComponent<RectTransform>();
        inputRT.anchorMin = new Vector2(0.5f, 0.5f); inputRT.anchorMax = new Vector2(0.5f, 0.5f);
        inputRT.anchoredPosition = new Vector2(0, 0); inputRT.sizeDelta = new Vector2(500, 60);
        var bgImg = inputGO.AddComponent<Image>(); bgImg.color = new Color(1f, 1f, 1f, 0.9f);
        var inputField = inputGO.AddComponent<TMP_InputField>();
        // TextArea
        var textArea = new GameObject("Text Area", typeof(RectTransform));
        textArea.transform.SetParent(inputGO.transform, false);
        var taRT = textArea.GetComponent<RectTransform>();
        taRT.anchorMin = Vector2.zero; taRT.anchorMax = Vector2.one; taRT.offsetMin = new Vector2(10, 6); taRT.offsetMax = new Vector2(-10, -6);
        var placeholderGO = new GameObject("Placeholder", typeof(RectTransform));
        placeholderGO.transform.SetParent(textArea.transform, false);
        var phRT = placeholderGO.GetComponent<RectTransform>();
        phRT.anchorMin = Vector2.zero; phRT.anchorMax = Vector2.one; phRT.offsetMin = Vector2.zero; phRT.offsetMax = Vector2.zero;
        var phTMP = placeholderGO.AddComponent<TextMeshProUGUI>();
        phTMP.text = "Escribe tu nombre..."; phTMP.fontSize = 24; phTMP.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        phTMP.alignment = TextAlignmentOptions.Left; if (fuente != null) phTMP.font = fuente;
        var textGO = new GameObject("Text", typeof(RectTransform));
        textGO.transform.SetParent(textArea.transform, false);
        var tRT = textGO.GetComponent<RectTransform>();
        tRT.anchorMin = Vector2.zero; tRT.anchorMax = Vector2.one; tRT.offsetMin = Vector2.zero; tRT.offsetMax = Vector2.zero;
        var txtTMP = textGO.AddComponent<TextMeshProUGUI>();
        txtTMP.text = ""; txtTMP.fontSize = 24; txtTMP.color = Color.black; txtTMP.alignment = TextAlignmentOptions.Left;
        if (fuente != null) txtTMP.font = fuente;
        inputField.textViewport = taRT;
        inputField.textComponent = txtTMP;
        inputField.placeholder = phTMP;
        inputField.targetGraphic = bgImg;

        // Botón Confirmar
        var btnGO = new GameObject("BotonConfirmarNombre", typeof(RectTransform));
        btnGO.transform.SetParent(panel.transform, false);
        var btnRT = btnGO.GetComponent<RectTransform>();
        btnRT.anchorMin = new Vector2(0.5f, 0.3f); btnRT.anchorMax = new Vector2(0.5f, 0.3f);
        btnRT.anchoredPosition = Vector2.zero; btnRT.sizeDelta = new Vector2(250, 55);
        var btnImg = btnGO.AddComponent<Image>(); btnImg.color = new Color(0.2f, 0.6f, 0.3f, 1f);
        var btn = btnGO.AddComponent<Button>(); btn.targetGraphic = btnImg;
        var btnTxtGO = new GameObject("Text", typeof(RectTransform));
        btnTxtGO.transform.SetParent(btnGO.transform, false);
        var btrRT = btnTxtGO.GetComponent<RectTransform>();
        btrRT.anchorMin = Vector2.zero; btrRT.anchorMax = Vector2.one; btrRT.offsetMin = Vector2.zero; btrRT.offsetMax = Vector2.zero;
        var btnTMP = btnTxtGO.AddComponent<TextMeshProUGUI>();
        btnTMP.text = "Confirmar"; btnTMP.fontSize = 26; btnTMP.color = Color.white; btnTMP.alignment = TextAlignmentOptions.Center;
        if (fuente != null) btnTMP.font = fuente;

        // Manager
        var mgrGO = new GameObject("InputNombreManager");
        mgrGO.transform.SetParent(canvas.transform, false);
        var mgr = mgrGO.AddComponent<InputNombreJugador>();
        mgr.panelInputNombre = panel;
        mgr.inputNombre = inputField;
        mgr.botonConfirmar = btn;

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("PanelInputNombre creado en menuprincipal.unity");
    }
}
