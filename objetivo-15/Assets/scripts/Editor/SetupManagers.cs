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

        string sheetPath = "Assets/Resources/Transicion/corriendo.png";
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

    [MenuItem("Tools/Sincronizar SceneTransitionManager en todas las escenas")]
    static void SincronizarTodasEscenas()
    {
        string menuPath = "Assets/Scenes/menuprincipal.unity";
        if (!System.IO.File.Exists(menuPath))
        {
            Debug.LogError($"No existe menuprincipal: {menuPath}");
            if (Application.isBatchMode) EditorApplication.Exit(1);
            return;
        }

        EditorSceneManager.OpenScene(menuPath);
        var canonical = Object.FindFirstObjectByType<SceneTransitionManager>();
        if (canonical == null)
        {
            Debug.LogError("No se encontro SceneTransitionManager en menuprincipal.");
            if (Application.isBatchMode) EditorApplication.Exit(1);
            return;
        }

        Sprite[] spritesCanonicos = canonical.spritesCorriendo != null ? canonical.spritesCorriendo.ToArray() : null;
        string[] datosCanonicos = canonical.datosCuriosos != null ? canonical.datosCuriosos.ToArray() : null;
        TMP_FontAsset fontCanonica = canonical.fontCarga;

        string[] niveles = {
            "nivel1_colegio", "nivel2_hipodromo", "nivel3_mercado",
            "nivel4_basurero", "nivel5_subterraneo", "nivel6_empresa"
        };

        int sincronizados = 0;
        foreach (var nivel in niveles)
        {
            string path = $"Assets/Scenes/{nivel}.unity";
            if (!System.IO.File.Exists(path))
            {
                Debug.LogWarning($"No existe la escena {path}");
                continue;
            }

            EditorSceneManager.OpenScene(path);
            var stm = Object.FindFirstObjectByType<SceneTransitionManager>();
            if (stm == null)
            {
                GameObject go = new GameObject("SceneTransitionManager");
                stm = go.AddComponent<SceneTransitionManager>();
            }

            float duracionNivel = stm.duracionMinima;

            stm.colorVerdeNeon = canonical.colorVerdeNeon;
            stm.fondoSprite = canonical.fondoSprite;
            stm.segmentosBarra = canonical.segmentosBarra;
            stm.spritesCorriendo = spritesCanonicos;
            stm.framerateAnimacion = canonical.framerateAnimacion;
            stm.escalaJugador = canonical.escalaJugador;
            stm.colorLoading = canonical.colorLoading;
            stm.colorPorcentaje = canonical.colorPorcentaje;
            stm.colorDato = canonical.colorDato;
            stm.posYLoading = canonical.posYLoading;
            stm.posYBarra = canonical.posYBarra;
            stm.posYPorcentaje = canonical.posYPorcentaje;
            stm.posYDato = canonical.posYDato;
            stm.posYJugador = canonical.posYJugador;
            stm.datosCuriosos = datosCanonicos;
            stm.fontCarga = fontCanonica;
            stm.duracionMinima = duracionNivel;

            stm.gameObject.name = "SceneTransitionManager";
            stm.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            stm.transform.localScale = Vector3.one;

            EditorUtility.SetDirty(stm);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
            sincronizados++;
            Debug.Log($"Sincronizado SceneTransitionManager en {nivel}");
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"SceneTransitionManager sincronizado en {sincronizados} escenas.");

        if (Application.isBatchMode) EditorApplication.Exit(0);
    }

    static void CrearGameOverManager()
    {
        var stm = Object.FindFirstObjectByType<GameOverManager>();
        bool esNuevo = false;

        if (stm == null)
        {
            GameObject go = new GameObject("GameOverManager");
            stm = go.AddComponent<GameOverManager>();
            Undo.RegisterCreatedObjectUndo(go, "Crear GameOverManager");
            esNuevo = true;
        }

        string prefabPath = "Assets/prefabs/GameOverPanel.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab != null)
        {
            stm.panelGameOverPrefab = prefab;
            EditorUtility.SetDirty(stm);
            Debug.Log("Prefab GameOver asignado al GameOverManager.");
        }
        else
        {
            Debug.LogWarning("No se encontró el prefab GameOverPanel en Assets/prefabs/. Crea el prefab manualmente o usa la escena existente.");
        }

        if (esNuevo)
            Debug.Log("GameOverManager creado en la escena.");
        else
            Debug.Log("GameOverManager actualizado (prefab asignado).");
    }
}
