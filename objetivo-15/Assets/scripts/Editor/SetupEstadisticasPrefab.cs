using UnityEditor;
using UnityEngine;

public static class SetupEstadisticasPrefab
{
    [MenuItem("Objetivo15/Crear Prefab EstadisticasManager")]
    public static void CrearPrefab()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");

        string path = "Assets/Resources/EstadisticasManager.prefab";
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null)
        {
            Debug.Log("Prefab ya existe: " + path);
            return;
        }

        var go = new GameObject("EstadisticasManager");
        go.AddComponent<EstadisticasManager>();
        var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        AssetDatabase.SaveAssets();
        Debug.Log("Prefab creado: " + path);
    }
}
