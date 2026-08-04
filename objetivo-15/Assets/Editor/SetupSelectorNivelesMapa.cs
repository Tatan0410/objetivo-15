using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

public class SetupSelectorNivelesMapa
{
    [MenuItem("Tools/Setup Selector Niveles Mapa")]
    public static void OpenSetup()
    {
        string scenePath = "Assets/Scenes/Mapamundial.unity";
        if (System.IO.File.Exists(scenePath))
            EditorSceneManager.OpenScene(scenePath);
        else
        {
            Debug.LogError("Mapamundial.unity not found at " + scenePath);
            return;
        }

        SetupManager();
    }

    static void SetupManager()
    {
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No se encontro un Canvas en la escena");
            return;
        }

        string[] nombresBotones = { "nivelone", "niveltwo", "nivelthree", "nivelfour", "nivelfive", "nivelsix" };
        string[] escenasCutscene = { "cutscene_0", "cutscene_1", "cutscene_2", "cutscene_3", "cutscene_4", "cutscene_5" };

        GameObject existingManager = GameObject.Find("SelectorNivelesManager");
        if (existingManager != null)
            Object.DestroyImmediate(existingManager);

        GameObject managerObj = new GameObject("SelectorNivelesManager");
        managerObj.transform.SetParent(canvas.transform, false);

        SelectorNivelesMapa selector = managerObj.AddComponent<SelectorNivelesMapa>();

        NodoNivelMapa[] nodos = new NodoNivelMapa[6];
        for (int i = 0; i < 6; i++)
        {
            NodoNivelMapa nodo = new NodoNivelMapa();
            nodo.numeroNivel = i + 1;
            nodo.nombreEscenaCutscene = escenasCutscene[i];
            nodo.nombreBoton = nombresBotones[i];

            GameObject btnObj = GameObject.Find(nombresBotones[i]);
            if (btnObj != null)
            {
                nodo.boton = btnObj.GetComponent<Button>();
                if (nodo.boton == null)
                    Debug.LogWarning("El boton " + nombresBotones[i] + " no tiene componente Button");
            }
            else
            {
                Debug.LogWarning("No se encontro el boton " + nombresBotones[i] + " en la escena");
            }

            nodos[i] = nodo;
        }

        selector.nodos = nodos;

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("SelectorNivelesMapa configurado correctamente!");
    }
}
