using UnityEditor;
using UnityEngine;
using TMPro;

public static class SetupContadorVidaBoss
{
    const string PREFAB_PATH = "Assets/prefabs/ContadorVidaBoss.prefab";
    const string PREFAB_ERDIABLO = "Assets/prefabs/erdiablo.prefab";

    [MenuItem("Objetivo15/Crear Prefab Contador Vida Boss")]
    public static void Ejecutar()
    {
        GameObject raiz = CrearContador();
        PrefabUtility.SaveAsPrefabAsset(raiz, PREFAB_PATH);
        Object.DestroyImmediate(raiz);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[SetupContadorVidaBoss] Prefab creado/actualizado: " + PREFAB_PATH);
    }

    static GameObject CrearContador()
    {
        GameObject go = new GameObject("ContadorVidaBoss");

        if (go.GetComponent<MeshRenderer>() == null)
            go.AddComponent<MeshRenderer>();

        TextMeshPro tmp = go.AddComponent<TextMeshPro>();
        tmp.text = "0 / 0";
        tmp.fontSize = 8;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = false;

        if (TMP_Settings.defaultFontAsset != null)
            tmp.font = TMP_Settings.defaultFontAsset;

        // Sorting: misma capa que el boss para que nunca quede detras del fondo
        SpriteRenderer sr = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_ERDIABLO)
            ?.GetComponent<SpriteRenderer>();
        MeshRenderer rend = go.GetComponent<MeshRenderer>();
        if (sr != null)
        {
            rend.sortingLayerID = sr.sortingLayerID;
            rend.sortingOrder = sr.sortingOrder + 100;
        }
        else
        {
            rend.sortingOrder = 100;
        }

        return go;
    }
}