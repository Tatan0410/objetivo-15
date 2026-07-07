using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using System.Linq;

public static class SetupManagers
{
    [MenuItem("Tools/Setup Managers en MenuPrincipal")]
    static void Ejecutar()
    {
        SetupFonts.GenerarFontAssets();

        string scenePath = "Assets/Scenes/menuprincipal.unity";
        EditorSceneManager.OpenScene(scenePath);

        CrearSceneTransitionManager();
        CrearGameOverManager();

        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.Refresh();
        Debug.Log("Managers creados en menuprincipal. Asigna sprites desde el Inspector.");
    }

    static void CrearSceneTransitionManager()
    {
        var stm = Object.FindFirstObjectByType<SceneTransitionManager>();
        bool esNuevo = false;

        if (stm == null)
        {
            GameObject go = new GameObject("SceneTransitionManager");
            stm = go.AddComponent<SceneTransitionManager>();
            Undo.RegisterCreatedObjectUndo(go, "Crear SceneTransitionManager");
            esNuevo = true;
        }

        string[] fontAssets = AssetDatabase.FindAssets("t:TMP_FontAsset", new[] { "Assets/fonts" });
        if (fontAssets.Length > 0)
        {
            TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(fontAssets[0]));
            stm.fontCarga = font;
            EditorUtility.SetDirty(stm);
            Debug.Log($"Font asignado: {font.name}");
        }

        string sheetPath = "Assets/WhatsApp_Image_2026-05-29_at_16.10.28-removebg-preview (1).png";
        Sprite[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(sheetPath)
            .OfType<Sprite>().ToArray();
        if (sprites.Length >= 4)
        {
            stm.spritesCorriendo = new Sprite[] { sprites[0], sprites[1], sprites[2], sprites[3] };
            EditorUtility.SetDirty(stm);
            Debug.Log("Sprites de correr asignados automaticamente.");
        }
        else
        {
            Debug.LogWarning("No se encontraron los 4 sprites de correr. Asignalos manualmente.");
        }

        if (esNuevo)
            Debug.Log("SceneTransitionManager creado en la escena.");
        else
            Debug.Log("SceneTransitionManager actualizado (font + sprites).");
    }

    static void CrearGameOverManager()
    {
        var existente = Object.FindFirstObjectByType<GameOverManager>();
        if (existente != null)
        {
            Debug.Log("GameOverManager ya existe en la escena.");
            return;
        }

        GameObject go = new GameObject("GameOverManager");
        go.AddComponent<GameOverManager>();
        Undo.RegisterCreatedObjectUndo(go, "Crear GameOverManager");
    }
}
