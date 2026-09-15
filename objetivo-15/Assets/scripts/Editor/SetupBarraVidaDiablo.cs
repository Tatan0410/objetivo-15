using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public static class SetupBarraVidaDiablo
{
    const string PREFAB_PATH = "Assets/prefabs/BarraVidaDiablo.prefab";

    [MenuItem("Objetivo15/Crear Prefab Barra Vida Diablo")]
    public static void Ejecutar()
    {
        GameObject raiz = CrearBarraVida();
        PrefabUtility.SaveAsPrefabAsset(raiz, PREFAB_PATH);
        Object.DestroyImmediate(raiz);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[SetupBarraVidaDiablo] Prefab creado/actualizado: " + PREFAB_PATH);
    }

    static GameObject CrearBarraVida()
    {
        GameObject raiz = new GameObject("BarraVidaDiablo", typeof(RectTransform));

        // Canvas world space
        Canvas canvas = raiz.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.scaleFactor = 1f;
        canvas.sortingOrder = 100;
        raiz.AddComponent<CanvasScaler>();
        raiz.AddComponent<GraphicRaycaster>();

        RectTransform rt = raiz.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(160, 20);

        // Slider
        Slider slider = raiz.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;
        slider.interactable = false;

        // Background
        GameObject bg = Hijo(raiz, "Background", new Vector2(160, 20), new Vector2(0, 0));
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0f, 0f, 0f, 0.65f);

        // Fill Area
        GameObject fillArea = Hijo(raiz, "Fill Area", new Vector2(160, 20), new Vector2(0, 0));
        RectTransform faRt = fillArea.GetComponent<RectTransform>();
        faRt.anchorMin = new Vector2(0, 0.5f);
        faRt.anchorMax = new Vector2(0, 0.5f);
        faRt.pivot = new Vector2(0, 0.5f);
        faRt.sizeDelta = new Vector2(150, 16);
        faRt.anchoredPosition = new Vector2(0, 0);

        // Fill
        GameObject fill = Hijo(fillArea, "Fill", new Vector2(150, 16), new Vector2(0, 0));
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = Color.green;

        RectTransform fillRt = fill.GetComponent<RectTransform>();
        fillRt.anchorMin = new Vector2(0, 0);
        fillRt.anchorMax = new Vector2(0, 1);
        fillRt.pivot = new Vector2(0, 0.5f);
        fillRt.sizeDelta = new Vector2(150, 0);

        // Conectar referencias del Slider
        slider.targetGraphic = bgImg;
        slider.fillRect = fillRt;
        slider.handleRect = null;

        return raiz;
    }

    static GameObject Hijo(GameObject padre, string nombre, Vector2 tam, Vector2 pos)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre.transform, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = tam;
        rt.anchoredPosition = pos;
        return go;
    }
}