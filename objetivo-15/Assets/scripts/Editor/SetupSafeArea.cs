using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public static class SetupSafeArea
{
    const string ESCENA = "Assets/Scenes/menuprincipal.unity";

    [MenuItem("Objetivo15/Aplicar Safe Area al Menu")]
    public static void Aplicar()
    {
        EditorSceneManager.OpenScene(ESCENA, OpenSceneMode.Single);
        bool hecho = false;

        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null && canvas.transform.Find("SafeAreaRoot") == null)
        {
            // Root que se ajusta al safe area (contiene todo el contenido del menu, menos el fondo)
            GameObject root = new GameObject("SafeAreaRoot", typeof(RectTransform));
            root.transform.SetParent(canvas.transform, false);
            RectTransform rt = root.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            root.AddComponent<SafeArea>();

            // Mover todos los hijos del canvas (menos el fondo y este root) dentro del SafeAreaRoot
            var hijos = new List<Transform>();
            foreach (Transform child in canvas.transform)
            {
                if (child == root.transform) continue;
                if (child.name == "imagenFondo") continue;
                hijos.Add(child);
            }
            foreach (Transform child in hijos)
                child.SetParent(root.transform, true);

            hecho = true;
        }

        if (hecho)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), ESCENA);
        }
        AssetDatabase.Refresh();
        Debug.Log("[SetupSafeArea] Menu: " + (hecho ? "SafeArea aplicado" : "sin cambios (ya aplicado)"));
    }

    public static void EjecutarTodoBatch()
    {
        Aplicar();
    }
}