using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using System.IO;

public static class SetupGameOverPrefab
{
    const string prefabPath = "Assets/prefabs/GameOverPanel.prefab";

    [MenuItem("Tools/Generar Prefab GameOver")]
    public static void GenerarPrefab()
    {
        GameObject root = new GameObject("CanvasGameOver", typeof(RectTransform));
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 998;
        root.AddComponent<CanvasScaler>();
        root.AddComponent<GraphicRaycaster>();

        // Fondo
        GameObject fondo = new GameObject("Fondo", typeof(RectTransform));
        fondo.transform.SetParent(root.transform, false);
        Image imgFondo = fondo.AddComponent<Image>();
        imgFondo.color = new Color(0f, 0f, 0f, 0.78f);
        RectTransform rtFondo = fondo.GetComponent<RectTransform>();
        rtFondo.anchorMin = Vector2.zero;
        rtFondo.anchorMax = Vector2.one;
        rtFondo.sizeDelta = Vector2.zero;

        // Texto GAME OVER
        GameObject txtGO = new GameObject("TextoGameOver", typeof(RectTransform));
        txtGO.transform.SetParent(root.transform, false);
        TMP_Text tmpGO = txtGO.AddComponent<TextMeshProUGUI>();
        tmpGO.text = "GAME OVER";
        tmpGO.fontSize = 56;
        tmpGO.alignment = TextAlignmentOptions.Center;
        tmpGO.color = new Color(0.9f, 0.2f, 0.2f);
        tmpGO.fontStyle = FontStyles.Bold;
        RectTransform rtGO = txtGO.GetComponent<RectTransform>();
        rtGO.anchorMin = new Vector2(0.5f, 0.7f);
        rtGO.anchorMax = new Vector2(0.5f, 0.7f);
        rtGO.pivot = new Vector2(0.5f, 0.5f);
        rtGO.sizeDelta = new Vector2(400, 70);
        rtGO.anchoredPosition = Vector2.zero;

        // Botón Reintentar
        CrearBotonPrefab(root.transform, "Btn_Reintentar", "Reintentar",
            new Vector2(0.5f, 0.45f), new Color(0.2f, 0.6f, 0.2f));

        // Botón Menú Principal
        CrearBotonPrefab(root.transform, "Btn_Menú Principal", "Menú Principal",
            new Vector2(0.5f, 0.3f), new Color(0.6f, 0.2f, 0.2f));

        // Asegurar carpeta
        string dir = Path.GetDirectoryName(prefabPath);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        // Guardar prefab
        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        Object.DestroyImmediate(root);

        AssetDatabase.Refresh();
        Debug.Log("Prefab GameOver creado: " + prefabPath);
    }

    static void CrearBotonPrefab(Transform parent, string nombre, string texto, Vector2 anchorPos, Color colorBase)
    {
        GameObject btnGO = new GameObject(nombre, typeof(RectTransform));
        btnGO.transform.SetParent(parent, false);

        RectTransform rt = btnGO.GetComponent<RectTransform>();
        rt.anchorMin = anchorPos;
        rt.anchorMax = anchorPos;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(260, 50);
        rt.anchoredPosition = Vector2.zero;

        Image img = btnGO.AddComponent<Image>();
        img.color = colorBase;

        Button btn = btnGO.AddComponent<Button>();
        btn.targetGraphic = img;

        ColorBlock cb = btn.colors;
        cb.highlightedColor = colorBase * 1.3f;
        cb.pressedColor = colorBase * 0.7f;
        btn.colors = cb;

        GameObject txtGO = new GameObject("Texto", typeof(RectTransform));
        txtGO.transform.SetParent(btnGO.transform, false);
        TMP_Text tmp = txtGO.AddComponent<TextMeshProUGUI>();
        tmp.text = texto;
        tmp.fontSize = 24;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;

        RectTransform rtTxt = txtGO.GetComponent<RectTransform>();
        rtTxt.anchorMin = Vector2.zero;
        rtTxt.anchorMax = Vector2.one;
        rtTxt.sizeDelta = Vector2.zero;
    }

    public static string ObtenerRutaPrefab() => prefabPath;
}
