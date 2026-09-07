using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public static class SetupCanecasCrafteo
{
    const string PREFAB_PATH = "Assets/prefabs/PanelCrafteoFijo.prefab";

    [MenuItem("Objetivo15/Agregar Canecas y Potenciadores al Panel")]
    public static void Ejecutar()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH);
        if (prefab == null)
        {
            Debug.LogError("[SetupCanecasCrafteo] No existe " + PREFAB_PATH);
            return;
        }

        string prefabPath = AssetDatabase.GetAssetPath(prefab);
        GameObject contenido = PrefabUtility.LoadPrefabContents(prefabPath);
        if (contenido == null)
        {
            Debug.LogError("[SetupCanecasCrafteo] No se pudo abrir el prefab");
            return;
        }

        try
        {
            ConfigurarPrefab(contenido);
            PrefabUtility.SaveAsPrefabAsset(contenido, prefabPath);
            AssetDatabase.SaveAssets();
            Debug.Log("[SetupCanecasCrafteo] Panel actualizado: 3 canecas + 3 botones de potenciador.");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(contenido);
        }
    }

    public static void EjecutarTodoBatch()
    {
        Ejecutar();
    }

    static void ConfigurarPrefab(GameObject root)
    {
        Transform panel = root.transform;
        TMP_FontAsset font = EncontrarFuente();

        CanecasCrafteo canecas = root.GetComponent<CanecasCrafteo>();
        if (canecas == null)
            canecas = root.AddComponent<CanecasCrafteo>();

        // Canecas (solo contadores)
        canecas.contadorCanecaReciclaje = CrearCaneca(panel, "CanecaReciclaje", new Vector2(-330, -240), font);
        canecas.contadorCanecaIcopor = CrearCaneca(panel, "CanecaIcopor", new Vector2(0, -240), font);
        canecas.contadorCanecaFruta = CrearCaneca(panel, "CanecaFruta", new Vector2(330, -240), font);

        // Potenciadores (boton + restante)
        string[] nombresBoton = { "Btn_CraftearPotenciadorVelocidad", "Btn_CraftearPotenciadorInmortalidad", "Btn_CraftearPotenciadorVida" };
        string[] etiquetas = { "VELOCIDAD\n2M+2B", "INMORTALIDAD\n3M+3B", "VIDA EXTRA\n4M+4B" };
        Vector2[] posiciones = { new Vector2(-330, -420), new Vector2(0, -420), new Vector2(330, -420) };

        canecas.contadorRestantePotenciador = new ContadorHUD[3];
        canecas.botonPotenciador = new Button[3];

        for (int i = 0; i < 3; i++)
        {
            GameObject grupo = CrearBotonPotenciador(panel, nombresBoton[i], etiquetas[i], posiciones[i], font);
            canecas.contadorRestantePotenciador[i] = grupo.GetComponentInChildren<ContadorHUD>(true);
            canecas.botonPotenciador[i] = grupo.GetComponent<Button>();
        }

        EditorUtility.SetDirty(root);
    }

    static ContadorHUD CrearCaneca(Transform parent, string nombre, Vector2 pos, TMP_FontAsset font)
    {
        Transform existente = parent.Find(nombre);
        if (existente != null)
            return existente.GetComponent<ContadorHUD>();

        GameObject go = new GameObject(nombre);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(120, 120);

        Image icono = CrearImagen("Icono", go.transform, new Vector2(0f, 18f), new Vector2(80, 80), Color.white);
        TextMeshProUGUI tmp = CrearTMP("Texto", go.transform, new Vector2(0f, -50f), new Vector2(120, 40), "0", 40, font, TextAlignmentOptions.Midline);

        ContadorHUD cont = go.AddComponent<ContadorHUD>();
        cont.icono = icono;
        cont.texto = tmp;
        cont.Actualizar(0);
        return cont;
    }

    static GameObject CrearBotonPotenciador(Transform parent, string nombre, string etiqueta, Vector2 pos, TMP_FontAsset font)
    {
        Transform existente = parent.Find(nombre);
        if (existente != null)
            return existente.gameObject;

        GameObject go = new GameObject(nombre);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(160, 120);

        Image img = go.AddComponent<Image>();
        img.color = new Color(0.2f, 0.6f, 0.2f, 0.9f);

        Button btn = go.AddComponent<Button>();
        btn.targetGraphic = img;

        TextMeshProUGUI label = CrearTMP("Texto", go.transform, new Vector2(0f, 30f), new Vector2(150, 50), etiqueta, 20, font, TextAlignmentOptions.Center);

        Image restanteIcono = CrearImagen("RestIcono", go.transform, new Vector2(-30f, -45f), new Vector2(28, 28), Color.white);
        TextMeshProUGUI restanteTxt = CrearTMP("Restante", go.transform, new Vector2(10f, -45f), new Vector2(100, 34), "3", 30, font, TextAlignmentOptions.MidlineLeft);

        ContadorHUD restante = go.AddComponent<ContadorHUD>();
        restante.icono = restanteIcono;
        restante.texto = restanteTxt;
        restante.Actualizar(3);

        return go;
    }

    static Image CrearImagen(string nombre, Transform parent, Vector2 pos, Vector2 tamano, Color color)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = tamano;
        Image img = go.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
        return img;
    }

    static TextMeshProUGUI CrearTMP(string nombre, Transform parent, Vector2 pos, Vector2 tamano, string texto, float size, TMP_FontAsset font, TextAlignmentOptions alineacion)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = tamano;
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = texto;
        tmp.fontSize = size;
        tmp.color = Color.white;
        tmp.alignment = alineacion;
        tmp.raycastTarget = false;
        if (font != null) tmp.font = font;
        return tmp;
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