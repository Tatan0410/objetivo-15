using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections.Generic;

public static class DeshacerSafeAreaMenu
{
    const string ESCENA = "Assets/Scenes/menuprincipal.unity";

    [MenuItem("Objetivo15/Revertir Safe Area Menu")]
    public static void Ejecutar()
    {
        EditorSceneManager.OpenScene(ESCENA, OpenSceneMode.Single);
        bool hecho = false;

        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        Transform root = canvas != null ? canvas.transform.Find("SafeAreaRoot") : null;

        if (root != null)
        {
            var hijos = new List<Transform>();
            foreach (Transform child in root)
                hijos.Add(child);

            foreach (Transform child in hijos)
                child.SetParent(canvas.transform, true);

            Object.DestroyImmediate(root.gameObject);
            hecho = true;
        }

        if (hecho)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), ESCENA);
        }
        AssetDatabase.Refresh();
        Debug.Log("[DeshacerSafeAreaMenu] Menu: " + (hecho ? "SafeArea revertido, botones restaurados" : "sin cambios (no habia SafeAreaRoot)"));
    }

    public static void EjecutarTodoBatch()
    {
        Ejecutar();
    }
}